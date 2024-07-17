using Entities.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Repository.Extensions.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Extensions
{
    public static class EmployeeRepositoryExtensions
    {
        public static IQueryable<Employee> FilterByAge(this IQueryable<Employee> employees,
            uint minAge, uint maxAge) => employees.Where(e => (e.Age >= minAge &&
                                                                e.Age <= maxAge));

        public static IQueryable<Employee> Search(this IQueryable<Employee> employees,
            string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return employees;

            return employees.Where(e => e.Name.ToLower().Contains(searchTerm.Trim().ToLower()));
        }

        public static IQueryable<Employee> Sort(this IQueryable<Employee> employees, 
                                                                string sortQueryString)
        {
            // if no sort params passed - return full collection
            if(string.IsNullOrWhiteSpace(sortQueryString))
                return employees.OrderBy(e => e.Name);

            // here we get "Name ascending, Age descending"
            var sortQuery = SortQueryBuilder.CreateSortQuery<Employee>(sortQueryString);

            if (string.IsNullOrWhiteSpace(sortQuery))
                return employees.OrderBy(e => e.Name);

            return employees.OrderBy(sortQuery);
        }
    }
}
