using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class WalletTransaction : BaseEntity
    {
        public decimal Amount { get; set; }
        public decimal Current { get; set; }
        public decimal AfterAmount { get; set; }
        public string Type { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public Guid? ReferenceId { get; set; }
        public AppUser User { get; set; }
    }
}
