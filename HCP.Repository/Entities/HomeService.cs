using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class HomeService : BaseEntity
    {
        public string? Name { get; set; }
        public Guid CategoryId { get; set; }
        public string? PictureUrl { get; set; }
        public string? Description { get; set; }
        public double? BasePrice {  get; set; }

        public virtual ServiceCategory? Category { get; set; }
        public virtual ICollection<ServiceOption> Options { get; set; } = new List<ServiceOption>();
        public virtual ICollection<ServiceCleaningMethod> CleaningMethods { get; set; } = new List<ServiceCleaningMethod>();
    }

}
