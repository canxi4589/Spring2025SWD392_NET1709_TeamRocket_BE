using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class HousekeeperSkill : BaseEntity
    {
        [ForeignKey("ServiceCategory")]
        public Guid CategoryId { get; set; }
        [ForeignKey("Housekeeper")]
        public string HousekeeperId { get; set; }
        public string Status { get; set; }
        public int SkillLevel { get; set; }

        public AppUser Housekeeper { get; set; }
        public ServiceCategory ServiceCategory { get; set; }
    }
}
