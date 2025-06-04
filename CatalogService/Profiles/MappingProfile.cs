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
            CreateMap<Service, ServiceDtoForUpdate>().ReverseMap();
            CreateMap<Service, ServiceDtoForCreate>().ReverseMap();

            CreateMap<ServiceOption, ServiceOptionDto>().ReverseMap();
            CreateMap<ServiceDtoForCreate, Service>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));

            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CategoryForCreateAndUpdate>().ReverseMap();

            CreateMap<PaginationDto<Service>, PaginationDto<ServiceDto>>().ReverseMap();
        }
    }
}
