using AutoMapper;
using CatalogService.Dtos;
using CatalogService.Entities;

namespace CatalogService.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Service, ServiceDto>().ReverseMap();
            CreateMap<Service, ServiceForUpdate>().ReverseMap();

            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CategoryForUpdate>().ReverseMap();


        }
    }
}
