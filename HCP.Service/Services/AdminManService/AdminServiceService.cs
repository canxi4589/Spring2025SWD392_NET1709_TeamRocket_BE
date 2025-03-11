using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceAdminDTO;

namespace HCP.Service.Services.AdminManService
{
    public class AdminServiceService : IAdminServiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminServiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ServiceAdminShowDTO>> GetAllServicesAsync()
        {
            var services = _unitOfWork.Repository<CleaningService>().GetAll();
            return services.Select(service => new ServiceAdminShowDTO
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                CategoryId = service.CategoryId,
                CategoryName = service.Category.CategoryName,
                Status = service.Status,
                Rating = service.Rating,
                RatingCount = service.RatingCount,
                Price = service.Price,
                City = service.City,
                District = service.District,
                AddressLine = service.AddressLine,
                PlaceId = service.PlaceId,
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt,
                Duration = service.Duration,
                HousekeeperId = service.UserId,
                HousekeeperName = service.User.FullName,
                StaffId = service.StaffId,
                StaffName = service.StaffId != null ? service.User.FullName : null,
                FirstImgLinkUrl = service.ServiceImages.FirstOrDefault().LinkUrl
            }).ToList();
        }
    }
}
