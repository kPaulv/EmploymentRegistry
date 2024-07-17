using AutoMapper;
using Contracts.Interfaces;
using Entities.Entities;
using MediatorService.Commands;
using MediatorService.Queries;
using MediatR;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatorService.Handlers
{
    internal sealed class CreateCompanyHandler : 
        IRequestHandler<CreateCompanyCommand, CompanyOutputDto>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public CreateCompanyHandler(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CompanyOutputDto> Handle(CreateCompanyCommand command,
            CancellationToken cancellationToken)
        {
            var companyEntity = _mapper.Map<Company>(command.CompanyCreateDto);

            _repository.CompanyStorage.CreateCompany(companyEntity);

            await _repository.SaveAsync();

            var companyDto = _mapper.Map<CompanyOutputDto>(companyEntity);

            return companyDto;
        }
    }
}
