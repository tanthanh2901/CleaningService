using AutoMapper;
using Shared.Entities;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<AppUser, UserDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();
            CreateMap<UpdateUserInfoViewModel, AppUser>().ReverseMap();
        }
    }
}
