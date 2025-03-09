using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCP.Repository.Entities;

namespace HCP.Service.DTOs.WalletDTO
{
    //DTO xu ly hoat dong rut va nap cua user wallet: 1 la nap, 2 la rut
    public class ProcessWalletDTO
    {
        public int Type { get; set; }
        public double Amount { get; set; }
        public double Current { get; set; }
        public double AfterAmount { get; set; }
        public string UserId { get; set; }
        public Guid? ReferenceId { get; set; }
        public AppUser User { get; set; }
        public bool Status { get; set; }

    }
    public class WalletWithdrawRequestDTO
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public Guid? ReferenceId { get; set; }
        public AppUser User { get; set; }

    }
}
