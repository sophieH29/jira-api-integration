namespace HelloWorld.Models.BaseListViewModel
{
    /// <summary>
    /// Attribute used to mark a property as being used as a filter in the context of a ListViewModel&lt;/T&gt;.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ListViewFilterAttribute : System.Attribute
    {
        /// <summary>
        /// The name of the fitler when included in the URL.
        /// </summary>
        public string UlrParameterName { get; set; }

        /// <summary>
        /// Attribute used to mark a property as being used as a filter in the context of a ListViewModel&lt;/T&gt;.
        /// </summary>
        /// <param name="ulrParameterName">The name of the fitler when included in the URL.</param>
        public ListViewFilterAttribute(string ulrParameterName)
        {
            UlrParameterName = ulrParameterName;
        }
    }
}