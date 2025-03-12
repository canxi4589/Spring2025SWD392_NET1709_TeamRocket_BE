using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.CheckoutDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HCP.Service.Services.CheckoutService
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public CheckoutService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<CheckoutResponseDTO1> CreateCheckout(CheckoutRequestDTO1 requestDTO, ClaimsPrincipal user)
        {
            try
            {
                var userId = user.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception(CustomerConst.NotFoundError);
                }

                var checkoutAddress = _unitOfWork.Repository<Address>().GetById(requestDTO.AddressId);
                var checkoutService = _unitOfWork.Repository<CleaningService>().GetById(requestDTO.ServiceId);
                var checkoutTimeSlot = _unitOfWork.Repository<ServiceTimeSlot>().GetById(requestDTO.ServiceTimeSlotId);

                if (checkoutService == null || checkoutAddress == null || checkoutTimeSlot == null)
                {
                    throw new Exception(CommonConst.NotFoundError);
                }

                var additionalPrice = 0.0;

                var checkout = new Checkout()
                {
                    AddressId = requestDTO.AddressId,
                    AddressLine = checkoutAddress.AddressLine1,
                    AdditionalPrice = 0,                                // Set initially to 0
                    City = checkoutAddress.City,
                    CleaningServiceId = requestDTO.ServiceId,
                    ServiceName = checkoutService.ServiceName,
                    BookingDate = requestDTO.BookingDate,
                    DayOfWeek = checkoutTimeSlot.DayOfWeek,
                    StartTime = checkoutTimeSlot.StartTime,
                    EndTime = checkoutTimeSlot.EndTime,
                    CustomerId = userId,
                    District = checkoutAddress.District,
                    ServicePrice = checkoutService.Price,
                    PlaceId = checkoutAddress.PlaceId,
                    Status = CheckoutStatus.Pending.ToString(),
                    Customer = await _userManager.FindByIdAsync(userId),
                    Note = string.Empty,
                    TimeSLotId = requestDTO.ServiceTimeSlotId,
                    DistancePrice = 0,                                   // Set initially to 0   
                    TotalPrice = 0,                                     // Set initially to 0                
                };

                await _unitOfWork.Repository<Checkout>().AddAsync(checkout);
                await _unitOfWork.SaveChangesAsync();

                var additionalServices = new List<CheckoutAdditionalService>();

                if (requestDTO.AdditionalServices != null)
                {
                    foreach (var additional in requestDTO.AdditionalServices)
                    {
                        var additionalServiceEntity = _unitOfWork.Repository<AdditionalService>()
                            .GetById(additional.AdditionalServiceId);

                        if (additionalServiceEntity != null)
                        {
                            var checkoutAdditionalService = new CheckoutAdditionalService()
                            {
                                AdditionalServiceId = additional.AdditionalServiceId,
                                AdditionalServiceName = additionalServiceEntity.Name,
                                Amount = (decimal)additionalServiceEntity.Amount,
                                IsActive = additionalServiceEntity.IsActive,
                                Url = additionalServiceEntity.Url,
                                Duration = additionalServiceEntity.Duration,
                                Description = additionalServiceEntity.Description,
                                CheckoutId = checkout.Id
                            };

                            additionalServices.Add(checkoutAdditionalService);
                            additionalPrice += additionalServiceEntity.Amount;
                        }
                    }
                }

                if (additionalServices.Any())
                {
                    await _unitOfWork.Repository<CheckoutAdditionalService>().AddRangeAsync(additionalServices);
                }

                checkout.AdditionalPrice = (decimal)additionalPrice;
                //checkout.DistancePrice =                                                                  // them logic
                checkout.TotalPrice = (decimal)additionalPrice + checkout.ServicePrice;                   //thieu distance price
                _unitOfWork.Repository<Checkout>().Update(checkout);

                await _unitOfWork.SaveChangesAsync();

                return new CheckoutResponseDTO1()
                {
                    CheckoutId = checkout.Id,
                    CustomerId = userId,
                    AdditionalPrice = (decimal)additionalPrice,
                    AdditionalServices = additionalServices.Select(a => new CheckoutAdditionalServiceResponseDTO
                    {
                        AdditionalServiceId = a.AdditionalServiceId,
                        AdditionalServiceName = a.AdditionalServiceName,
                        Amount = (double)a.Amount,
                        IsActive = a.IsActive,
                        Description = a.Description,
                        Duration = a.Duration,
                        Url = a.Url
                    }).ToList(),
                    AddressId = requestDTO.AddressId,
                    AddressLine = checkoutAddress.AddressLine1,
                    City = checkoutAddress.City,
                    CleaningServiceId = requestDTO.ServiceId,
                    CleaningServiceName = checkoutService.ServiceName,
                    District = checkoutAddress.District,
                    PlaceId = checkoutAddress.PlaceId,
                    ServicePrice = checkoutService.Price,
                    Status = CheckoutStatus.Pending.ToString(),
                    TimeSlotId = requestDTO.ServiceTimeSlotId,
                    BookingDate = requestDTO.BookingDate,
                    DateOfWeek = checkoutTimeSlot.DayOfWeek,
                    EndTime = checkoutTimeSlot.EndTime,
                    StartTime = checkoutTimeSlot.StartTime,
                    DistancePrice = checkout.DistancePrice,
                    TotalPrice = checkout.TotalPrice
                };
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database Error: {dbEx.Message} | Inner: {dbEx.InnerException?.Message}");
                throw new Exception(CommonConst.DatabaseError);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateCheckout: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ChangeStatusCheckout(Guid checkoutId)
        {
            var checkout = _unitOfWork.Repository<Checkout>().GetById(checkoutId);
            if (checkout == null)
            {
                return false;
            }

            checkout.Status = CheckoutStatus.Completed.ToString();

            _unitOfWork.Repository<Checkout>().Update(checkout);
            await _unitOfWork.Repository<Checkout>().SaveChangesAsync();
            return true;
        }

        public async Task<List<CheckoutResponseDTO1>> GetPendingCheckouts(ClaimsPrincipal user)
        {
            var userId = user.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new List<CheckoutResponseDTO1>();
            }

            var pendingCheckouts = await _unitOfWork.Repository<Checkout>()
                .GetAll()
                .Where(c => c.CustomerId == userId && c.Status == CheckoutStatus.Pending.ToString())
                .Include(c => c.CheckoutAdditionalServices)
                    .ThenInclude(cas => cas.AdditionalService)
                .ToListAsync();

            return pendingCheckouts.Select(c => new CheckoutResponseDTO1
            {
                CheckoutId = c.Id,
                CustomerId = c.CustomerId,
                AdditionalPrice = c.AdditionalPrice,
                AdditionalServices = c.CheckoutAdditionalServices.Select(a => new CheckoutAdditionalServiceResponseDTO
                {
                    AdditionalServiceId = a.AdditionalServiceId,
                    AdditionalServiceName = a.AdditionalServiceName,
                    Amount = (double)a.Amount,
                    IsActive = a.IsActive,
                    Url = a.Url,
                    Duration = a.Duration,
                    Description = a.Description,
                }).ToList(),
                AddressId = c.AddressId,
                AddressLine = c.AddressLine,
                City = c.City,
                CleaningServiceId = c.CleaningServiceId,
                CleaningServiceName = c.ServiceName,
                DateOfWeek = c.DayOfWeek,
                EndTime = c.EndTime,
                StartTime = c.StartTime,
                TimeSlotId = c.TimeSLotId,
                BookingDate = c.BookingDate,
                District = c.District,
                PlaceId = c.PlaceId,
                ServicePrice = c.ServicePrice,
                Status = c.Status,
                TotalPrice = c.TotalPrice,
                DistancePrice = c.DistancePrice
            }).ToList();
        }

        public async Task<CheckoutResponseDTO1> GetCheckoutById(Guid checkoutId)
        {
            var checkout = await _unitOfWork.Repository<Checkout>()
                                                .GetAll()
                                                .Where(c => c.Id == checkoutId)
                                                .Include(c => c.CheckoutAdditionalServices)
                                                    .ThenInclude(cas => cas.AdditionalService)
                                                .FirstOrDefaultAsync();

            if (checkout == null)
            {
                return new CheckoutResponseDTO1(); // Return empty list if userId is null
            }

            return new CheckoutResponseDTO1()
            {
                CheckoutId = checkoutId,
                CustomerId = checkout.CustomerId,
                AdditionalPrice = (decimal)checkout.AdditionalPrice,
                AdditionalServices = checkout.CheckoutAdditionalServices.Select(a => new CheckoutAdditionalServiceResponseDTO
                {
                    AdditionalServiceId = a.AdditionalServiceId,
                    AdditionalServiceName = a.AdditionalServiceName,
                    Amount = (double)a.Amount,
                    IsActive = a.IsActive,
                    Description = a.Description,
                    Duration = a.Duration,
                    Url = a.Url
                }).ToList(),
                AddressId = checkout.AddressId,
                AddressLine = checkout.AddressLine,
                City = checkout.City,
                CleaningServiceId = checkout.CleaningServiceId,
                CleaningServiceName = checkout.ServiceName,
                District = checkout.District,
                PlaceId = checkout.PlaceId,
                ServicePrice = checkout.ServicePrice,
                Status = CheckoutStatus.Pending.ToString(),
                TimeSlotId = checkout.TimeSLotId,
                BookingDate = checkout.BookingDate,
                DateOfWeek = checkout.DayOfWeek,
                EndTime = checkout.EndTime,
                StartTime = checkout.StartTime,
                DistancePrice = checkout.DistancePrice,
                TotalPrice = checkout.TotalPrice
            };
        }
    }
}

