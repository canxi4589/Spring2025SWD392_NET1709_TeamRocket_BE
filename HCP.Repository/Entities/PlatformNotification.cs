using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class PlatformNotification : BaseEntity
    {
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public string? ReturnUrl { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }

        // Navigation properties
        public AppUser? Sender { get; set; }
        public AppUser? Receiver { get; set; }
    }
}
