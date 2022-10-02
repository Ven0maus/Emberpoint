using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Emberpoint.Core.Resources
{
    /// <summary>
    /// A helper class that handles resource related tasks
    /// </summary>
    public sealed class ResourceHelper
    {
        private readonly Dictionary<string, Dictionary<string, string>> _resourceCache;

        public ResourceHelper()
        {
            _resourceCache = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            var type = typeof(Strings);
            var properties = type.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .Where(a => a.PropertyType == typeof(string));

            // Build a resource cache for each supported language
            foreach (var culture in Constants.SupportedCultures)
            {
                var cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                _resourceCache.Add(culture, cache);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);

                foreach (var property in properties)
                {
                    var value = (string)property.GetValue(null);
                    cache.Add(property.Name, value);

                    System.Diagnostics.Debug.WriteLine("["+culture+"] Added localization: " + property.Name + ": " + value);
                }
            }

            // Reset to default language
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Constants.Language);
        }

        /// <summary>
        /// Read's the given property of the Strings class for use in a non typed context.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string ReadProperty(string property, string defaultValue = null)
        {
            if (property == null) return defaultValue;
            if (_resourceCache[Constants.Language].TryGetValue(property, out string value))
                return value;
            return defaultValue;
        }
    }
}
