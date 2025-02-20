using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class ServiceOption : BaseEntity
    {
        public string Name { get; set; }
        public Guid ServiceId { get; set; }

        public virtual HomeService Service { get; set; }
        public virtual ICollection<ServiceOptionValue> Values { get; set; } = new List<ServiceOptionValue>();
    }
}
