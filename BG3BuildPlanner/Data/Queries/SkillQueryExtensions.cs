using System.Linq;
using BG3BuildPlanner.Data;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Data.Queries;

public static class SkillQueryExtensions
{
    public static IQueryable<Skill> Active(this IQueryable<Skill> skills)
        => skills.Where(s => s.DeletedAt == null);

    public static IQueryable<Skill> SearchName(this IQueryable<Skill> skills, string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return skills;
        }

        var q = query.Trim();
        return skills.Where(s => s.Name != null && EF.Functions.Like(s.Name, $"%{q}%"));
    }
}
