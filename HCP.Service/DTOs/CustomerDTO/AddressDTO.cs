using HCP.Repository.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.CustomerDTO
{
    public class AddressDTO
    {
        public Guid Id { get; internal set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Province { get; set; }
        public string ZipCode { get; set; }
        public string Title { get; set; }
        public bool IsDefault { get; set; } = false;
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
        public string Province { get; set; }

        [Required]
        [MaxLength(20)]
        public string Zipcode { get; set; }

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
        public string Province { get; set; }

        [Required]
        [MaxLength(20)]
        public string Zipcode { get; set; }

        public bool IsDefault { get; set; } = false;

        [MaxLength(50)]
        public string? Title { get; set; }
    }
}
