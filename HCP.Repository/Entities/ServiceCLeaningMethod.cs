using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class ServiceCleaningMethod : BaseEntity
    {
        public Guid ServiceId { get; set; }
        public Guid CleaningMethodId { get; set; }
        
        public virtual HomeService Service { get; set; }
        public virtual CleaningMethod CleaningMethod { get; set; }
    }
}
