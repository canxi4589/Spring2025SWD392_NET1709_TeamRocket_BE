using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.Services.ListService;
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
                location = c.AddressLine + ", " + c.Province+", " + c.City,
                price = c.Price,
                
            }).ToList();
        }
        public async Task<CleaningServiceListDTO> GetAllServiceItems(int? pageIndex, int? pageSize)
        {
            var list = _unitOfWork.Repository<CleaningService>().GetAll().Include(c => c.Category);
            var list1 = list.Select(c => new CleaningServiceItemDTO
            {
                id = c.Id,
                name = c.ServiceName,
                category = c.Category.CategoryName,
                overallRating = c.Rating,
                price = c.Price,
                location = c.AddressLine + ", " + c.Province + ", " + c.City
            });
            if(pageIndex == null || pageSize == null)
            {
                var temp1 = await PaginatedList<CleaningServiceItemDTO>.CreateAsync(list1, 1, list1.Count());
                return new CleaningServiceListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = temp1.TotalCount,
                    totalPages = temp1.TotalPages,
                };
            }
            var temp =await PaginatedList<CleaningServiceItemDTO>.CreateAsync(list1, (int)pageIndex, (int)pageSize);
            var temp2 = new CleaningServiceListDTO
            {
                Items = temp,
                hasNext = temp.HasNextPage,
                hasPrevious = temp.HasPreviousPage,
                totalCount = list1.Count(),
                totalPages = temp.TotalPages,
            };
            return temp2;
        }
        public async Task<PaginatedList<CategoryDTO>> GetAllCategories(int pageIndex, int pageSize)
        {
            var categories = _unitOfWork.Repository<ServiceCategory>().GetAll();

            var categoryList = categories.Select(c => new CategoryDTO
            {
                id = c.Id,
                name = c.CategoryName,
                description = "hehe",
                imgUrl = c.PictureUrl
            });

            return await PaginatedList<CategoryDTO>.CreateAsync(categoryList, pageIndex, pageSize);
        }
    }
}
