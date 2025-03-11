using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.AdminManagementDTO;
using HCP.Service.DTOs.CleaningServiceDTO;
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

        public async Task<List<ServiceCategoryAdminShowDTO>> GetAllServiceCategoriesAsync()
        {
            var categories = await _unitOfWork.Repository<ServiceCategory>().ListAllAsync();
            return categories.Select(category => new ServiceCategoryAdminShowDTO
            {
                CategoryName = category.CategoryName,
                CategoryId = category.Id,
                PictureUrl = category.PictureUrl,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                Description = category.Description
            }).ToList();
        }
    }
}
