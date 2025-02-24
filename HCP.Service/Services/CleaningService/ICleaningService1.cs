using HCP.Service.DTOs.CleaningServiceDTO;

namespace HCP.Service.Services.CleaningService1
{
    public interface ICleaningService1
    {
        Task<List<CategoryDTO>> GetAllCategories();
        Task<List<CleaningServiceItemDTO>> GetAllServiceItems();
    }
}