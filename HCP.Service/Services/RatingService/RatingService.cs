using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.GenericRepository;
using HCP.Repository.Interfaces;
using HCP.Service.Services.CustomerService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static HCP.Service.DTOs.RatingDTO.RatingDTO;

namespace HCP.Service.Services.RatingService
{
    public class RatingService : IServiceRatingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public RatingService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<CreatedRatingResponseDTO> CreateRating(CreateRatingRequestDTO request, ClaimsPrincipal customer)
        {
            var userId = customer.FindFirst("id")?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception(CustomerConst.NotFoundError);
            }

            var newRating = new ServiceRating
            {
                UserId = userId,
                CleaningServiceId = request.CleaningServiceId,
                Rating = request.Rating,
                Review = request.Review
            };

            await _unitOfWork.Repository<ServiceRating>().AddAsync(newRating);
            await _unitOfWork.SaveChangesAsync();

            var service = await _unitOfWork.Repository<CleaningService>()
                .FindAsync(s => s.Id == request.CleaningServiceId);

            if (service != null)
            {
                var allRatings = await _unitOfWork.Repository<ServiceRating>()
                    .GetAll()
                    .Where(r => r.CleaningServiceId == request.CleaningServiceId)
                    .ToListAsync();

                service.Rating = allRatings.Average(r => r.Rating);
                service.RatingCount = allRatings.Count;

                await _unitOfWork.SaveChangesAsync();
            }

            return new CreatedRatingResponseDTO
            {
                CustomerId = userId,
                RatingId = newRating.Id,
                CleaningServiceId = newRating.CleaningServiceId,
                Rating = newRating.Rating,
                Review = newRating.Review,
                CustomerName = user.FullName,
                CustomerAvatar = user.Avatar
            };
        }

        public async Task<RatingResponseDTO> GetRatingsByCustomer(string userId)
        {
            var customer = await _userManager.FindByIdAsync(userId);
            if (customer == null)
            {
                return new RatingResponseDTO { RatingCount = 0, Ratings = new List<RatingResponseListDTO>() };
            }

            var ratings = await _unitOfWork.Repository<ServiceRating>()
                .GetAll()
                .Where(r => r.UserId == userId)
                .Select(r => new RatingResponseListDTO
                {
                    Rating = r.Rating,
                    Review = r.Review,
                    CustomerName = customer.FullName,
                    CustomerAvatar = customer.Avatar
                })
                .ToListAsync();

            return new RatingResponseDTO
            {
                RatingCount = ratings.Count,
                Ratings = ratings
            };
        }

        public async Task<RatingResponseDTO> GetRatingsByService(Guid serviceId)
        {
            var ratings = await _unitOfWork.Repository<ServiceRating>()
                .GetAll()
                .Where(r => r.CleaningServiceId == serviceId)
                .Include(r => r.User)
                .Select(r => new RatingResponseListDTO
                {
                    Rating = r.Rating,
                    Review = r.Review,
                    CustomerName = r.User.FullName,
                    CustomerAvatar = r.User.Avatar
                })
                .ToListAsync();

            return new RatingResponseDTO
            {
                RatingCount = ratings.Count,
                Ratings = ratings
            };
        }

        public async Task<RatingResponseDTO> SortRatings(Guid serviceId, decimal minRating, decimal maxRating)
        {
            var ratings = await _unitOfWork.Repository<ServiceRating>()
                .GetAll()
                .Where(r => r.CleaningServiceId == serviceId && r.Rating >= minRating && r.Rating <= maxRating)
                .Include(r => r.User)
                .Select(r => new RatingResponseListDTO
                {
                    Rating = r.Rating,
                    Review = r.Review,
                    CustomerName = r.User.FullName,
                    CustomerAvatar = r.User.Avatar
                })
                .ToListAsync();

            return new RatingResponseDTO
            {
                RatingCount = ratings.Count,
                Ratings = ratings
            };
        }

    }
}
