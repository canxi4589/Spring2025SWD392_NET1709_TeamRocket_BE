using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class HousekeeperPackage : BaseEntity
    {
        [ForeignKey("Housekeeper")]

        public string UserId { get; set; }
        [ForeignKey("Package")]

        public Guid PackageId { get; set; }
        public double Duration { get; set; }
        public double SubscriptionDuration { get; set; }
        public string Type { get; set; }

        // Navigation properties
        public AppUser Housekeeper { get; set; }
        public Package Package { get; set; }
    }
}
