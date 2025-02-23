using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class BookingAdditional : BaseEntity
    {
        [ForeignKey("AdditionalService")]
        public Guid AdditionalServiceId { get; set; }
        [ForeignKey("Booking")]

        public Guid BookingId { get; set; }
        public double Amount { get; set; }
        public bool IsActive { get; set; }

        public virtual AdditionalService AdditionalService { get; set; }
        public virtual Booking Booking { get; set; }
    }
}
