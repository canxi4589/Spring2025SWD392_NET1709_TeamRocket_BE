using HCP.Repository.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.DTOs.DTOs.CustomerDTO
{
    public class AddressDTO
    {
        public Guid Id { get;  set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string District { get; set; }
        public string PlaceId { get; set; }
        public string Title { get; set; }
        public bool IsDefault { get; set; } = false;
    }

    public class GetAddressListDTO
    {
        public List<AddressDTO> Items { get; set; }
        public int totalCount { get; set; }
        public int totalPages { get; set; }
        public bool hasNext { get; set; }
        public bool hasPrevious { get; set; }
    }

    public class CreataAddressDTO
    {
        [Required]
        [MaxLength(255)]
        public string Address { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(100)]
        public string District { get; set; }

        public string PlaceId { get; set; }

        public bool IsDefault { get; set; } = false;

        [MaxLength(50)]
        public string? Title { get; set; }
    }
    public class UpdateAddressDTO
    {

        public Guid Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Address { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(100)]
        public string District { get; set; }

        public string PlaceId { get; set; }

        public bool IsDefault { get; set; } = false;

        [MaxLength(50)]
        public string? Title { get; set; }
    }
}
