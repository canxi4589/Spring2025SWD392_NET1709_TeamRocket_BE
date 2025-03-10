using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCP.Repository.Entities
{
    public class Checkout : BaseEntity
    {
        [ForeignKey("CleaningService")]
        public Guid CleaningServiceId { get; set; }

        public string ServiceName { get; set; }

        [ForeignKey("Customer")]
        public string CustomerId { get; set; }

        [ForeignKey("ServiceTimeSlot")]
        public Guid TimeSLotId { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string DayOfWeek { get; set; }

        public string Status { get; set; } = default!;

        public decimal TotalPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ServicePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DistancePrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdditionalPrice { get; set; } // Fixed spelling

        public string Note { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string PlaceId { get; set; }
        public string AddressLine { get; set; }

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual AppUser Customer { get; set; }

        public ICollection<CheckoutAdditionalService> CheckoutAdditionalServices { get; set; } = new List<CheckoutAdditionalService>();
    }
}