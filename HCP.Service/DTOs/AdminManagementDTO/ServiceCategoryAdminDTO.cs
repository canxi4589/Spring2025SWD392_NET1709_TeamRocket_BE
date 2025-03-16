using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceAdminDTO;

namespace HCP.Service.DTOs.AdminManagementDTO
{
    public class ServiceCategoryAdminDTO
    {
        public class ServiceCategoryAdminShowDTO
        {
            public string CategoryName { get; set; }
            public Guid CategoryId { get; set; }
            public string PictureUrl { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public string Description { get; set; }
            public int NumberOfBooking { get; set; }
        }
        public class ServiceCategoryAdminShowListDTO
        {
            public List<ServiceCategoryAdminShowDTO> Items { get; set; }
            public int totalCount { get; set; }
            public int totalPages { get; set; }
            public bool hasNext { get; set; }
            public bool hasPrevious { get; set; }
        }
    }
}
