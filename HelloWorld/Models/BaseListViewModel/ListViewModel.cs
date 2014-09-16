using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using HelloWorld.Code.DataAccess;
using HelloWorld.Code.DataAccess.Paging;

namespace HelloWorld.Models.BaseListViewModel
{
    /// <summary>
    /// Generic class used to show a list of domain entities and allow for easy paging, sorting and filtering.
    /// </summary>
    public class ListViewModel<T> : IListViewModel where T : class
    {
        #region Properties

        /// <summary>
        /// The number of elements to display in the list. Default is 10.
        /// </summary>
        public int PageSize
        {
            get { return _pageSize < 1 ? 10 : _pageSize; }
            set { _pageSize = value; }
        }
        private int _pageSize;

        /// <summary>
        /// The current page to be displayed. Default is 1.
        /// </summary>
        public int CurrentPageNumber
        {
            get { return _currentPageNumber < 1 ? 1 : _currentPageNumber; }
            set { _currentPageNumber = value; }
        }
        private int _currentPageNumber;

        /// <summary>
        /// The column on which the sorting is applied.
        /// </summary>
        public string SortColumn { get; set; }

        /// <summary>
        /// The direction of the sorting. Default is ascending.
        /// </summary>
        public ListSortDirection SortDirection
        {
            get { return _sortingDirection; }
            set { _sortingDirection = value; }
        }
        private ListSortDirection _sortingDirection = ListSortDirection.Ascending;

        /// <summary>
        /// Get the current sort expression.
        /// </summary>
        public string SortExpression
        {
            get
            {
                return string.IsNullOrEmpty(SortColumn) ? string.Empty
                           : string.Format("{0} {1}", SortColumn, SortDirection == ListSortDirection.Ascending ? "asc" : "desc");
            }
        }

        /// <summary>
        /// Used to specify the related objects to include in the result.
        /// </summary>
        public List<string> IncludeList { get; private set; }

        /// <summary>
        /// A lambda expression used to filter the list
        /// </summary>
        /// <returns></returns>
        public virtual Expression<Func<T, bool>> FilterCondition { get; set; }

        /// <summary>
        /// The repository used to obtain the data
        /// </summary>
        public IRepository<T> Repository { get; set; } 

        #endregion

        #region Constructor

        public ListViewModel()
        {
            IncludeList = new List<string>();
        }

        #endregion

        #region Fill list & Paged list

        /// <summary>
        /// Returns the elements for the current page. The list is filtered and sorted.
        /// </summary>
        /// <param name="refresh">The refresh parameter is used to force the data retrieval in care the list was already filled.</param>
        /// <returns></returns>
        protected IPagedList<T> GetPagedList(bool refresh)
        {
            if (refresh || _pagedList == null)                           
                 _pagedList = Repository.Search(FilterCondition, SortExpression, IncludeList, CurrentPageNumber, PageSize);                
            
            return _pagedList;
        }

        public IPagedList<T> PagedList
        {
            get { return GetPagedList(false); }
            set { _pagedList = value; }
        }
        private IPagedList<T> _pagedList;

        /// <summary>
        /// Returns all the elements that match the filter condition. The list is sorted.
        /// </summary>
        /// <param name="refresh">The refresh parameter is used to force the data retrieval in care the list was already filled.</param>
        /// <returns></returns>
        protected IList<T> GetList(bool refresh)
        {
            if (refresh || _list == null)            
                _list = Repository.Search(FilterCondition, SortExpression, IncludeList);            

            return List;
        }

        public IList<T> List
        {
            get { return GetList(false); }
            set { _list = value; }
        }
        private IList<T> _list;

        #endregion

        #region Public methods

        /// <summary>
        /// Helper methods used for setting the current page number, the column to sort on and the sorting direction.
        /// </summary>
        /// <param name="currentPageNumber">The current page to be displayed.</param>
        /// <param name="sortColumn">The column on which the sorting is applied.</param>
        /// <param name="sortDirection">The direction of the sorting. Default is ascending.</param>
        /// <param name="requestParameters">Used to populate the filters.</param>
        public void SetParameters(int currentPageNumber, string sortColumn, string sortDirection, NameValueCollection requestParameters)
        {
            CurrentPageNumber = currentPageNumber;
            if (!string.IsNullOrEmpty(sortColumn))
            {
                SortColumn = sortColumn;
                if (string.Compare(sortDirection, "desc", StringComparison.InvariantCultureIgnoreCase) == 0 || string.Compare(sortDirection, "descending", StringComparison.InvariantCultureIgnoreCase) == 0)
                    SortDirection = ListSortDirection.Descending;
            }

            //Populate the filter values
            if(requestParameters != null)
            {
                var filters = GetFilters();
                foreach (var filter in filters)
                {
                    filter.Value = requestParameters[filter.UrlFilterName] ?? requestParameters[filter.PropertyName];
                }
                SetFilters(filters);
            }
        }

        /// <summary>
        /// Get a list of name and value for all the properties inside this class that are marked with the ListViewFilterAttribute.
        /// </summary>
        /// <returns></returns>
        public List<ListViewFilter> GetFilters()
        {
            return _filters ?? (_filters = (from p in GetType().GetProperties()
                                            let attr = p.GetCustomAttributes(typeof (ListViewFilterAttribute), true)
                                            where attr.Length == 1
                                            select new ListViewFilter
                                                       {
                                                           PropertyName = p.Name,
                                                           UrlFilterName =
                                                               ((ListViewFilterAttribute) (attr.First())).
                                                               UlrParameterName,
                                                           Value = p.GetValue(this, null)
                                                       }).ToList());
        }
        private List<ListViewFilter> _filters;
        
        /// <summary>
        /// Populate the model filter properties using the filter values.
        /// </summary>
        /// <param name="filters"></param>
        public void SetFilters(List<ListViewFilter> filters)
        {
            var type = GetType();
            foreach (var filter in filters)
            {
                var property = type.GetProperty(filter.PropertyName);
                if(property != null)
                    property.SetValue(this, filter.Value, null);
            }
        }

        #endregion
    }
}