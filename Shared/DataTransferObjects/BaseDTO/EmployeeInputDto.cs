using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.BaseDTO
{
    public abstract record EmployeeInputDto
    {

        [Required(ErrorMessage = "Name is a required field.")]
        [MaxLength(30, ErrorMessage = "Name max length is 30 symbols.")]
        public string? Name { get; init; }
        [Range(18, int.MaxValue, ErrorMessage = "Age is a required field and must be 18+.")]
        public int Age { get; init; }
        [Required(ErrorMessage = "Position is a required field.")]
        [MaxLength(20, ErrorMessage = "Position max length is 20 symbols.")]
        public string? Position { get; init; }
    }
}
