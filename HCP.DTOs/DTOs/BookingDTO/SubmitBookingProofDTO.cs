using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.DTOs.DTOs.BookingDTO
{
    public class SubmitBookingProofDTO
    {
        public Guid BookingId { get; set; }
        public string Title { get; set; }
        public string ImgUrl { get; set; }
    }
}
