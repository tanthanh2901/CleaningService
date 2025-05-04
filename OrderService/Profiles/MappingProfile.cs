using AutoMapper;
using OrderService.Dtos;
using OrderService.Entities;

namespace OrderService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Service, ServiceDto>().ReverseMap();
        }
    }
}
