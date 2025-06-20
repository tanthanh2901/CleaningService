using AutoMapper;
using CatalogService.DbContexts;
using CatalogService.Dtos;
using CatalogService.Entities;
using CatalogService.Extensions;
using CatalogService.Interface;
using CatalogService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper mapper;
        private readonly CatalogDbContext dbContext;

        public ServiceService(
            IServiceRepository serviceRepository,
            IMapper mapper,
            CatalogDbContext dbContext)
        {
            _serviceRepository = serviceRepository;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<ServiceDto>> GetServicesAsync()
        {
            var services = await dbContext.Services
                .Include(s => s.Category)
                .Include(s => s.DurationConfigs)
                .Include(s => s.PremiumServices)
                .ToListAsync();
            return mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        public async Task<ServiceDto> GetServiceById(int serviceId)
        {
            var service = await dbContext.Services
                .Include(s => s.DurationConfigs)
                .Include(s => s.PremiumServices)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            return mapper.Map<ServiceDto>(service);
        }

        public async Task DeleteService(int serviceId)
        {
            var service = await dbContext.Services
                .Include(s => s.Category)
                .Include(s => s.DurationConfigs)
                .Include(s => s.PremiumServices)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            dbContext.DurationConfigs.RemoveRange(service.DurationConfigs);
            dbContext.PremiumServices.RemoveRange(service.PremiumServices);
            dbContext.Services.Remove(service);

            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int serviceId, ServiceDtoForUpdate dto)
        {
            using var transaction = await _serviceRepository.BeginTransactionAsync();

            try
            {
                var service = await dbContext.Services
                    .Include(s => s.DurationConfigs)
                    .Include(s => s.PremiumServices)
                    .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

                if (service == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                service.Name = dto.Name;
                service.Description = dto.Description;
                service.ImageUrl = dto.ImageUrl;
                service.TaskDetails = dto.TaskDetails;
                service.BasePrice = dto.BasePrice;
                service.PriceUnit = dto.PriceUnit;
                service.CategoryId = dto.CategoryId;

                var durationIdsInDto = dto.DurationConfigs
                    .Where(d => d.DurationConfigId > 0)
                    .Select(d => d.DurationConfigId)
                    .ToList();

                var toRemoveDuration = service.DurationConfigs
                    .Where(d => !durationIdsInDto.Contains(d.DurationConfigId))
                    .ToList();

                dbContext.DurationConfigs.RemoveRange(toRemoveDuration);

                foreach (var durationDto in dto.DurationConfigs)
                {
                    if (durationDto.DurationConfigId > 0)
                    {
                        // Update existing
                        var existing = service.DurationConfigs
                            .FirstOrDefault(d => d.DurationConfigId == durationDto.DurationConfigId);

                        if (existing != null)
                        {
                            existing.DurationHours = durationDto.DurationHours;
                            existing.MaxAreaSqm = durationDto.MaxAreaSqm;
                            existing.MaxRooms = durationDto.MaxRooms;
                            existing.PriceMultiplier = durationDto.PriceMultiplier;
                        }
                    }
                    else
                    {
                        service.DurationConfigs.Add(new DurationConfig
                        {
                            DurationHours = durationDto.DurationHours,
                            MaxAreaSqm = durationDto.MaxAreaSqm,
                            MaxRooms = durationDto.MaxRooms,
                            PriceMultiplier = durationDto.PriceMultiplier,
                            ServiceId = service.ServiceId
                        });
                    }
                }

                var premiumIdsInDto = dto.PremiumServices
                    .Where(p => p.PremiumServiceId > 0)
                    .Select(p => p.PremiumServiceId)
                    .ToList();

                var toRemovePremium = service.PremiumServices
                    .Where(p => !premiumIdsInDto.Contains(p.PremiumServiceId))
                    .ToList();

                dbContext.PremiumServices.RemoveRange(toRemovePremium);

                foreach (var premiumDto in dto.PremiumServices)
                {
                    if (premiumDto.PremiumServiceId > 0)
                    {
                        var existing = service.PremiumServices
                            .FirstOrDefault(p => p.PremiumServiceId == premiumDto.PremiumServiceId);

                        if (existing != null)
                        {
                            existing.Name = premiumDto.Name;
                            existing.AdditionalPrice = premiumDto.AdditionalPrice;
                            existing.IsPercentage = premiumDto.IsPercentage;
                        }
                    }
                    else
                    {
                        service.PremiumServices.Add(new PremiumService
                        {
                            Name = premiumDto.Name,
                            AdditionalPrice = premiumDto.AdditionalPrice,
                            IsPercentage = premiumDto.IsPercentage,
                            ServiceId = service.ServiceId
                        });
                    }
                }

                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw; // Re-throw to let the controller handle it
            }
        }
        public async Task<IEnumerable<ServiceDto>> GetServicesByCategory(int categoryId)
        {
            var services = await dbContext.Services
                .Include(x => x.Category)
                .Where(x => (x.CategoryId == categoryId))
                .ToListAsync();

            return mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        public async Task<PaginationDto<ServiceDto>> GetPaginatedServices(int pageNumber, int pageSize)
        {
            var query = dbContext.Services
                .Include(s => s.DurationConfigs)
                .Include(s => s.PremiumServices)
                .OrderBy(s => s.ServiceId)
                .AsQueryable();

            var result = await query.ToPaginatedListAsync(pageNumber, pageSize);

            return mapper.Map<PaginationDto<ServiceDto>>(result);
        }

        public async Task<PaginationDto<ServiceDto>> SearchProduct(string searchQuery, int pageNumber, int pageSize)
        {
            IQueryable<Service> query = dbContext.Set<Service>();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchQuery) ||
                    p.Description.Contains(searchQuery));
            }

            var result = await query.ToPaginatedListAsync(pageNumber, pageSize);

            return mapper.Map<PaginationDto<ServiceDto>>(result);
        }

        public async Task<ServiceDtoForCreate> AddService(ServiceDtoForCreate service)
        {
            var serviceToAdd = new Service
            {
                Name = service.Name,
                ImageUrl = service.ImageUrl,
                Description = service.Description,
                TaskDetails = service.TaskDetails,
                BasePrice = service.BasePrice,
                PriceUnit = service.PriceUnit,
                CategoryId = service.CategoryId,
                DurationConfigs = service.DurationConfigs.Select(d => new DurationConfig
                {
                    DurationHours = d.DurationHours,
                    MaxAreaSqm = d.MaxAreaSqm,
                    MaxRooms = d.MaxRooms,
                    PriceMultiplier = d.PriceMultiplier
                }).ToList(),
                PremiumServices = service.PremiumServices.Select(p => new PremiumService
                {
                    Name = p.Name,
                    AdditionalPrice = p.AdditionalPrice,
                    IsPercentage = p.IsPercentage
                }).ToList()
            };

            await dbContext.Services.AddAsync(serviceToAdd);
            await dbContext.SaveChangesAsync();

            return service;
        }
    }
}

