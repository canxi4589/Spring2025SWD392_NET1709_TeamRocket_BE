using HCP.Repository.Entities;
using HCP.Repository.GenericRepository;
using HCP.Repository.Interfaces;
using HCP.Service.Services.CustomerService;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.Services.RatingService
{
    public class RatingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICustomerService _customerService;

        public RatingService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ICustomerService customerService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _customerService = customerService;
        }
    }
}
