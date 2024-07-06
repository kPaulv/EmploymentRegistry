using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions.BadRequest
{
    public class AgeRangeBadRequestException : BadRequestException
    {
        public AgeRangeBadRequestException() : 
            base("Age range is not valid. Max Age can't be less than Min Age.")
        {

        }
    }
}
