using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.BookingDTO
{
    public class BookingDTO
    {
        public Guid Id { get; set; }
        public DateTime PreferDateStart { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string CustomerId { get; set; }
        public Guid CleaningServiceId { get; set; }
        public Guid TimeSlotId { get; set; }
        public string Note { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string AddressLine { get; set; }
    }
    public class BookingRequestDTO
    {

    }

}
