﻿using HCP.Service.DTOs.CleaningServiceDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string Location { get; set; }
        public string ServiceName { get; set; }
        public double CleaningServiceDuration { get; set; }
        public Guid BookingId { get; set; }
        public bool isRating { get; set; }
    }
    public class BookingHistoryResponseListDTO
    {
        public List<BookingHistoryResponseDTO> Items { get; set; }
        public int totalCount { get; set; }
        public int totalPages { get; set; }
        public bool hasNext { get; set; }
        public bool hasPrevious { get; set; }
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
        public string Location { get; set; }
        public string ServiceName { get; set; }
        public List<string> AdditionalServiceName { get; set; }
        public DateTime PaymentDate { get; set; }
        public double CleaningServiceDuration { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string HousekeeperName { get; set; }
        public string HouseKeeperMail { get; set; }
        public string HouseKeeperPhoneNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMail { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public bool isRating {  get; set; }
    }
    public class BookingTransactionShowDTO
    {
        public Guid BookingId { get; set; }
        public Guid PaymentId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMail { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public decimal Ammount { get; set; }
        public decimal Commission { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string ServiceName { get; set; }
        public string LinkUrl { get; set; }
    }
    public class BookingTransactionDetailShowDTO
    {
        public Guid BookingId { get; set; }
        public Guid PaymentId { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerMail { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public string HousekeeperName { get; set; }
        public string? HousekeeperMail { get; set; }
        public string? HousekeeperPhoneNumber { get; set; }
        public decimal Ammount { get; set; }
        public decimal Commission { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string ServiceName { get; set; }
        public string LinkUrl { get; set; }
    }
    public class BookingTransactionShowListDTO
    {
        public List<BookingTransactionShowDTO> Items { get; set; }
        public int totalCount { get; set; }
        public int totalPages { get; set; }
        public bool hasNext { get; set; }
        public bool hasPrevious { get; set; }
    }
}