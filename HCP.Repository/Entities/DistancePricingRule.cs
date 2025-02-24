using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class DistancePricingRule : BaseEntity
    {
        public double MinDistance { get; set; }
        public double MaxDistance { get; set; }
        public decimal BaseFee { get; set; }
        public decimal ExtraPerKm { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("CleaningService")]

        public Guid CleaningServiceId { get; set; }
        public CleaningService CleaningService { get; set; }
    }


}
