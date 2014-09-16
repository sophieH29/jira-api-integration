using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using HelloWorld.Code.DataAccess.Paging;

namespace HelloWorld.Code.DataAccess
{
    /// <summary>
    /// Base repository class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Return a paged list.
        /// </summary>
        /// <param name="filters">A lambda expression used to filter the result.</param>
        /// <param name="sorting">The sort expression, example: ColumnName desc</param>
        /// <param name="includeList">The list of related entities to load.</param>
        /// <param name="currentPageNumber">The page number to return.</param>
        /// <param name="pageSize">The page size (the maximum number of elements to return in the list).</param>
        /// <returns></returns>
        IPagedList<T> Search(Expression<Func<T, bool>> filters, string sorting, List<string> includeList, int currentPageNumber, int pageSize);
 
        /// <summary>
        /// Return a list.
        /// </summary>
        /// <param name="filters">A lambda expression used to filter the result.</param>
        /// <param name="sorting">The sort expression, example: ColumnName desc</param>
        /// <param name="includeList">The list of related entities to load.</param>
        /// <returns></returns>
        IList<T> Search(Expression<Func<T, bool>> filters, string sorting, List<string> includeList);

        /// <summary>
        /// Retrieve an entity from the repository based on the unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns></returns>
        T GetById(object id);

        /// <summary>
        /// Add a new item to the repository.
        /// </summary>
        /// <param name="entity">The element to add to the repository.</param>
        /// <returns></returns>
        void Insert(T entity);

        /// <summary>
        /// Updates an item in the repository.
        /// </summary>
        /// <param name="entity">The element to update.</param>
        /// <returns></returns>
        void Update(T entity);

        /// <summary>
        /// Deletes an item from the repository.
        /// </summary>
        /// <param name="entity">The item to be deleted.</param>
        /// <returns></returns>
        void Delete(T entity);

        /// <summary>
        /// Deletes an item from the repository.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to be deleted.</param>
        /// <returns></returns>
        void Delete(object id);
    }
}