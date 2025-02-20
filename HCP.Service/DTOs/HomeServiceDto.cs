using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.DTOs
{
    public class HomeServiceDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? PictureUrl { get; set; }
        public string? Description { get; set; }
        public double? BasePrice { get; set; }
    }
}
