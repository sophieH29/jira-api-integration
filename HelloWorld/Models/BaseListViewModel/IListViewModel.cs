using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace HelloWorld.Models.BaseListViewModel
{
    /// <summary>
    /// Interface used for defining view models that contain a list of domain entities and allow easy paging, sorting and filtering.
    /// </summary>
    public interface IListViewModel
    {
        #region Properties

        /// <summary>
        /// The number of elements to display in the list. Default is 10.
        /// </summary>
        int PageSize { get; set; }

        /// <summary>
        /// The current page to be displayed. Default is 1.
        /// </summary>
        int CurrentPageNumber { get; set; }

        /// <summary>
        /// The column on which the sorting is applied.
        /// </summary>
        string SortColumn { get; set; }

        /// <summary>
        /// The direction of the sorting. Default is ascending.
        /// </summary>
        ListSortDirection SortDirection { get; set; }

        /// <summary>
        /// Get the current sort expression.
        /// </summary>
        string SortExpression { get; }

        /// <summary>
        /// Used to specify the related objects to include in the result.
        /// </summary>
        List<string> IncludeList { get; }

        #endregion

        /// <summary>
        /// Get a list of name and value for all the properties inside this class that are marked with the ListViewFilterAttribute.
        /// </summary>
        /// <returns></returns>
        List<ListViewFilter> GetFilters();

        /// <summary>
        /// Populate the model filter properties using the filter values.
        /// </summary>
        /// <param name="filters"></param>
        void SetFilters(List<ListViewFilter> filters);

        /// <summary>
        /// Helper methods used for setting the current page number, the column to sort on and the sorting direction.
        /// </summary>
        /// <param name="currentPageNumber">The current page to be displayed.</param>
        /// <param name="sortColumn">The column on which the sorting is applied.</param>
        /// <param name="sortDirection">The direction of the sorting. Default is ascending.</param>
        /// <param name="requestParameters">Used to populate the filters.</param>
        void SetParameters(int currentPageNumber, string sortColumn, string sortDirection, NameValueCollection requestParameters);
    }
}