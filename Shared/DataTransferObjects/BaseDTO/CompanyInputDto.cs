using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.BaseDTO
{
    public abstract record CompanyInputDto
    {
        [Required(ErrorMessage = "Name is a required field.")]
        [MaxLength(30, ErrorMessage = "Name max length is 60 symbols.")]
        public string? Name { get; init; }

        [Required(ErrorMessage = "Address is a required field.")]
        [MaxLength(30, ErrorMessage = "Name max length is 60 symbols.")]
        public string? Address { get; init; }

        [Required(ErrorMessage = "Country is a required field.")]
        [MaxLength(30, ErrorMessage = "Name max length is 60 symbols.")]
        public string? Country { get; init; }
        public IEnumerable<EmployeeCreateDto>? EmployeesInput { get; init; }
    }
}
