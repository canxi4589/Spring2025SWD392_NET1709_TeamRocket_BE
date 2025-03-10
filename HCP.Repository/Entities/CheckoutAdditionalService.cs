﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCP.Repository.Entities
{
    public class CheckoutAdditionalService : BaseEntity
    {
        [ForeignKey("AdditionalService")]
        public Guid AdditionalServiceId { get; set; }

        public string AdditionalServiceName { get; set; } = "This is Service about cleaning...";

        [ForeignKey("Checkout")]
        public Guid CheckoutId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public bool IsActive { get; set; }

        public string? Url { get; set; }
        public string? Description { get; set; }
        public double? Duration { get; set; }

        public virtual AdditionalService AdditionalService { get; set; }
        public virtual Checkout Checkout { get; set; }
    }
}