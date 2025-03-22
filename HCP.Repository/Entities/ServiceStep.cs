using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class ServiceSteps : BaseEntity
    {
        public int StepOrder { get; set; }
        public string StepDescription { get; set; }

        public double? Duration { get; set; }
        [ForeignKey("Service")]

        public Guid ServiceId { get; set; }
        public CleaningService Service { get; set; }

    }

}
