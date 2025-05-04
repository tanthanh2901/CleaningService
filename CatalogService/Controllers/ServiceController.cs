using AutoMapper;
using CatalogService.Dtos;
using CatalogService.Entities;
using CatalogService.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers
{
    [Route("api/services")]
    [ApiController]
    public class ServiceController : Controller
    {
        private readonly IServiceRepository serviceRepository;
        private readonly IMapper mapper;

        public ServiceController(IServiceRepository serviceRepository, IMapper mapper)
        {
            this.serviceRepository = serviceRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            var result = await serviceRepository.GetServices();
            return Ok(result);
        }

        [HttpPut("{serviceId}")]
        public async Task<ActionResult<int>> Update(int serviceId, ServiceForUpdate serviceForUpdate)
        {
            var serviceToUpdate = await serviceRepository.GetServiceById(serviceId);
            if (serviceToUpdate == null)
            {
                return NotFound();
            }

            mapper.Map(serviceForUpdate, serviceToUpdate, typeof(ServiceForUpdate), typeof(Service));

            await serviceRepository.UpdateService(serviceToUpdate);

            return Ok();
        }

        [HttpDelete("{serviceId}")]
        public async Task<ActionResult> Delete(int serviceId)
        {
            var serviceToDelete = await serviceRepository.GetServiceById(serviceId);
            if (serviceToDelete == null)
            {
                return NotFound();
            }

            await serviceRepository.DeleteService(serviceId);

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create(ServiceDto serviceDto)
        {
            var service = mapper.Map<Service>(serviceDto);

            await serviceRepository.AddService(service);

            return Created();
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Service>>> GetServicesByCategory(
           int categoryId)
        {
            var result = await serviceRepository.GetServicesByCategory(categoryId);
            return Ok(result);
        }

        [HttpGet("{serviceId}")]
        public async Task<ActionResult<Service>> GetById(int serviceId)
        {
            var result = await serviceRepository.GetServiceById(serviceId);
            return Ok(result);
        }  
    }
}
