using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions.Validation
{
    internal class ValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; set; }

        public ValidationException(IReadOnlyDictionary<string, string[]> errors) : 
            base("One or more errors occured during Validation.") => Errors = errors;
    }
}
