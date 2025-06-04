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
        private readonly IServiceOptionRepository _serviceOptionRepository;
        private readonly IMapper mapper;
        private readonly CatalogDbContext dbContext;

        public ServiceService(
            IServiceRepository serviceRepository,
            IServiceOptionRepository serviceOptionRepository,
            IMapper mapper,
            CatalogDbContext dbContext)
        {
            _serviceRepository = serviceRepository;
            _serviceOptionRepository = serviceOptionRepository;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }


        public async Task<ServiceDto> GetByIdWithtOptionsAsync(int id)
        {
            var service = await _serviceRepository.GetByIdWithOptionsAsync(id);
            if (service == null)
                throw new Exception($"Service with ID {id} not found");

            return mapper.Map<ServiceDto>(service);
        }

        public async Task<ServiceDto> GetServiceById(int serviceId)
        {
            var service = await dbContext.Services
                .Include(s => s.Category)
                .Include(s => s.Options)
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            return mapper.Map<ServiceDto>(service);
        }

        public async Task DeleteService(int serviceId)
        {
            var service = await dbContext.Services
                        .Include(s => s.Options)
                        .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            dbContext.ServiceOptions.RemoveRange(service.Options);
            dbContext.Services.Remove(service);

            await dbContext.SaveChangesAsync();
        }
        public async Task<ServiceDto> UpdateAsync(int id, ServiceDtoForUpdate serviceDto)
        {
            using var transaction = await _serviceRepository.BeginTransactionAsync();

            try
            {
                var existingService = await _serviceRepository.GetByIdAsync(id);
                if (existingService == null)
                    throw new Exception($"Service with ID {id} not found");

                var serviceToUpdate = mapper.Map<Service>(serviceDto);

                await _serviceRepository.UpdateAsync(serviceToUpdate);

                await UpdateServiceOptionsAsync(id, serviceDto.Options);

                await transaction.CommitAsync();

                return await GetByIdWithtOptionsAsync(id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task UpdateServiceOptionsAsync(int serviceId, List<ServiceOptionDtoForUpdate> optionDtos)
        {
            var existingOptions = await _serviceOptionRepository.GetByServiceIdAsync(serviceId);

            // Handle deleted options
            var deletedOptionIds = optionDtos
                .Where(o => o.IsDeleted && o.Id.HasValue)
                .Select(o => o.Id.Value)
                .ToList();

            if (deletedOptionIds.Any())
            {
                await _serviceOptionRepository.DeleteRangeAsync(deletedOptionIds);
            }

            var activeOptions = optionDtos.Where(o => !o.IsDeleted).ToList();

            foreach (var optionDto in activeOptions)
            {
                if (optionDto.Id.HasValue)
                {
                    // Update existing option
                    var existingOption = existingOptions.FirstOrDefault(o => o.Id == optionDto.Id.Value);
                    if (existingOption != null)
                    {
                        var optionToUpDate = mapper.Map<ServiceOption>(optionDto);

                        await _serviceOptionRepository.UpdateAsync(optionToUpDate);
                    }
                }
                else
                {
                    var newOption = new ServiceOption
                    {
                        ServiceId = serviceId,
                        OptionKey = optionDto.OptionKey,
                        OptionLabel = optionDto.OptionLabel,
                        DataType = optionDto.DataType,
                        DefaultValue = optionDto.DefaultValue,
                    };

                    await _serviceOptionRepository.CreateAsync(newOption);
                }
            }
        }

        public async Task<IEnumerable<ServiceDto>> GetServicesByCategory(int categoryId)
        {
            var services = await dbContext.Services
                .Include(x => x.Category)
                .Include(s => s.Options)
                .Where(x => (x.CategoryId == categoryId))
                .ToListAsync();

            return mapper.Map<IEnumerable<ServiceDto>>(services);
        }

        public async Task<PaginationDto<ServiceDto>> GetPaginatedServices(int pageNumber, int pageSize)
        {
            var query = dbContext.Services
                .Include(s => s.Category)
                .Include(s => s.Options)
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
            var serviceToAdd = mapper.Map<Service>(service);

            await _serviceRepository.AddService(serviceToAdd);

            return service;
        }
    }
}

