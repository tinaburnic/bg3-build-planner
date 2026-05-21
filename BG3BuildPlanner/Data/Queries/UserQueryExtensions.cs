using System.Linq;
using BG3BuildPlanner.Data;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Data.Queries;

public static class UserQueryExtensions
{
    /// <summary>
    /// Filters users that are not soft-deleted.
    /// </summary>
    public static IQueryable<User> Active(this IQueryable<User> users)
        => users.Where(u => u.DeletedAt == null);

    /// <summary>
    /// Full-text-ish username search using SQL LIKE when supported.
    /// </summary>
    public static IQueryable<User> SearchUsername(this IQueryable<User> users, string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return users;
        }

        var q = query.Trim();
        return users.Where(u => u.Username != null && EF.Functions.Like(u.Username, $"%{q}%"));
    }
}
