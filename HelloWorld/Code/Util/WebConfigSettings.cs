using System;
using System.Configuration;

namespace HelloWorld.Code.Util
{
    /// <summary>
    /// Get the settings from the web.config
    /// </summary>
    public static class WebConfigSettings
    {
        /// <summary>
        /// Get the number of seconds cached objects will be kept in cache
        /// Default is 300.
        /// </summary>
        public static int CachingTime
        {
            get
            {
                var value = ConfigurationManager.AppSettings["CachingTime"];
                int intValue;
                if (string.IsNullOrEmpty(value) || !Int32.TryParse(value, out intValue))
                    return 300;

                return intValue;
            }
        }
    }
}