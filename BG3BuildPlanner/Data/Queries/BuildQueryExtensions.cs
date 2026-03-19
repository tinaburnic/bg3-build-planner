using System;
using System.Linq;
using BG3BuildPlanner.Models;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Data.Queries;

public static class BuildQueryExtensions
{
	/// <summary>
	/// Filters builds belonging to a specific character.
	/// </summary>
	public static IQueryable<Build> ForCharacter(this IQueryable<Build> builds, int characterId)
		=> builds.Where(b => b.CharacterId == characterId);

	/// <summary>
	/// Filters builds belonging to a character by exact name (case-insensitive).
	/// </summary>
	public static IQueryable<Build> ForCharacter(this IQueryable<Build> builds, string characterName)
	{
		if (string.IsNullOrWhiteSpace(characterName))
		{
			return builds;
		}

		var name = characterName.Trim();
		return builds.Where(b => b.Character != null && b.Character.Name != null && b.Character.Name.ToLower() == name.ToLower());
	}

	/// <summary>
	/// Filters builds to a single difficulty.
	/// </summary>
	public static IQueryable<Build> WithDifficulty(this IQueryable<Build> builds, Difficulty difficulty)
		=> builds.Where(b => b.Difficulty == difficulty);

	/// <summary>
	/// Filters builds by optional difficulty.
	/// </summary>
	public static IQueryable<Build> WithDifficulty(this IQueryable<Build> builds, Difficulty? difficulty)
		=> difficulty is null ? builds : builds.Where(b => b.Difficulty == difficulty);

	/// <summary>
	/// Filters builds created after (or at) a given UTC timestamp.
	/// </summary>
	public static IQueryable<Build> CreatedSince(this IQueryable<Build> builds, DateTime utcTimestamp)
		=> builds.Where(b => b.CreatedAt >= utcTimestamp);

	/// <summary>
	/// Full-text-ish title search using SQL LIKE when supported (falls back to client for InMemory).
	/// </summary>
	public static IQueryable<Build> SearchTitle(this IQueryable<Build> builds, string? query)
	{
		if (string.IsNullOrWhiteSpace(query))	return builds;
		var q = query.Trim();
		return builds.Where(b => b.Title != null && EF.Functions.Like(b.Title, $"%{q}%"));
	}

	/// <summary>
	/// Filters builds containing at least one item of the given type.
	/// </summary>
	public static IQueryable<Build> WithItemType(this IQueryable<Build> builds, ItemType itemType)
		=> builds.Where(b => b.Items.Any(i => i.Type == itemType));

	/// <summary>
	/// Filters builds containing a skill name (case-insensitive exact match).
	/// </summary>
	public static IQueryable<Build> WithSkill(this IQueryable<Build> builds, string skillName)
	{
		if (string.IsNullOrWhiteSpace(skillName))	return builds;
		var name = skillName.Trim();
		return builds.Where(b => b.Skills.Any(s => s.Name != null && s.Name.ToLower() == name.ToLower()));
	}

	/// <summary>
	/// Eager-loads the common navigation properties used in build listing pages.
	/// </summary>
	public static IQueryable<Build> WithDetails(this IQueryable<Build> builds)
		=> builds
			.Include(b => b.Character)
			.Include(b => b.Items)
			.Include(b => b.Skills)
			.Include(b => b.Ratings);

	/// <summary>
	/// Orders builds by newest first.
	/// </summary>
	public static IQueryable<Build> OrderByNewest(this IQueryable<Build> builds)
		=> builds.OrderByDescending(b => b.CreatedAt);

	/// <summary>
	/// Orders builds by rating count (popularity) then newest.
	/// </summary>
	public static IQueryable<Build> OrderByPopularity(this IQueryable<Build> builds)
		=> builds
			.OrderByDescending(b => b.Ratings.Count)
			.ThenByDescending(b => b.CreatedAt);

	/// <summary>
	/// Returns the top builds by average rating (optionally requiring a minimum number of ratings).
	/// </summary>
	public static IQueryable<Build> TopByAverageRating(
		this IQueryable<Build> builds,
		int take,
		int minRatings = 1)
	{
		if (take <= 0)	return builds.Where(_ => false);
		if (minRatings < 0)	minRatings = 0;

		return builds
			.Where(b => b.Ratings.Count >= minRatings)
			.OrderByDescending(b => b.Ratings.Select(r => (double?)r.Score).Average() ?? 0)
			.ThenByDescending(b => b.Ratings.Count)
			.ThenByDescending(b => b.CreatedAt)
			.Take(take);
	}

	/// <summary>
	/// Projects builds into a small shape that includes rating stats.
	/// </summary>
	public static IQueryable<BuildRatingStats> SelectRatingStats(this IQueryable<Build> builds)
		=> builds.Select(b => new BuildRatingStats(
			b.Id,
			b.Title,
			b.Difficulty,
			b.CharacterId,
			b.Character != null ? b.Character.Name : null,
			b.Ratings.Select(r => (double?)r.Score).Average() ?? 0,
			b.Ratings.Count,
			b.CreatedAt));
}

public sealed record BuildRatingStats(
	int BuildId,
	string? Title,
	Difficulty Difficulty,
	int CharacterId,
	string? CharacterName,
	double AverageScore,
	int RatingCount,
	DateTime CreatedAt);
