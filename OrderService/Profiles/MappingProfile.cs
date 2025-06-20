using AutoMapper;
using MessageBus;
using OrderService.Dtos;
using OrderService.Entities;

namespace OrderService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Booking, BookingDto>().ReverseMap();
            CreateMap<Booking, BookingDetailsDto>().ReverseMap();
            CreateMap<Service, ServiceDto>().ReverseMap();

            //CreateMap<Enums.PaymentMethodType, MessageBus.PaymentMethodType>().ReverseMap();
        }
    }
}
