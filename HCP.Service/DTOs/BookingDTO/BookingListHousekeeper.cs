using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.BookingDTO
{
    // List Response
    public class BookingListResponseDto
    {
        public List<BookingListItemDto> Items { get; set; }
        public int TotalCount { get; set; }
    }

    public class BookingCountDTO
    {
        public int UpcomingBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CanceledBookings { get; set; }
        public int RefundedBookings { get; set; }
    }
    public class BookingListItemDto
    {
        public Guid Id { get; set; }
        public DateTime PreferDateStart { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public string Status { get; set; }
        public bool IsFinishable { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal ServicePrice { get; set; }
        public decimal DistancePrice { get; set; }
        public decimal AdditionalPrice { get; set; }

        // Service Info
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string? ServiceImageUrl { get; set; }

        // Customer Info
        public CustomerDto Customer { get; set; }

        // Address
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string District { get; set; }
    }

    public class CustomerDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class BookingDetailDto : BookingListItemDto
    {
        public DateTime CreatedDate { get; set; }
        public string Note { get; set; }
        public List<BookingAdditionalServiceDto> AdditionalServices { get; set; }

        public decimal? Rating { get; set; }
        public string? Feedback { get; set; }
        public List<BookingProofDto> Proofs { get; set; } = new List<BookingProofDto>();
    }

    public class BookingAdditionalServiceDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    // For submitting proofs
    public class BookingFinishProofsRequestDto
    {
        [Required]
        public Guid BookingId { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(4)]
        public List<BookingProofDto> Proofs { get; set; }
    }

    public class BookingProofDto
    {
        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public string ImgUrl { get; set; }
    }

    
}
