using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class ServiceRating : BaseEntity
    {
        public decimal Rating { get; set; }
        public string Review { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        [ForeignKey("CleaningService")]
        public Guid CleaningServiceId { get; set; }
        public string Status { get; set; }
        // Navigation properties
        public AppUser User { get; set; }
        public CleaningService CleaningService { get; set; }
    }
}
