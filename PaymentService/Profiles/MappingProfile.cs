﻿using AutoMapper;
using MessageBus.IntegrationEvents;
using PaymentService.Dtos;

namespace PaymentService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<BookingCreatedEvent, BookingDto>().ReverseMap();
        }
    }
}
