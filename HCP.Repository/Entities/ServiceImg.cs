using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class ServiceImg : BaseEntity
    {
        [ForeignKey("CleaningService")]

        public Guid CleaningServiceId { get; set; }
        public string LinkUrl { get; set; }

        public CleaningService CleaningService { get; set; }
    }
}
