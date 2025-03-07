using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.CheckoutDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
                    throw new Exception("User ID not found in claims.");
                }

                var address = _unitOfWork.Repository<Address>().GetById(requestDTO.AddressId);
                var service = _unitOfWork.Repository<CleaningService>().GetById(requestDTO.ServiceId);

                if (service == null || address == null)
                {
                    throw new Exception("Invalid Service or Address.");
                }

                var additionalServices = new List<CheckoutAdditionalService>();
                var additionalPrice = 0.0;

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
                                Id = Guid.NewGuid(),
                                AdditionalServiceId = additional.AdditionalServiceId,
                                Amount = (decimal)additionalServiceEntity.Amount,
                                IsActive = additionalServiceEntity.IsActive
                            };

                            additionalServices.Add(checkoutAdditionalService);
                            additionalPrice += additionalServiceEntity.Amount;
                        }
                        else
                        {
                            Console.WriteLine($"Additional service not found: {additional.AdditionalServiceId}");
                        }
                    }
                }

                var checkout = new Checkout()
                {
                    AddressLine = address.AddressLine1,
                    AdditionalPrice = (decimal)additionalPrice,
                    CheckoutAdditionalServices = additionalServices,
                    City = address.City,
                    CleaningServiceId = requestDTO.ServiceId,
                    CustomerId = userId,
                    District = address.District,
                    ServicePrice = service.Price,
                    PlaceId = address.PlaceId,
                    Status = CheckoutStatus.Pending.ToString(),
                    Customer = await _userManager.FindByIdAsync(userId),
                    Note = string.Empty
                };

                await _unitOfWork.Repository<Checkout>().AddAsync(checkout);

                Console.WriteLine("Saving Checkout...");
                var saveResult = await _unitOfWork.SaveChangesAsync();
                Console.WriteLine($"SaveChangesAsync result: {saveResult}");

                foreach (var additionalService in additionalServices)
                {
                    additionalService.CheckoutId = checkout.Id;
                }

                await _unitOfWork.Repository<CheckoutAdditionalService>().AddRangeAsync(additionalServices);
                await _unitOfWork.SaveChangesAsync();

                return new CheckoutResponseDTO1()
                {
                    CustomerId = userId,
                    AdditionalPrice = (decimal)additionalPrice,
                    AdditionalServices = additionalServices.Select(a => new CheckoutAdditionalServiceResponseDTO
                    {
                        AdditionalServiceId = a.AdditionalServiceId,
                        Amount = (double)a.Amount,
                        IsActive = a.IsActive
                    }).ToList(),
                    AddressLine = address.AddressLine1,
                    City = address.City,
                    CleaningServiceId = requestDTO.ServiceId,
                    District = address.District,
                    PlaceId = address.PlaceId,
                    ServicePrice = service.Price,
                    Status = CheckoutStatus.Pending.ToString(),
                };
            }
                catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database Error: {dbEx.Message} | Inner: {dbEx.InnerException?.Message}");
                throw new Exception("Database error occurred.");
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
                return false; // ko tìm thấy checkout nào 
            }

            checkout.Status = CheckoutStatus.Completed.ToString();

            _unitOfWork.Repository<Checkout>().Update(checkout);
            return true;
        }

        public async Task<List<CheckoutResponseDTO1>> GetPendingCheckouts(ClaimsPrincipal user)
        {
            var userId = user.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new List<CheckoutResponseDTO1>(); // Return empty list if userId is null
            }

            var pendingCheckouts = await _unitOfWork.Repository<Checkout>()
                .GetAll()
                .Where(c => c.CustomerId == userId && c.Status == CheckoutStatus.Pending.ToString())
                .Include(c => c.CheckoutAdditionalServices)
                    .ThenInclude(cas => cas.AdditionalService)
                .ToListAsync();

            return pendingCheckouts.Select(c => new CheckoutResponseDTO1
            {
                CustomerId = c.CustomerId,
                AdditionalPrice = c.AdditionalPrice,
                AdditionalServices = c.CheckoutAdditionalServices.Select(a => new CheckoutAdditionalServiceResponseDTO
                {
                    AdditionalServiceId = a.AdditionalServiceId,
                    Amount = (double)a.Amount,
                    IsActive = a.IsActive
                }).ToList(),
                AddressLine = c.AddressLine,
                City = c.City,
                CleaningServiceId = c.CleaningServiceId,
                District = c.District,
                PlaceId = c.PlaceId,
                ServicePrice = c.ServicePrice,
                Status = c.Status
            }).ToList();
        }

    }
}

