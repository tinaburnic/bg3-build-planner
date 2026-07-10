using System.Linq;
using BG3BuildPlanner.Data;
using Microsoft.EntityFrameworkCore;

namespace BG3BuildPlanner.Data.Queries;

public static class ItemQueryExtensions
{
    public static IQueryable<Item> SearchName(this IQueryable<Item> items, string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return items;
        }

        var q = query.Trim();
        return items.Where(i => i.Name != null && EF.Functions.Like(i.Name, $"%{q}%"));
    }
}