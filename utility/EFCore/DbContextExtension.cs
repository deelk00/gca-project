using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Utility.Other.Extensions;

namespace Utility.EFCore
{
    public static class DbContextExtension
    {
        /// <summary>
        /// starts a transaction on the database, saves the changes automatically and commits or rolls them back
        /// </summary>
        /// <param name="context">database context</param>
        /// <param name="action">action to perform on the database</param>
        /// <returns></returns>
        public static async Task TransactionAsync(this DbContext context, Action<IDbContextTransaction> action)
        {
            var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                action(transaction);

                context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public static Task<T> TransactionAsync<T>(this DbContext context,
            Func<IDbContextTransaction, EntityEntry<T>> action)
            where T : class 
            => TransactionAsync(context, async transaction => action(transaction));
        
        public static async Task<T> TransactionAsync<T>(this DbContext context, Func<IDbContextTransaction, ValueTask<EntityEntry<T>>> action)
            where T : class
        {
            var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var result = await action(transaction);

                context.SaveChanges();

                transaction.Commit();

                return result.Entity;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public static IQueryable<T> IncludeComplexTypes<T>(this IQueryable<T> query)
            where T : class
        {
            var t = typeof(T);
            var complexEntityNames = query.GetAdjacentEntityTables();
            query = complexEntityNames.Select(x => x.Name).Aggregate(query, (current, name) => current.Include(name));
            return query;
        }
    }
}
