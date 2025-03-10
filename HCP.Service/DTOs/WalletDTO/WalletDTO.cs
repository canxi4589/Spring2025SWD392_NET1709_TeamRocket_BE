using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCP.Repository.Entities;
using HCP.Service.DTOs.CustomerDTO;

namespace HCP.Service.DTOs.WalletDTO
{
    public class WalletTransactionWithdrawResponseDTO
    {
        public double Amount { get; set; }
        public double Current { get; set; }
        public double AfterAmount { get; set; }
        public string Type { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Mail { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? ReferenceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
    public class WalletTransactionDepositResponseDTO
    {
        public double Amount { get; set; }
        public double Current { get; set; }
        public double AfterAmount { get; set; }
        public string Type { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Mail { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? ReferenceId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
    public class WalletWithdrawRequestDTO
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Mail { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? ReferenceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
    public class WalletWithdrawStaffShowDTO
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public Guid? ReferenceId { get; set; }
        public string FullName { get; set; }
        public string Mail { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
    public class GetWalletWithdrawRequestListDTO
    {
        public List<WalletWithdrawStaffShowDTO> Items { get; set; }
        public int totalCount { get; set; }
        public int totalPages { get; set; }
        public bool hasNext { get; set; }
        public bool hasPrevious { get; set; }
    }
}
