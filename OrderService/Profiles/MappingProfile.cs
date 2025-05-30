using AutoMapper;
using MessageBus;
using OrderService.Dtos;
using OrderService.Entities;
using OrderService.Enums;

namespace OrderService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Booking, BookingDto>().ReverseMap();
            CreateMap<Service, ServiceDto>().ReverseMap();

            CreateMap<Enums.PaymentMethodType, MessageBus.PaymentMethodType>().ReverseMap();
        }
    }
}
