using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.DTOs.DTOs.FilterDTO
{
    public class ServiceFilterOptionsDTO
    {
        public List<CategoryFilterDTO> Categories { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public List<RatingFilterDTO> RatingOptions { get; set; }
    }

    public class CategoryFilterDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class RatingFilterDTO
    {
        public decimal Range { get; set; }
        public int Count { get; set; }
    }

}
