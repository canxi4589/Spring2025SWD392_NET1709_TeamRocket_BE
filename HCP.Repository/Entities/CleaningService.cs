using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class CleaningService : BaseEntity
    {
        public string ServiceName { get; set; }
        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public decimal Rating { get; set; }
        public int RatingCount { get; set; }
        public decimal Price { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string AddressLine { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public double Duration { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }

        // Navigation properties
        public AppUser User { get; set; }
        public ServiceCategory Category { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<AdditionalService> AdditionalServices { get; set; }
        public ICollection<ServiceImg> ServiceImages { get; set; }
        public ICollection<ServiceSteps> ServiceSteps { get; set; }
        public ICollection<ServiceTimeSlot> ServiceTimeSlots { get; set; }
        public ICollection<ServiceRating> ServiceRatings { get; set; }
    }
}
