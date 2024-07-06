using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Extensions.Utility
{
    public static class SortQueryBuilder
    {
        public static string CreateSortQuery<T>(string sortQueryString)
        {
            // separate sort params (passed separated by ',')
            var sortParams = sortQueryString.Trim().Split(',');

            // array of all object's of type T properties
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public |
                                                                BindingFlags.Instance);

            // stringBuilder to build a string query for sorting
            var sortQueryBuilder = new StringBuilder();

            foreach (var sortParam in sortParams)
            {
                if (string.IsNullOrWhiteSpace(sortParam))
                    continue;

                // param = propName + ' ' + direction
                // get the sort property name from url
                var propName = sortParam.Split(' ')[0];
                // find a field in object of passed type T that must be sorted
                var objectProp =
                    propertyInfos.FirstOrDefault(pi =>
                        pi.Name.Equals(propName, StringComparison.InvariantCultureIgnoreCase));

                if (objectProp == null)
                    continue;

                // get the sort direction for each sort param
                var direction = sortParam.EndsWith(" desc") ? "descending" : "ascending";

                // append example: [need to sort by:] "Name ascending, Age descending, ...,"
                sortQueryBuilder.Append($"{objectProp.Name} {direction},");
            }

            return sortQueryBuilder.ToString().TrimEnd(',', ' ');
        }
    }
}
