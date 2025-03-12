using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using HCP.Service.Services.ListService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static HCP.Service.DTOs.RatingDTO.RatingDTO;

namespace HCP.Service.Services.RatingService
{
    public class RatingService : IRatingService
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
            var service = await _unitOfWork.Repository<CleaningService>()
                .FindAsync(s => s.Id == request.CleaningServiceId);
            var booking = await _unitOfWork.Repository<Booking>()
                .FindAsync(s => s.Id == request.BookingId);
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
                Review = request.Review,
                Status = RatingStatus.Active.ToString(),
                CleaningService = service,
                User = user,
                RatingDate = DateTime.Now
            };

            await _unitOfWork.Repository<ServiceRating>().AddAsync(newRating);
            await _unitOfWork.SaveChangesAsync();

            if (service != null && booking != null)
            {
                var allRatings = await _unitOfWork.Repository<ServiceRating>()
                    .GetAll()
                    .Where(r => r.CleaningServiceId == request.CleaningServiceId)
                    .ToListAsync();

                service.Rating = allRatings.Any() ? allRatings.Average(r => r.Rating) : 0;
                service.RatingCount = allRatings.Count;

                booking.isRating = true;
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
                CustomerAvatar = user.Avatar,
                RatingDate = newRating.RatingDate,
                BookingId = booking.Id,
                ServiceName = service.ServiceName
            };
        }

        public async Task<PagingRatingResponseListDTO> GetRatingsByCustomer(ClaimsPrincipal user, int? pageIndex, int? pageSize)
        {
            var userId = user.FindFirst("id")?.Value;
            var customer = await _userManager.FindByIdAsync(userId);

            if (customer == null)
            {
                return new PagingRatingResponseListDTO();
            }

            var ratings = _unitOfWork.Repository<ServiceRating>()
                .GetAll()
                .Where(r => r.UserId == userId)
                .Select(r => new RatingResponseListDTO
                {
                    Rating = r.Rating,
                    Review = r.Review,
                    CustomerName = customer.FullName,
                    CustomerAvatar = customer.Avatar,
                    RatingDate = DateTime.Now,
                    ServiceName = r.CleaningService.ServiceName
                });

            if (pageIndex == null || pageSize == null)
            {
                var temp1 = await PaginatedList<RatingResponseListDTO>.CreateAsync(ratings, 1, ratings.Count());
                return new PagingRatingResponseListDTO
                {
                    Items = temp1,
                    HasNext = temp1.HasNextPage,
                    HasPrevious = temp1.HasPreviousPage,
                    TotalCount = temp1.TotalCount,
                    TotalPages = temp1.TotalPages
                };
            }
            var temp2 = await PaginatedList<RatingResponseListDTO>.CreateAsync(ratings, (int)pageIndex, (int)pageSize);
            return new PagingRatingResponseListDTO
            {
                Items = temp2,
                HasNext = temp2.HasNextPage,
                HasPrevious = temp2.HasPreviousPage,
                TotalCount = ratings.Count(),
                TotalPages = temp2.TotalPages,
            };
        }

        public async Task<PagingRatingResponseListDTO> GetRatingsByService(Guid serviceId, int? pageIndex, int? pageSize)
        {
            var ratings = _unitOfWork.Repository<ServiceRating>()
                .GetAll()
                .Where(r => r.CleaningServiceId == serviceId)
                .Include(r => r.User)
                .Select(r => new RatingResponseListDTO
                {
                    Rating = r.Rating,
                    Review = r.Review,
                    CustomerName = r.User.FullName,
                    CustomerAvatar = r.User.Avatar,
                    RatingDate = DateTime.Now,
                    ServiceName = r.CleaningService.ServiceName
                });

            var allRatings = await _unitOfWork.Repository<ServiceRating>()
                    .GetAll()
                    .Where(r => r.CleaningServiceId == serviceId)
                    .ToListAsync();

            if (pageIndex == null || pageSize == null)
            {
                var temp1 = await PaginatedList<RatingResponseListDTO>.CreateAsync(ratings, 1, ratings.Count());
                return new PagingRatingResponseListDTO
                {
                    Items = temp1,
                    HasNext = temp1.HasNextPage,
                    HasPrevious = temp1.HasPreviousPage,
                    TotalCount = temp1.TotalCount,
                    TotalPages = temp1.TotalPages,
                    RatingAvg = allRatings.Any() ? allRatings.Average(r => r.Rating) : 0
                };
            }
            var temp2 = await PaginatedList<RatingResponseListDTO>.CreateAsync(ratings, (int)pageIndex, (int)pageSize);
            return new PagingRatingResponseListDTO
            {
                Items = temp2,
                HasNext = temp2.HasNextPage,
                HasPrevious = temp2.HasPreviousPage,
                TotalCount = ratings.Count(),
                TotalPages = temp2.TotalPages,
                RatingAvg = allRatings.Any() ? allRatings.Average(r => r.Rating) : 0
            };
        }

        public async Task<PagingRatingResponseListDTO> SortRatings(Guid serviceId, decimal rate, int? pageIndex, int? pageSize)
        {
            var ratings = _unitOfWork.Repository<ServiceRating>()
                .GetAll()
                .Where(r => r.CleaningServiceId == serviceId && r.Rating == rate)
                .Include(r => r.User)
                .Select(r => new RatingResponseListDTO
                {
                    Rating = r.Rating,
                    Review = r.Review,
                    CustomerName = r.User.FullName,
                    CustomerAvatar = r.User.Avatar,
                    RatingDate = DateTime.Now,
                    ServiceName = r.CleaningService.ServiceName
                });

            var allRatings = await _unitOfWork.Repository<ServiceRating>()
                    .GetAll()
                    .Where(r => r.CleaningServiceId == serviceId)
                    .ToListAsync();

            if (pageIndex == null || pageSize == null)
            {
                var temp1 = await PaginatedList<RatingResponseListDTO>.CreateAsync(ratings, 1, ratings.Count());
                return new PagingRatingResponseListDTO
                {
                    Items = temp1,
                    HasNext = temp1.HasNextPage,
                    HasPrevious = temp1.HasPreviousPage,
                    TotalCount = temp1.TotalCount,
                    TotalPages = temp1.TotalPages,
                    RatingAvg = allRatings.Any() ? allRatings.Average(r => r.Rating) : 0
                };
            }
            var temp2 = await PaginatedList<RatingResponseListDTO>.CreateAsync(ratings, (int)pageIndex, (int)pageSize);
            return new PagingRatingResponseListDTO
            {
                Items = temp2,
                HasNext = temp2.HasNextPage,
                HasPrevious = temp2.HasPreviousPage,
                TotalCount = ratings.Count(),
                TotalPages = temp2.TotalPages,
                RatingAvg = allRatings.Any() ? allRatings.Average(r => r.Rating) : 0
            };
        }

    }
}
