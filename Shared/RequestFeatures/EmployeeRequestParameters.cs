using Shared.RequestFeatures.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestFeatures
{
    public class EmployeeRequestParameters : RequestParameters
    {
        public EmployeeRequestParameters() => OrderBy = "name"; // default sorting field

        public uint MinAge { get; set; }
        public uint MaxAge { get; set; } = int.MaxValue;

        public bool IsAgeRangeValid => MaxAge > MinAge;

        public string? SearchTerm { get; set; }
    }
}
