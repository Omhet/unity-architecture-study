namespace App.Boot.Utility
{
    using System;
    using System.Collections.Generic;

    public static class ConfigValidationHelper
    {
        /// <summary>
        /// Validates that all items have unique, non-empty IDs and reports errors for missing or duplicate ones.
        /// </summary>
        public static void ValidateUniqueIds<T>(IEnumerable<T> items, Func<T, string> selector, string itemType, List<string> errors)
        {
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in items)
            {
                string id = selector(item);

                if (string.IsNullOrWhiteSpace(id))
                {
                    errors.Add("Found " + itemType + " with missing id.");
                    continue;
                }

                if (!ids.Add(id))
                {
                    errors.Add("Duplicate " + itemType + " id: " + id);
                }
            }
        }

        /// <summary>
        /// Builds a set of non-empty IDs from a collection for reference validation.
        /// </summary>
        public static HashSet<string> BuildIdSet<T>(IEnumerable<T> items, Func<T, string> selector)
        {
            var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (items == null)
            {
                return ids;
            }

            foreach (var item in items)
            {
                string id = selector(item);

                if (!string.IsNullOrWhiteSpace(id))
                {
                    ids.Add(id);
                }
            }

            return ids;
        }
    }
}
