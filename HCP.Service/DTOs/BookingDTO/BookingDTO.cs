using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.BookingDTO
{
    public class BookingDTO
    {
        public decimal ServicePrice { get; set; }
        public decimal DistancePrice { get; set; }       
        public Guid CleaningServiceId { get; set; }
        public Guid TimeSlotId { get; set; }
        public string Note { get; set; }
        public Guid AddressId {  get; set; }
        public List<BookingAdditionalDTO> bookingAdditionalDTOs { get; set; }
    }
    public class CheckoutRequestDTO
    {
        public Guid TimeSlotId { get; set; }
        public Guid AddressId { get; set; }
        public Guid ServiceId { get; set; }
        public List<Guid> BookingAdditionalIds { get; set; } = new List<Guid>();
    }
    public class ConfirmBookingDTO
    {
        public DateTime StartDate {  get; set; }
        public Guid TimeSlotId { get; set; }
        public Guid AddressId { get; set; }
        public Guid ServiceId { get; set; }
        public string? Note {  get; set; }
        public List<Guid> BookingAdditionalIds { get; set; } = new List<Guid>();
        public decimal? totalAmount { get; set; }
    }
    public class CheckoutResponseDTO
    {
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public string UserName {  get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string AddressLine { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ServiceName {  get; set; }
        public string Distance {  get; set; }
        public decimal ServiceBasePrice {  get; set; }
        public decimal DistancePrice { get; set; }
        public decimal AddidionalPrice {  get; set; }
        public string? Note { get; set; }
        public string? exchangeRate {  get; set; }
        public decimal TotalPrice {  get; set; }
        public List<BookingAdditionalDTO> BookingAdditionalDTOs = new List<BookingAdditionalDTO>();
        public List<PaymentMethodDTO1> PaymentMethods { get; set; } = new List<PaymentMethodDTO1>();
    }
    public class BillingAddress
    {
        public string City { get; set; }
        public string District { get; set; }
        public string AddressLine {  get; set; }
        public string FullName {  get; set; }
        public string Phone { get; set; }
        public string Email {  get; set; }
    }

    public class BookingAdditionalDTO
    {
        public Guid AdditionalId { get; set; }
        public string Url {  get; set; }
        public string Name { get; set; }
        public decimal Price {  get; set; }
        public double? Duration {  get; set; }
    }
    public class PaymentMethodDTO1
    {
        public string Name { get; set; }
        public bool IsChoosable { get; set; }
        public string Reason { get; set; }
    }
    public class BookingProofDTO
    {
        public string Title { get; set; }
        public string ImgUrl { get; set; }
    }
    public class CompleteBookingDTO
    {
        public Guid BookingId { get; set; }
        public List<BookingProofDTO> Proofs { get; set; }
    }

}
