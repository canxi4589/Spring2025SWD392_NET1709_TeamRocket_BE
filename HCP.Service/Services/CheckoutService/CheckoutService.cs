using HCP.DTOs.DTOs.CheckoutDTO;
using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace HCP.Service.Services.CheckoutService
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGoongDistanceService _goongDistanceService;

        public CheckoutService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IGoongDistanceService goongDistanceService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _goongDistanceService = goongDistanceService;
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

                var checkoutAddress = await _unitOfWork.Repository<Address>().GetEntityByIdAsync(requestDTO.AddressId);
                var checkoutService = await _unitOfWork.Repository<CleaningService>().GetEntityByIdAsync(requestDTO.ServiceId);
                var checkoutTimeSlot = await _unitOfWork.Repository<ServiceTimeSlot>().GetEntityByIdAsync(requestDTO.ServiceTimeSlotId);

                double? distance = await _goongDistanceService.GetDistanceAsync(checkoutAddress.PlaceId, checkoutService.PlaceId);
                if (distance == null)
                    throw new Exception(CommonConst.SomethingWrongMessage);

                var customer = await _userManager.FindByIdAsync(userId);
                if (customer == null)
                {
                    throw new Exception(CustomerConst.NotFoundError);
                }

                if (checkoutService == null || checkoutAddress == null || checkoutTimeSlot == null)
                {
                    throw new Exception(CommonConst.NotFoundError);
                }

                Checkout? checkExistedCheckout = null;

                var checkouts = _unitOfWork.Repository<Checkout>()
                    .GetAll()
                    .Include(c => c.CheckoutAdditionalServices)
                    .Where(c =>
                        c.CleaningServiceId == requestDTO.ServiceId &&
                        c.AddressId == requestDTO.AddressId &&
                        c.TimeSLotId == requestDTO.ServiceTimeSlotId)
                    .AsEnumerable();

                var requestBookingDate = requestDTO.BookingDate.Date;

                var filteredCheckouts = checkouts.Where(c => c.BookingDate.Date == requestBookingDate).ToList();

                if (filteredCheckouts.Count != 0)
                {
                    checkExistedCheckout = filteredCheckouts.FirstOrDefault(c =>
                        (c.CheckoutAdditionalServices.Count == 0 && requestDTO.AdditionalServices.Count == 0) ||
                        c.CheckoutAdditionalServices
                            .Select(s => s.AdditionalServiceId)
                            .OrderBy(id => id)
                            .SequenceEqual(requestDTO.AdditionalServices
                                .Select(s => s.AdditionalServiceId)
                                .OrderBy(id => id))
                    );
                }

                if (checkExistedCheckout != null)
                {
                    bool isWalletChoosableExisted = (decimal)customer.BalanceWallet >= checkExistedCheckout.TotalPrice;
                    var paymentMethodsExisted = new List<PaymentMethodDTO>
                    {
                        new PaymentMethodDTO { Name = KeyConst.Wallet, IsChoosable = isWalletChoosableExisted },
                        new PaymentMethodDTO { Name = KeyConst.VNPay, IsChoosable = true }
                    };

                    return new CheckoutResponseDTO1()
                    {
                        PaymentMethods = paymentMethodsExisted,
                        Distance = $"{distance} km",
                        CheckoutId = checkExistedCheckout.Id,
                        CustomerId = checkExistedCheckout.CustomerId,
                        AdditionalPrice = checkExistedCheckout.AdditionalPrice,
                        DistancePrice = checkExistedCheckout.DistancePrice,
                        TotalPrice = checkExistedCheckout.TotalPrice,
                        AddressId = checkExistedCheckout.AddressId,
                        AddressLine = checkExistedCheckout.AddressLine,
                        BookingDate = checkExistedCheckout.BookingDate,
                        City = checkExistedCheckout.City,
                        CleaningServiceId = checkExistedCheckout.CleaningServiceId,
                        CleaningServiceName = checkExistedCheckout.ServiceName,
                        DateOfWeek = checkExistedCheckout.DayOfWeek,
                        District = checkExistedCheckout.District,
                        EndTime = checkExistedCheckout.EndTime,
                        PlaceId = checkExistedCheckout.PlaceId,
                        ServicePrice = checkExistedCheckout.ServicePrice,
                        StartTime = checkExistedCheckout.StartTime,
                        Status = checkExistedCheckout.Status,
                        TimeSlotId = checkExistedCheckout.TimeSLotId,
                        AdditionalServices = checkExistedCheckout.CheckoutAdditionalServices
                .Select(a => new CheckoutAdditionalServiceResponseDTO
                {
                    AdditionalServiceId = a.AdditionalServiceId,
                    AdditionalServiceName = a.AdditionalServiceName,
                    Amount = a.Amount,
                    IsActive = a.IsActive,
                    Description = a.Description,
                    Duration = a.Duration,
                    Url = a.Url
                }).ToList()
                    };
                }

                var pricingRule = await _unitOfWork.Repository<DistancePricingRule>().GetEntityAsync(
                    rule => rule.CleaningServiceId == checkoutService.Id &&
                            rule.MinDistance <= distance &&
                            rule.MaxDistance >= distance &&
                            rule.IsActive
                );
                if (pricingRule == null)
                {
                    throw new Exception(CleaningServiceConst.ServiceNotAvailableDistance);
                }

                var distancePrice = pricingRule.BaseFee;
                decimal additionalPrice = 0;

                var checkout = new Checkout()
                {
                    AddressId = requestDTO.AddressId,
                    AddressLine = checkoutAddress.AddressLine1,
                    AdditionalPrice = 0,                                    
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
                    Customer = customer,
                    Note = string.Empty,
                    TimeSLotId = requestDTO.ServiceTimeSlotId,
                    DistancePrice = distancePrice,
                    TotalPrice = checkoutService.Price + distancePrice,     // Update later with additional services
                };

                await _unitOfWork.Repository<Checkout>().AddAsync(checkout);
                await _unitOfWork.SaveChangesAsync();

                var additionalServices = new List<CheckoutAdditionalService>();

                if (requestDTO.AdditionalServices != null)
                {
                    foreach (var additional in requestDTO.AdditionalServices)
                    {
                        var additionalServiceEntity = await _unitOfWork.Repository<AdditionalService>()
                            .GetEntityByIdAsync(additional.AdditionalServiceId);

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
                            additionalPrice += (decimal)additionalServiceEntity.Amount;
                        }
                    }
                }

                if (additionalServices.Any())
                {
                    await _unitOfWork.Repository<CheckoutAdditionalService>().AddRangeAsync(additionalServices);
                }

                checkout.AdditionalPrice = additionalPrice;
                checkout.TotalPrice = checkout.ServicePrice + checkout.DistancePrice + additionalPrice;

                _unitOfWork.Repository<Checkout>().Update(checkout);
                await _unitOfWork.SaveChangesAsync();

                bool isWalletChoosable = (decimal)customer.BalanceWallet >= checkout.TotalPrice;
                var paymentMethods = new List<PaymentMethodDTO>
                    {
                        new PaymentMethodDTO { Name = KeyConst.Wallet, IsChoosable = isWalletChoosable },
                        new PaymentMethodDTO { Name = KeyConst.VNPay, IsChoosable = true }
                    };

                return new CheckoutResponseDTO1()
                {
                    CheckoutId = checkout.Id,
                    CustomerId = userId,
                    AdditionalPrice = additionalPrice,
                    Distance = $"{distance} km",
                    DistancePrice = checkout.DistancePrice,
                    TotalPrice = checkout.TotalPrice,
                    PaymentMethods = paymentMethods,
                    AddressId = requestDTO.AddressId,
                    AddressLine = checkout.AddressLine,
                    BookingDate = checkout.BookingDate,
                    City = checkout.City,
                    CleaningServiceId = requestDTO.ServiceId,
                    CleaningServiceName = checkout.ServiceName,
                    DateOfWeek = checkout.DayOfWeek,
                    District = checkout.District,
                    EndTime = checkout.EndTime,
                    PlaceId = checkout.PlaceId,
                    ServicePrice = checkout.ServicePrice,
                    StartTime = checkout.StartTime,
                    Status = checkout.Status,
                    TimeSlotId = checkout.TimeSLotId,
                    AdditionalServices = additionalServices.Select(a => new CheckoutAdditionalServiceResponseDTO
                    {
                        AdditionalServiceId = a.AdditionalServiceId,
                        AdditionalServiceName = a.AdditionalServiceName,
                        Amount = a.Amount,
                        IsActive = a.IsActive,
                        Description = a.Description,
                        Duration = a.Duration,
                        Url = a.Url
                    }).ToList(),
                };
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

            return pendingCheckouts.Select(c => new CheckoutResponseDTO1()
            {
                CheckoutId = c.Id,
                CustomerId = c.CustomerId,
                AdditionalPrice = c.AdditionalPrice,
                AdditionalServices = c.CheckoutAdditionalServices.Select(a => new CheckoutAdditionalServiceResponseDTO()
                {
                    AdditionalServiceId = a.AdditionalServiceId,
                    AdditionalServiceName = a.AdditionalServiceName,
                    Amount = a.Amount,
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
                                                .Include(c => c.Customer)
                                                .Include(c => c.CheckoutAdditionalServices)
                                                    .ThenInclude(cas => cas.AdditionalService)
                                                .FirstOrDefaultAsync();

            var cleaningService = await _unitOfWork.Repository<CleaningService>().FindAsync(cs => cs.Id == checkout.CleaningServiceId);
            if (checkout == null)
            {
                return new CheckoutResponseDTO1();
            }

            bool isWalletChoosable = (decimal)checkout.Customer.BalanceWallet >= checkout.TotalPrice;

            var paymentMethods = new List<PaymentMethodDTO>
                    {
                        new PaymentMethodDTO { Name = KeyConst.Wallet, IsChoosable = isWalletChoosable },
                        new PaymentMethodDTO { Name = KeyConst.VNPay, IsChoosable = true }
                    };

            double? distance = await _goongDistanceService.GetDistanceAsync(checkout.PlaceId, cleaningService.PlaceId);

            return new CheckoutResponseDTO1()
            {
                CheckoutId = checkout.Id,
                CustomerId = checkout.CustomerId,
                AdditionalPrice = checkout.AdditionalPrice,
                AdditionalServices = checkout.CheckoutAdditionalServices.Select(a => new CheckoutAdditionalServiceResponseDTO()
                {
                    AdditionalServiceId = a.AdditionalServiceId,
                    AdditionalServiceName = a.AdditionalServiceName,
                    Amount = a.Amount,
                    IsActive = a.IsActive,
                    Url = a.Url,
                    Duration = a.Duration,
                    Description = a.Description,
                }).ToList(),
                AddressId = checkout.AddressId,
                AddressLine = checkout.AddressLine,
                City = checkout.City,
                CleaningServiceId = checkout.CleaningServiceId,
                CleaningServiceName = checkout.ServiceName,
                DateOfWeek = checkout.DayOfWeek,
                EndTime = checkout.EndTime,
                StartTime = checkout.StartTime,
                TimeSlotId = checkout.TimeSLotId,
                BookingDate = checkout.BookingDate,
                District = checkout.District,
                PlaceId = checkout.PlaceId,
                ServicePrice = checkout.ServicePrice,
                Status = checkout.Status,
                TotalPrice = checkout.TotalPrice,
                DistancePrice = checkout.DistancePrice,
                Distance = $"{distance} km",
                PaymentMethods = paymentMethods
            };
        }

    }
}