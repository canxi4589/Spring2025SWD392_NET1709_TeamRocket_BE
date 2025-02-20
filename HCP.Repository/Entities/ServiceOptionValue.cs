using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class ServiceOptionValue : BaseEntity
    {
        public string? Name { get; set; }
        public decimal PriceFactor { get; set; }
        public Guid ServiceOptionId { get; set; }

        public virtual ServiceOption? ServiceOption { get; set; }
    }
}
