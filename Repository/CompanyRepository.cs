﻿using Contracts.Interfaces;
using Entities.Entities;

namespace Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }
    }
}