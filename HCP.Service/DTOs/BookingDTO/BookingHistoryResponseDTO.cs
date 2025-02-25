using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.BookingDTO
{
    public class BookingHistoryResponseDTO
    {
        public DateTime PreferDateStart { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string Note { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string AddressLine { get; set; }
        public string ServiceName { get; set; }
        public double CleaningServiceDuration { get; set; }
        public Guid BookingId { get; set; }
    }
    public class BookingHistoryDetailResponseDTO
    {
        public Guid BookingId { get; set; }
        public DateTime PreferDateStart { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string Note { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string AddressLine { get; set; }
        public string ServiceName { get; set; }
        public List<string> AdditionalServiceName { get; set; }
        public DateTime PaymentDate { get; set; }
        public double CleaningServiceDuration { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
    }
}
