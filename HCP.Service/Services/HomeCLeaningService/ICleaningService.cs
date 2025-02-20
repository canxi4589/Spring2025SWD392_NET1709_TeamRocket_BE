using HCP.Repository.Entities;
using HCP.Service.DTOs;

namespace HCP.Service.Services.HomeCleaningService
{
    public interface ICleaningService
    {
        Task<List<HomeServiceDto>> GetAllServiceWithoutCategoryAsync();
        Task<List<ServicePricingDto>> GetServicePricesForCustomer(Guid serviceId);
        Task<ServiceDetailDto> GetServiceDetails(Guid serviceId);
    }
}