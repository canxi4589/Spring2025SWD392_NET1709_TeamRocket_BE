using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class ServiceCategory : BaseEntity
    {
        public string CategoryName { get; set; }
        public string PictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        //public string Description { get; set; }

        // Navigation properties
        public ICollection<CleaningService> CleaningServices { get; set; }
    }
}
