using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Repository
{
    public class RequestRepository : IRequestRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public RequestRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CleaningService>?> GetCleaningServices(string? status)
        {
            return (List<CleaningService>?)await _unitOfWork.Repository<CleaningService>().ListAsync
                (
                filter: c => status.IsNullOrEmpty() || c.Status == status,
                includeProperties: c => c.Include(cs => cs.AdditionalServices)
                .Include(cs => cs.ServiceImages)
                .Include(cs => cs.ServiceTimeSlots)
                .Include(cs => cs.DistancePricingRules)
                .Include(cs => cs.ServiceSteps)
                );
        }
    }
}
