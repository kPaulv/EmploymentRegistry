using Entities.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;

        public RootController(LinkGenerator linkGenerator) => _linkGenerator = linkGenerator;

        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")] string headerMediaType)
        {
            if (headerMediaType.Contains("application/vnd.kpaulv.apiroot"))
            {
                var apiLinksList = new List<Link>() 
                {
                    new Link()
                    {
                        Href = _linkGenerator.GetUriByName(HttpContext, nameof(GetRoot), new { }),
                        Rel = "self",
                        Method = "GET"
                    },
                    new Link()
                    {
                        Href = _linkGenerator.GetUriByName(HttpContext, "CompanyById", new { }),
                        Rel = "company",
                        Method = "GET"
                    },
                    new Link()
                    {
                        Href = _linkGenerator.GetUriByName(HttpContext, "CompanyCollection", new { }),
                        Rel = "companies",
                        Method = "GET"
                    }
                };
                return Ok(apiLinksList);
            }
            return NoContent();
        }
    }
}
