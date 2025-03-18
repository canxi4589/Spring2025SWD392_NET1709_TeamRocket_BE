using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class Address : BaseEntity
    {
        [MaxLength(255)]
        public string AddressLine1 { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        public string PlaceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string District { get; set; }

        public bool IsDefault { get; set; } = false;

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        
        public virtual AppUser User { get; set; }

        [MaxLength(50)]
        public string? Title { get; set; }
    }
}
