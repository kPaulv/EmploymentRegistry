# EmploymentRegistry

## App for tracking employees of different companies

### Stack:
+ .NET 6
+ ASP.NET Core 
+ EF Core 6
+ MS SQL 19
+ Visual Studio 22

### Implemented features:
+ Patterns: Onion architecture, Repository pattern
+ DAL: new way of seeding: through Repository classes in OnModelCreating() of DbContext. Data's being inserted via applying EF Core migration (hence no Seeder class used)
+ DAL: passing boolean variable to Repository that indicates whether we want to use data in Read-Only mode(if true, context.SaveChanges() won't apply changes to DB), or want to modify it. this method improves query speed
+ DAL: Both interfaces for Generic Repository and specialized Repositories for Entities and abstract class of Generic Repository. We need both interfaces and abstract class for separating the logic that is common for all our Repository Entities and also specific for every Entity.
+ BLL: Services for CREATE, UPDATE, READ, DELETE
+ BLL: Searching by Key, Data Shaping, Sorting, Filtering (partly)
+ PL: Controllers in a separate project, injected in Program entry point into Services collection (Controllers as Services)

### TODO:
+ Sorting, Filtering, Key-Search and Data Shaping for all Entities
+ Rewrite Entity and Link models
