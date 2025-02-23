using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class BookingFinishProof : BaseEntity
    {
        [ForeignKey("Booking")]
        public Guid BookingId { get; set; }
        public Booking Booking { get; set; }
        public string Title {  get; set; }
        public string ImgUrl {  get; set; }
    }
}
