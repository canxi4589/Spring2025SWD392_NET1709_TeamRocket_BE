using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.CleaningServiceDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.Services.RequestService
{
    public class HandleRequestService : IHandleRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public HandleRequestService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }


        public async Task<(bool Success, string Message)> UpdateServiceStatusAsync(ServiceStatusUpdateDto dto)
        {
            var service = await _unitOfWork.Repository<CleaningService>().FindAsync(cs => cs.Id == dto.ServiceId);
            if (service == null)
            {
                return (false, "Service not found");
            }

            if (service.Status != "Pending")
            {
                return (false, "Service status can only be updated if it is 'Pending'");
            }

            service.Status = dto.IsApprove ? "Active" : "Rejected";

            await _unitOfWork.Repository<CleaningService>().SaveChangesAsync();
            return (true, "Service status updated successfully");
        }

    }
}
