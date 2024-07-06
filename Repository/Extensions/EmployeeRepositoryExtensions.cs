using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
