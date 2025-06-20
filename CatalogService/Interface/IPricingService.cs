using CatalogService.Models;

namespace CatalogService.Interface
{
    public interface IPricingService
    {
        Task<ServicePriceResultDto> CalculatePriceAsync(ServicePriceCalculationDto calculation);
    }
}
