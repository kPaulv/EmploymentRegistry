using Contracts.Interfaces;
using Entities.Entities;
using Entities.LinkModels;
using Microsoft.Net.Http.Headers;
using Shared.DataTransferObjects;
using System.ComponentModel.Design;

namespace EmploymentRegistry.Utility
{
    public class EmployeeLinks : IEmployeeLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<EmployeeOutputDto> _dataShaper;

        public EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeOutputDto> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }

        public LinkResponse TryGenerateLinks(IEnumerable<EmployeeOutputDto> employeesDto, 
            Guid companyId, string fields, HttpContext httpContext)
        {
            var shapedEmployees = ShapeData(employeesDto, fields);

            if (NeedGenerateLinks(httpContext))
                return CreateShapedEmployeesWithLinks(employeesDto, shapedEmployees, 
                                                companyId, fields, httpContext);

            return CreateShapedEmployees(shapedEmployees);
        }

        private List<Entity> ShapeData(IEnumerable<EmployeeOutputDto> employeesDto,
            string fields) => _dataShaper.ShapeData(employeesDto, fields)
                                            .Select(e => e.Entity).ToList();

        private bool NeedGenerateLinks(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];

            var check = mediaType.SubTypeWithoutSuffix.EndsWith("hateoas",
                StringComparison.InvariantCultureIgnoreCase);

            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", 
                StringComparison.InvariantCultureIgnoreCase);
        }

        private LinkResponse CreateShapedEmployees(List<Entity> shapedEmployees) =>
            new LinkResponse { ShapedEntities = shapedEmployees };

        private LinkResponse CreateShapedEmployeesWithLinks
            (IEnumerable<EmployeeOutputDto> employeesDto, List<Entity> shapedEmployees,
                Guid companyId, string fields, HttpContext httpContext)
        {
            var employeesDtoList = employeesDto.ToList();

            for (int i = 0; i < employeesDtoList.Count(); i++)
            {
                // create links for eache employee using LinkGenerator
                var employeeLinks = CreateLinksForEmployee(httpContext, companyId,
                                                            employeesDtoList[i].Id, fields);
                shapedEmployees[i].Add("Links", employeeLinks);
            }

            // create links for employee collection
            var employeeCollectionWrapper = new LinkCollectionWrapper<Entity>(shapedEmployees);
            var linkedEmployeesCollection = CreateLinksForEmployees(httpContext, 
                                                                        employeeCollectionWrapper);

            return new LinkResponse { HasLinks = true, LinkedEntities = linkedEmployeesCollection };
        }

        private List<Link> CreateLinksForEmployee(HttpContext httpContext, 
            Guid companyId, Guid employeeId, string fields)
        {
            // here we generate href - URI to the resource for some action.
            // Link consists of this URI, relationship and HTTP method
            var GetEmployeesLink = _linkGenerator.GetUriByAction(httpContext, 
                                                                  "GetEmployeesForCompany", 
                                                                  values: new { companyId, employeeId, fields});
            var DeleteEmployeeLink = _linkGenerator.GetUriByAction(httpContext, 
                                                                    "DeleteEmployeeForCompany", 
                                                                    values: new { companyId, employeeId});
            var UpdateEmployeeLink = _linkGenerator.GetUriByAction(httpContext, 
                                                                    "UpdateEmployeeForCompany", 
                                                                    values: new { companyId, employeeId});
            var PatchEmployeeLink = _linkGenerator.GetUriByAction(httpContext,
                                                                   "PartiallyUpdateEmployeeForCompany",
                                                                   values: new { companyId, employeeId});

            return new List<Link>
            {
                // Link(string href, string rel, string method)
                new Link(GetEmployeesLink, "self", "GET"),
                new Link(DeleteEmployeeLink, "delete_employee", "DELETE"),
                new Link(UpdateEmployeeLink, "update_employee", "PUT"),
                new Link(PatchEmployeeLink, "partially_update_employee", "PATCH")
            };
        }

        private LinkCollectionWrapper<Entity> CreateLinksForEmployees(HttpContext httpContext, 
            LinkCollectionWrapper<Entity> employeeCollectionWrapper)
        {
            // generate href(URI) to all employees for company
            var GetEmployeesLink = _linkGenerator.GetUriByAction(httpContext,
                                                                  "GetEmployeesForCompany",
                                                                  values: new { });

            // add the Link(href, rel, method) to raw wrapper and return it
            employeeCollectionWrapper.Links.Add(new Link(GetEmployeesLink, "self", "GET"));

            return employeeCollectionWrapper;
        }

    }
}
