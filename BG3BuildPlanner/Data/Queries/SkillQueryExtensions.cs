using System;
using System.Linq;
using BG3BuildPlanner.Data;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Data.Queries;

public static class SkillQueryExtensions
{
	/// <summary>
	/// Filters skills that are not soft-deleted.
	/// </summary>
	public static IQueryable<Skill> Active(this IQueryable<Skill> skills)
		=> skills.Where(s => s.DeletedAt == null);
}
