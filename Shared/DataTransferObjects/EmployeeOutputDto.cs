using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public record EmployeeOutputDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? Age { get; init; }
        public string? Position { get; init; }
    }
}
