using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.AdminManagementDTO
{
    public class ServiceAdminDTO
    {
        public class ServiceAdminShowDTO
        {
            public Guid Id { get; set; }
            public string ServiceName { get; set; }
            public Guid CategoryId { get; set; }
            public string CategoryName { get; set; }
            public string Status { get; set; }
            public decimal Rating { get; set; }
            public int RatingCount { get; set; }
            public decimal Price { get; set; }
            public string City { get; set; }
            public string District { get; set; }
            public string AddressLine { get; set; }
            public string PlaceId { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public double Duration { get; set; }
            public string HousekeeperId { get; set; }
            public string HousekeeperName { get; set; }
            public string StaffId { get; set; }
            public string StaffName { get; set; }
            public string FirstImgLinkUrl { get; set; }
        }
    }
}
