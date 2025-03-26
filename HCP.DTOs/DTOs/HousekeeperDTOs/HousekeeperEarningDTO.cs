using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCP.DTOs.DTOs.WalletDTO;

namespace HCP.DTOs.DTOs.HousekeeperDTOs
{
    public class HousekeeperEarningDTO
    {
        public decimal Price { get; set; }
        public decimal Fee { get; set; }
        public decimal YourEarn { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
    public class HousekeeperEarningListDTO
    {
        public List<HousekeeperEarningDTO> Items { get; set; }
        public int totalCount { get; set; }
        public int totalPages { get; set; }
        public bool hasNext { get; set; }
        public bool hasPrevious { get; set; }
    }
}
