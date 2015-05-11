using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Caroline.Persistence.Extensions
{
    public static class MongoCollectionEx
    {
        public static async Task<T> First<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> func, FindOptions<T, T> options = null)
        {
            var result = await collection.FindAsync(func, options);
            return result.Current.First();
        }

        public static async Task<T> FirstOrDefault<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> func, FindOptions<T, T> options = null)
        {
            var result = await collection.FindAsync(func, options);
            return result.Current.FirstOrDefault();
        }

        public static async Task<T> Single<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> func, FindOptions<T, T> options = null)
        {
            var result = await collection.FindAsync(func, options);
            return result.Current.Single();
        }

        public static async Task<T> SingleOrDefault<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> func, FindOptions<T, T> options = null)
        {
            var result = await collection.FindAsync(func, options);
            return result.Current.SingleOrDefault();
        }
    }
}
