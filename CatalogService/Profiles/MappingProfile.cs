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

            CreateMap<PremiumService, CreatePremiumServiceDto>().ReverseMap();
            CreateMap<DurationConfig, CreateDurationConfigDto>().ReverseMap();
            CreateMap<PremiumService, PremiumServiceDto>().ReverseMap();
            CreateMap<DurationConfig, DurationConfigDto>().ReverseMap();
            CreateMap<ServiceDtoForCreate, Service>()
                .ForMember(dest => dest.PremiumServices, opt => opt.MapFrom(src => src.PremiumServices))
                .ForMember(dest => dest.DurationConfigs, opt => opt.MapFrom(src => src.DurationConfigs));

            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CategoryForCreateAndUpdate>().ReverseMap();

            CreateMap<PaginationDto<Service>, PaginationDto<ServiceDto>>().ReverseMap();
        }
    }
}
