using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class Payment : BaseEntity
    {
        [ForeignKey("Booking")]

        public Guid BookingId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public Booking Booking { get; set; }
    }

}
