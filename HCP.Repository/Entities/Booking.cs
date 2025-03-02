using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class Booking : BaseEntity
    {
        public DateTime PreferDateStart { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ServicePrice {  get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal DistancePrice {  get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal AddtionalPrice {  get; set; }
        public DateTime? CompletedAt { get; set; }
        public double? Rating { get; set; }
        public string? Feedback {  get; set; }

        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
        [ForeignKey("CleaningService")]
        public Guid CleaningServiceId { get; set; }
        public string Note { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string PlaceId { get; set; }
        public string AddressLine { get; set; }
        // Navigation properties
        public AppUser? Customer { get; set; }
        public CleaningService? CleaningService { get; set; }
        public ICollection<Payment>? Payments { get; set; } = new List<Payment>();
        public ICollection<BookingAdditional> BookingAdditionals { get; set; } = new List<BookingAdditional>();
    }

}
