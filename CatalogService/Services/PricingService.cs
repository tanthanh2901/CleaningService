using CatalogService.Interface;
using CatalogService.Models;

namespace CatalogService.Services
{
    public class PricingService : IPricingService
    {
        private readonly IServiceManagementService _serviceManagementService;

        public PricingService(IServiceManagementService serviceManagementService)
        {
            _serviceManagementService = serviceManagementService;
        }

        public async Task<ServicePriceResultDto> CalculatePriceAsync(ServicePriceCalculationDto calculation)
        {
            var result = new ServicePriceResultDto();
            var breakdown = new List<PriceBreakdownItem>();

            var serviceType = await _serviceManagementService.GetServiceAsync(calculation.ServiceId);
            var durationConfig = await _serviceManagementService.GetDurationConfigAsync(calculation.DurationConfigId);

            if (serviceType == null || durationConfig == null)
                throw new ArgumentException("Invalid service type or duration config");

            result.BaseAmount = serviceType.BasePrice * durationConfig.PriceMultiplier;
            breakdown.Add(new PriceBreakdownItem
            {
                Name = $"{serviceType.Name} ({durationConfig.DurationHours}h)",
                Type = "base",
                Amount = result.BaseAmount,
                Description = $"Giá cơ bản: {serviceType.BasePrice:N0} VND x {durationConfig.PriceMultiplier}"
            });

            if (calculation.PremiumServiceIds.Any())
            {
                var premiumServices = await _serviceManagementService.GetPremiumServicesByIdsAsync(calculation.PremiumServiceIds);

                foreach (var premium in premiumServices)
                {
                    var amount = premium.IsPercentage
                        ? result.BaseAmount * (premium.AdditionalPrice / 100)
                        : premium.AdditionalPrice;

                    result.PremiumAmount += amount;
                    breakdown.Add(new PriceBreakdownItem
                    {
                        Name = premium.Name,
                        Type = "premium",
                        Amount = amount,
                    });
                }
            }

            result.TotalAmount = result.BaseAmount + result.PremiumAmount;
            result.Breakdown = breakdown;

            return result;
        }
    }
}
