using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.CleaningServiceDTO
{
    public class CleaningServiceItemDTO
    {
        public Guid id { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public decimal overallRating { get; set; }
        public decimal price {  get; set; }
        public string location { get; set; }

    }
    public class CategoryDTO
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imgUrl { get; set; }
    }

}
