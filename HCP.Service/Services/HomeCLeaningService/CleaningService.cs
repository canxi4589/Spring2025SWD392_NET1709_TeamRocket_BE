using HCP.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCP.Repository.Entities;
using HCP.Service.DTOs;
using Microsoft.EntityFrameworkCore;
using HCP.Repository.Migrations;
namespace HCP.Service.Services.HomeCleaningService
{
    public class CleaningService : ICleaningService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CleaningService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<HomeServiceDto>> GetAllServiceWithoutCategoryAsync()
        {
            var services = await _unitOfWork.Repository<HomeService>().ListAsync();

            var serviceDtos = services.Select(s => new HomeServiceDto
            {
                Id = s.Id,
                Name = s.Name,
                PictureUrl = s.PictureUrl,
                Description = s.Description,
                BasePrice = s.BasePrice,
                
            }).ToList();

            return serviceDtos;
        }
        public async Task<List<ServicePricingDto>> GetServicePricesForCustomer(Guid serviceId)
        {
            var cleaningMethods = await _unitOfWork.Repository<ServiceCleaningMethod>()
    .ListAsync(c => c.ServiceId == serviceId,
               q => q.Include(c => c.CleaningMethod)
                     .OrderBy(c => c.CleaningMethod.Name));

            var options = await _unitOfWork.Repository<ServiceOption>()
                .ListAsync(o => o.ServiceId == serviceId,
                           q => q.Include(o => o.Values)
                                 .OrderBy(o => o.Name));
            var result = new List<ServicePricingDto>();
            var service = await _unitOfWork.Repository<HomeService>().FindAsync(c => c.Id == serviceId);
            var pricehe = service == null ? 20 : service.BasePrice;
            foreach (var method in cleaningMethods)
            {
                var methodPricing = new ServicePricingDto
                {
                   
                    CleaningMethod = method.CleaningMethod.Name, 
                    Variants = new List<ServiceVariantDto>()
                };

                foreach (var option in options)
                {
                    var variant = new ServiceVariantDto
                    {
                        OptionName = option.Name, 
                        Prices = option.Values.Select(v => new ServicePriceDetailDto
                        {
                            OptionValue = v.Name, 
                            Price = (double)((double)v.PriceFactor * pricehe)
                        }).ToList()
                    };

                    methodPricing.Variants.Add(variant);
                }

                result.Add(methodPricing);
            }

            return result;
        }

        public async Task<ServiceDetailDto> GetServiceDetails(Guid serviceId)
        {
            var service = await _unitOfWork.Repository<HomeService>()
                .FindAsync(s => s.Id == serviceId);
            var steps = await _unitOfWork.Repository<ServiceStep>().ListAsync();
            if (service == null)
            {
                throw new KeyNotFoundException("Service not found.");
            }

            var serviceDetail = new ServiceDetailDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                PictureUrl = service.PictureUrl,
                Steps = steps.OrderBy(s => s.StepOrder).Select(s => new ServiceStepDto
                {
                    StepOrder = s.StepOrder,
                    Description = s.Description
                }).ToList(),
                
            };
            serviceDetail.PricingOptions = await GetServicePricesForCustomer(serviceId);
            return serviceDetail;
        }

    }
}
