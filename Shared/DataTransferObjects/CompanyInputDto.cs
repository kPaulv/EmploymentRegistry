﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public record CompanyInputDto(string Name, string Address, string Country, 
        IEnumerable<EmployeeInputDto> EmployeesInput);
}