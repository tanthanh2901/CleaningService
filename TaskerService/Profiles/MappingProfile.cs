using AutoMapper;
using TaskerService.Dtos;
using TaskerService.Entities;

namespace TaskerService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Tasker, TaskerDto>().ReverseMap();
            CreateMap<Tasker, TaskerWithCategoriesDto>().ReverseMap();
        }
    }
}
