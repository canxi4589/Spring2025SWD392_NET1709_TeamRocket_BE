using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.FilterDTO
{
    public class ServiceFilterRequest
    {
        public string? UserPlaceId { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public List<Guid>? CategoryIds { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<decimal>? Ratings { get; set; }
        public string? Search { get; set; }
    }

}
