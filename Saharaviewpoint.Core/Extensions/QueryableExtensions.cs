// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Microsoft.EntityFrameworkCore;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Extensions;

public static class QueryableExtensions
{
    /// <summary>
    /// Returns a paginated list.
    /// </summary>
    /// <typeparam name="T">Type of items in list.</typeparam>
    /// <param name="source">A IQueryable instance to apply.</param>
    /// <param name="pageIndex">Page number that starts with zero.</param>
    /// <param name="pageSize">Size of each page.</param>
    /// <returns>Returns a paginated list.</returns>
    public static async Task<PagedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageIndex,
        int pageSize)
    {
        return await CreateAsync(source, pageIndex, pageSize);
    }

    private static async Task<PagedList<T>> CreateAsync<T>(IQueryable<T> source, int pageIndex, int pageSize)
    {
        int count = await source.CountAsync();
        var items = pageSize == 0
            ? await source.ToListAsync()
            : await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, count, pageIndex, pageSize);
    }

    public static PagedList<T> ToPaginatedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
    {
        return Create(source, pageIndex, pageSize);
    }

    /// <summary>
    /// Returns a paginated list. This function returns 15 rows from specific pageIndex.
    /// </summary>
    /// <typeparam name="T">Type of items in list.</typeparam>
    /// <param name="source">A IQueryable instance to apply.</param>
    /// <param name="pageIndex">Page number that starts with zero.</param>
    /// <returns>Returns a paginated list.</returns>
    /// <remarks>This functionality may not work in SQL Compact 3.5</remarks>
    public static async Task<PagedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageIndex)
    {
        return await CreateAsync(source, pageIndex, 15);
    }


    private static PagedList<T> Create<T>(IEnumerable<T> source, int pageIndex, int pageSize)
    {
        IEnumerable<T> enumerable = source.ToList();
        int count = enumerable.Count();
        var items = pageSize == 0
            ? enumerable.ToList()
            : enumerable.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PagedList<T>(items, count, pageIndex, pageSize);
    }
}