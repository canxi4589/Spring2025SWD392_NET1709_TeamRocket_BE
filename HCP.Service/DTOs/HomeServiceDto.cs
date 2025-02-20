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
    public class ServicePricingDto
    {
        public string CleaningMethod { get; set; } // "Cleaning", "Polishing"
        public List<ServiceVariantDto> Variants { get; set; }
    }

    public class ServiceVariantDto
    {
        public string OptionName { get; set; } // "Material", "Seats"
        public List<ServicePriceDetailDto> Prices { get; set; }
    }

    public class ServicePriceDetailDto
    {
        public string OptionValue { get; set; } // "Leather", "Fabric", "2 Seats"
        public double Price { get; set; } // e.g., 200000
    }
}
