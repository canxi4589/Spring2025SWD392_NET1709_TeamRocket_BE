using HCP.Repository.Entities;
using HCP.Service.DTOs;

namespace HCP.Service.Services.HomeCleaningService
{
    public interface ICleaningService
    {
        Task<List<HomeServiceDto>> GetAllServiceWithoutCategoryAsync();
    }
}