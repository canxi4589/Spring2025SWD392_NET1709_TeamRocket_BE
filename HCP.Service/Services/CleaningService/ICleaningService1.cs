using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.Services.ListService;

namespace HCP.Service.Services.CleaningService1
{
    public interface ICleaningService1
    {
        Task<List<CategoryDTO>> GetAllCategories();
        Task<List<CleaningServiceItemDTO>> GetAllServiceItems();
        Task<PaginatedList<CategoryDTO>> GetAllCategories(int pageIndex, int pageSize);
        Task<CleaningServiceListDTO> GetAllServiceItems(int? pageIndex, int? pageSize);
    }
}