using AutoMapper;
using COROLA_RENTACAR.EntityLayer.Entities;
using CorolaDtoLayer.Dto.CostomerDto;
using CorolaDtoLayer.Dto.CostumerDto;

namespace COROLA_RENTACAR.BusinessLayer.Mapping
{
    public class GenericMapping : Profile
    {
        public GenericMapping()
        {
            CreateMap<Customer, ResultCustomerDto>().ReverseMap();
            CreateMap<Customer, CreateCustomerDto>().ReverseMap();
            CreateMap<Customer, UpdateCustomerDto>().ReverseMap();
            CreateMap<Customer, GetCustomerByIdDto>().ReverseMap();
        }
    }
}