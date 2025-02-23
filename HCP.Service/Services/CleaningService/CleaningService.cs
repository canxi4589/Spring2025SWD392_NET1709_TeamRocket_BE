using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.CleaningServiceDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.Services.CleaningService1
{
    public class CleaningService1 : ICleaningService1
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public CleaningService1(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<List<CategoryDTO>> GetAllCategories()
        {
            var categories = _unitOfWork.Repository<ServiceCategory>().GetAll();

            return categories.Select(c => new CategoryDTO
            {
                id = c.Id,
                name = c.CategoryName,
                description = "hehe",
                imgUrl = c.PictureUrl
            }).ToList();
        }
        public async Task<List<CleaningServiceItemDTO>> GetAllServiceItems()
        {
            var list = _unitOfWork.Repository<CleaningService>().GetAll().Include(c => c.Category);
            return list.Select(c => new CleaningServiceItemDTO
            {
                id = c.Id,
                name = c.ServiceName,
                category = c.Category.CategoryName,
                overallRating = c.Rating,
                location = c.AddressLine + "" + c.Province + c.City
            }).ToList();
        }
    }
}
