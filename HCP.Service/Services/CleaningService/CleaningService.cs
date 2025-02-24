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
        public async Task<ServiceDetailDTO> GetServiceById(Guid serviceId)
        {
            var services = await _unitOfWork.Repository<CleaningService>().ListAsync(
                filter: c => c.Id == serviceId,
                includeProperties: query => query
                    .Include(c => c.Category)
                    .Include(c => c.ServiceRatings)
                    .Include(c => c.ServiceImages)
                    .Include(c => c.ServiceSteps)
                    .Include(c => c.AdditionalServices)
                    .Include(c => c.User)
            );

            var service = services.FirstOrDefault();

            if (service == null)
                return null;

            return new ServiceDetailDTO
            {
                id = service.Id,
                name = service.ServiceName,
                numOfBooks = service.Bookings?.Count ?? 0,
                location = $"{service.City}, {service.Province}",
                reviews = service.ServiceRatings.Any() ? service.ServiceRatings.Average(r => r.Rating) : 0,
                numOfReviews = service.ServiceRatings.Count,
                numOfPics = service.ServiceImages.Count,
                overview = service.Description,
                images = service.ServiceImages.Select(i => new ImgDTO { url = i.LinkUrl }).ToList(),
                steps = service.ServiceSteps.Select(s => s.StepDescription).ToList(),
                additionalServices = service.AdditionalServices.Select(a => new AdditionalServicedDTO
                {
                    id = a.Id,
                    name = a.Name,
                    price = a.Amount.ToString("0.0"),
                }).ToList(),
                housekeeper = service.User != null
                    ? new housekeeperDetailDTO
                    {
                        id = service.User.Id,
                        name = service.User.FullName,
                        //review = service.User.ServiceRatings.Any() ?
                        //         service.User.ServiceRatings.Average(r => r.Rating).ToString("0.0") : "No reviews",
                        review = "No Reviews",
                        avatar = service.User.Avatar,
                        memberSince = /*service.User..ToString("yyyy-MM-dd")*/"2025-03-12",
                        address = /*$"{service.User.City}, {service.User.Province}"*/ "HCM,Phu Giao",
                        email = service.User.Email,
                        mobile = service.User.PhoneNumber,
                        numOfServices =1 /*service.User.HousekeeperServices.Count*/
                    }
                    : null
            };
        }

    }
}
