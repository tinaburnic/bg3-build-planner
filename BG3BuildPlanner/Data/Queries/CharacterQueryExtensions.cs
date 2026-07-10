using System.Linq;
using BG3BuildPlanner.Data;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Data.Queries;

public static class CharacterQueryExtensions
{
    public static IQueryable<Character> Active(this IQueryable<Character> characters)
        => characters.Where(c => c.DeletedAt == null);

    public static IQueryable<Character> SearchName(this IQueryable<Character> characters, string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return characters;
        }

        var q = query.Trim();
        return characters.Where(c => c.Name != null && EF.Functions.Like(c.Name, $"%{q}%"));
    }
}