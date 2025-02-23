using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class Package : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public double Duration { get; set; }
        public string Version { get; set; }
        public string Type { get; set; }

        // Navigation properties
        public ICollection<HousekeeperPackage> HousekeeperPackages { get; set; }
    }
}
