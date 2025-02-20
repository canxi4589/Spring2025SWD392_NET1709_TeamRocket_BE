using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class ServiceStep : BaseEntity
    {
        public int StepOrder { get; set; }
        public string Description { get; set; }

        public Guid ServiceId { get; set; }

        public virtual HomeService Service { get; set; }

    }

}
