using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.AdminManagementDTO;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.Services.ListService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceAdminDTO;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceCategoryAdminDTO;

namespace HCP.Service.Services.AdminManService
{
    public class AdminManServiceCategory : IAdminManServiceCategory
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminManServiceCategory(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceCategoryAdminShowListDTO> GetAllServiceCategoriesAsync(int? pageIndex, int? pageSize)
        {
            var categories = _unitOfWork.Repository<ServiceCategory>().GetAll();
            var categorieList = categories.Select(category => new ServiceCategoryAdminShowDTO
            {
                CategoryName = category.CategoryName,
                CategoryId = category.Id,
                PictureUrl = category.PictureUrl,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                Description = category.Description
            });
            if (pageIndex == null || pageSize == null)
            {
                var temp1 = await PaginatedList<ServiceCategoryAdminShowDTO>.CreateAsync(categorieList, 1, categorieList.Count());
                return new ServiceCategoryAdminShowListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = categorieList.Count(),
                    totalPages = temp1.TotalPages,
                };
            }
            var temp2 = await PaginatedList<ServiceCategoryAdminShowDTO>.CreateAsync(categorieList, (int)pageIndex, (int)pageSize);
            return new ServiceCategoryAdminShowListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = categorieList.Count(),
                totalPages = temp2.TotalPages,
            };
        }
    }
}
