using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
