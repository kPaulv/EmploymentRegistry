namespace Entities.Exceptions.NotFound
{
    public sealed class CompanyNotFoundException : NotFoundException
    {
        public CompanyNotFoundException(Guid companyId) :
            base($"The company with id: {companyId} not found in registry.")
        { }
    }
}
