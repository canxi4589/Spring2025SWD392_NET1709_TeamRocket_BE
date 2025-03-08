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
        //[Column(TypeName = "decimal(18,2)")]
        public double Amount { get; set; }
        public bool IsActive { get; set; }
        public string? Url { get; set; }
        public string? Description {  get; set; }
        public double? Duration { get; set; } 
        public CleaningService Service { get; set; }
    }
}
