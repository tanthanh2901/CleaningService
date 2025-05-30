using AutoMapper;
using CatalogService.AWSS3;
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
        private readonly IS3Service s3Service;
        private readonly IMapper mapper;

        public ServiceController(IServiceRepository serviceRepository, IMapper mapper, IS3Service s3Service)
        {
            this.serviceRepository = serviceRepository;
            this.mapper = mapper;
            this.s3Service = s3Service;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        //{
        //    var result = await serviceRepository.GetServices();
        //    return Ok(result);
        //}

        [HttpGet]
        public async Task<ActionResult<PaginationDto<ServiceDto>>> GetPaginatedServices(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 50) pageSize = 50; // Limit maximum page size

            var result = await serviceRepository.GetPaginatedServices(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPut("{serviceId}")]
        public async Task<ActionResult<int>> Update(int serviceId, ServiceDtoForUpdate serviceForUpdate)
        {
            await serviceRepository.UpdateService(serviceId, serviceForUpdate);

            return NoContent();
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
        public async Task<ActionResult<int>> Create([FromBody] ServiceDtoForCreate serviceDto)
        {
            //var imgUrl = await s3Service.UploadFileAsync(serviceDto.Image.OpenReadStream());

            var service = mapper.Map<Service>(serviceDto);
            //service.ImageUrl = imgUrl;

            await serviceRepository.AddService(service);

            return Created();
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult> GetServicesByCategory(
           int categoryId)
        {
            var result = await serviceRepository.GetServicesByCategory(categoryId);

            return Ok(result);
        }

        [HttpGet("{serviceId}")]
        public async Task<ActionResult<Service>> GetById(int serviceId)
        {
            var result = await serviceRepository.GetServiceById(serviceId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IReadOnlyList<ServiceDto>>> Search([FromQuery] string searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var services = await serviceRepository.SearchProduct(searchQuery, pageNumber, pageSize);

            if (services == null)
            {
                return NotFound("No products found matching the search criteria.");
            }

            return Ok(services);
        }
    }
}
