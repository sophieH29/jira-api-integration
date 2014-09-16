using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HelloWorld.Code.DataAccess;

namespace HelloWorld.Code.Util
{
    public static class CacheManager
    {
        /// <summary>
        /// Get the list of contact reasons
        /// </summary>
        /// <param name="getFromCache"></param>
        /// <returns></returns>
        public static List<ContactReason> GetContactReasons(bool getFromCache)
        {
            return (List<ContactReason>)GetFromCache("ContactReasons", getFromCache, delegate
            {
                using (var context = new MvcDemoEntities())
                {
                    return context.ContactReasons.OrderBy(c => c.ContactReasonText).ToList();
                }
            });
        }

        /// <summary>
        /// Get the list of contact reasons
        /// </summary>
        /// <param name="getFromCache"></param>
        /// <returns></returns>
        public static List<UserRole> GetUserRoles(bool getFromCache)
        {
            return (List<UserRole>)GetFromCache("UserRoles", getFromCache, delegate
            {
                using (var context = new MvcDemoEntities())
                {
                    return context.UserRoles.OrderBy(ur => ur.UserRoleName).ToList();
                }
            });
        }

        #region Private methods

        /// <summary>
        /// Helper method for adding/retrieving a value from/to the cache.
        /// </summary>
        /// <param name="cacheKey">The key of the cached item</param>
        /// <param name="f">If the key is not found in the cache execute the f() function to retrieve the object</param>
        /// <returns></returns>
        private static object GetFromCache(string cacheKey, Func<object> f)
        {
            return GetFromCache(cacheKey, true, f);
        }

        /// <summary>
        /// Helper method for adding/retrieving a value from/to the cache.
        /// </summary>
        /// <param name="cacheKey">The key of the cached item</param>
        /// <param name="getFromCache">True if a cached vaersion cand be returned. If false, f() will be used to get the data</param>
        /// <param name="f">If the key is not found in the cache execute the f() function to retrieve the object</param>
        /// <returns></returns>
        private static object GetFromCache(string cacheKey, bool getFromCache, Func<object> f)
        {
            object cachedItem = null;
            if (getFromCache)
                cachedItem = HttpRuntime.Cache[cacheKey];

            if (cachedItem == null)
            {
                cachedItem = f();
                if (cachedItem != null)
                    HttpRuntime.Cache.Insert(cacheKey, cachedItem, null, DateTime.Now.AddSeconds(WebConfigSettings.CachingTime), TimeSpan.Zero);
            }

            return cachedItem;
        }

        #endregion
    }
}