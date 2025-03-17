using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class RefundRequest : BaseEntity
    {
        [ForeignKey("Booking")]
        public Guid BookingId { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string ProofOfPayment { get; set; }
        [ForeignKey("Staff")]
        public string? AcceptBy { get; set; }
        public DateTime? ResolutionDate { get; set; }

        // Navigation properties
        public Booking Booking { get; set; }
        public AppUser? Staff { get; set; }

    }
}
