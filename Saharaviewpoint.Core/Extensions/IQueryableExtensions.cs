using Microsoft.EntityFrameworkCore;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.Core.Extensions
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Returns a paginated list.
        /// </summary>
        /// <typeparam name="T">Type of items in list.</typeparam>
        /// <param name="source">A IQueryable instance to apply.</param>
        /// <param name="pageIndex">Page number that starts with zero.</param>
        /// <param name="pageSize">Size of each page.</param>
        /// <returns>Returns a paginated list.</returns>
        public static async Task<PagedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            return await CreateAsync(source, pageIndex, pageSize);
        }

        public static async Task<PagedList<T>> ToPaginatedListAsync<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return await CreateAsync(source, pageIndex, pageSize);
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

        /// <summary>
        /// Returns a paginated list. This function returns 15 rows from page one.
        /// </summary>
        /// <typeparam name="T">Type of items in list.</typeparam>
        /// <param name="source">A IQueryable instance to apply.</param>
        /// <returns>Returns a paginated list.</returns>
        /// <remarks>This functionality may not work in SQL Compact 3.5</remarks>
        public static async Task<PagedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> source)
        {
            return await CreateAsync(source, 1, 15);
        }

        private static async Task<PagedList<T>> CreateAsync<T>(IQueryable<T> source, int pageIndex, int pageSize)
        {
            try
            {
                var count = await source.CountAsync();
                var items = pageSize == 0
                    ? await source.ToListAsync()
                    : await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                return new PagedList<T>(items, count, pageIndex, pageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static async Task<PagedList<T>> CreateAsync<T>(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            try
            {
                var count = source.Count();
                var items = pageSize == 0
                    ? source.ToList()
                    : source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return new PagedList<T>(items, count, pageIndex, pageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}