using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using HelloWorld.Code.DataAccess.Paging;

namespace HelloWorld.Code.DataAccess
{
    /// <summary>
    /// Base repository class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        internal MvcDemoEntities Context;
        internal ObjectSet<T> ObjectSet { get; private set; }
        internal readonly string EntitySetName;
        internal readonly string PrimaryKeyName;

        internal Repository(MvcDemoEntities context)
        {
            Context = context;
            ObjectSet = Context.CreateObjectSet<T>();
            
            // Build full name of entity set for current entity type
            EntitySetName = string.Format("{0}.{1}", context.DefaultContainerName, ObjectSet.EntitySet.Name);
            // Get name of the entity's key property
            PrimaryKeyName = ObjectSet.EntitySet.ElementType.KeyMembers.First().Name;            
        }

        /// <summary>
        /// Return a paged list.
        /// </summary>
        /// <param name="filters">A lambda expression used to filter the result.</param>
        /// <param name="sorting">The sort expression, example: ColumnName desc</param>
        /// <param name="includeList">The list of related entities to load.</param>
        /// <param name="currentPageNumber">The page number to return.</param>
        /// <param name="pageSize">The page size (the maximum number of elements to return in the list).</param>
        /// <returns></returns>
        public IPagedList<T> Search(Expression<Func<T, bool>> filters, string sorting, List<string> includeList, int currentPageNumber, int pageSize)
        {
            return GetQuery(filters, sorting, includeList).ToPagedList(currentPageNumber, pageSize);
        }

        /// <summary>
        /// Return a list.
        /// </summary>
        /// <param name="filters">A lambda expression used to filter the result.</param>
        /// <param name="sorting">The sort expression, example: ColumnName desc</param>
        /// <param name="includeList">The list of related entities to load.</param>
        /// <returns></returns>
        public IList<T> Search(Expression<Func<T, bool>> filters, string sorting, List<string> includeList)
        {
            return GetQuery(filters, sorting, includeList).ToList();
        }

        /// <summary>
        /// Retrieve an entity from the repository based on the unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns></returns>
        public T GetById(object id)
        {
            var entityKey = new EntityKey(EntitySetName, PrimaryKeyName, id);
            object entity;
            if (Context.TryGetObjectByKey(entityKey, out entity))
                return (T)entity;
            
            return null;
        }

        /// <summary>
        /// Add a new item to the repository.
        /// </summary>
        /// <param name="entity">The element to add to the repository.</param>
        /// <returns></returns>
        public void Insert(T entity)
        {      
            ObjectSet.AddObject(entity);
        }

        /// <summary>
        /// Updates an item in the repository.
        /// </summary>
        /// <param name="entity">The element to update.</param>
        /// <returns></returns>
        public void Update(T entity)
        {
            if (!IsAttached(entity))
                Context.AttachTo(EntitySetName, entity);

            Context.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
        }

        /// <summary>
        /// Deletes an item from the repository.
        /// </summary>
        /// <param name="entity">The item to be deleted.</param>
        /// <returns></returns>
        public void Delete(T entity)
        {
            if (!IsAttached(entity))
                Context.AttachTo(EntitySetName, entity);

            Context.ObjectStateManager.ChangeObjectState(entity, EntityState.Deleted);
        }

        /// <summary>
        /// Deletes an item from the repository.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to be deleted.</param>
        /// <returns></returns>
        public void Delete(object id)
        {
            var entity = GetById(id);
            if(entity == null)
                throw new Exception("Entity not found.");
            Delete(entity);
        }

        /// <summary>
        /// Method used to build the query that will reflect the filter conditions and the sort expression.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="sorting"></param>
        /// <param name="includeList"></param>
        /// <returns></returns>
        protected IQueryable<T> GetQuery(Expression<Func<T, bool>> filters, string sorting, List<string> includeList)
        {            
            if (string.IsNullOrEmpty(sorting))
                sorting = PrimaryKeyName; //consider the PK as the default sort column

            var objectQuery = (ObjectQuery<T>)ObjectSet;
            if (includeList != null)
            {
                foreach (var include in includeList)
                {
                    objectQuery = objectQuery.Include(include);
                }
            }

            var query = objectQuery.Where(filters == null ? t => 1 == 1 : filters);
            query = query.OrderBy(sorting);
            return query;
        }

        /// <summary>
        /// Returns true if the entity is attached to the current context.
        /// </summary>
        /// <param name="entity"></param>
        protected bool IsAttached( T entity)
        {
            ObjectStateEntry entry;
            Context.ObjectStateManager.TryGetObjectStateEntry(Context.CreateEntityKey(EntitySetName, entity), out entry);

            return !(entry == null || entry.State == EntityState.Detached);
        }
    }
}