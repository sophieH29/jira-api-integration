using System.ComponentModel;
using System.Web.Mvc;
using System.Web.Routing;
using HelloWorld.Models.BaseListViewModel;

namespace HelloWorld.Code.Util
{
    public static class UrlHelperExtentions
    {
        public static RouteValueDictionary GetRouteValueDictionaryForList(this UrlHelper urlHelper, IListViewModel model, string sortColumn = "", string direction = "")
        {
            var dictionary = new RouteValueDictionary
                                 {
                                     {"page", model.CurrentPageNumber},
                                     {"sort", string.IsNullOrEmpty(sortColumn) ? model.SortColumn : sortColumn},
                                     {"direction", string.IsNullOrEmpty(direction) ? (model.SortDirection == ListSortDirection.Ascending ? "asc" : "desc") : direction}                                    
                                 };
            foreach (var filter in model.GetFilters())
            {
                dictionary.Add(filter.UrlFilterName, filter.Value);
            }

            return dictionary;
        }
    }
}