using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

namespace EmploymentRegistry.ContextFactory
{
    public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
    {
        public RepositoryContextFactory() { }

        public RepositoryContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("appsettings.json")
                                        .Build();
            var builder = new DbContextOptionsBuilder<RepositoryContext>()
                                        .UseSqlServer(configuration.GetConnectionString("EmploymentRegistryDb"), 
                                        b => b.MigrationsAssembly("EmploymentRegistry"));
            return new RepositoryContext(builder.Options);
        }
    }
}
