using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.AdminManagementDTO
{
    public class UserAdminDTO
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public DateTime? Birthday { get; set; }
        public string Avatar { get; set; }
    }
}
