using HCP.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCP.Repository.Entities;
using HCP.Service.DTOs;
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
    }
}
