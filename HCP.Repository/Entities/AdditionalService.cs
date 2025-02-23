using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class AdditionalService : BaseEntity
    {
        public string Name { get; set; }
        [ForeignKey("Service")]
        public Guid CleaningServiceId { get; set; }
        public double Amount { get; set; }
        public bool IsActive { get; set; }
        public CleaningService Service { get; set; }
    }
}
