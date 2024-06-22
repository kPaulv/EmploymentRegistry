﻿using AutoMapper;
using Entities.Entities;
using Shared.DataTransferObjects;

namespace EmploymentRegistry.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Company, CompanyOutputDto>()
                .ForMember("FullAddress",
                               opt => opt.MapFrom(c => string.Join(' ', 
                                                                    c.Address, 
                                                                    c.Country)));

            CreateMap<Employee, EmployeeOutputDto>();

            CreateMap<CompanyCreateDto, Company>();

            CreateMap<EmployeeCreateDto, Employee>();

            CreateMap<EmployeeUpdateDto, Employee>().ReverseMap();

            CreateMap<CompanyUpdateDto, Company>();
        }
    }
}
