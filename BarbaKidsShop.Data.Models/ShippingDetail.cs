﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static BarbaKidsShop.Common.EntityValidationConstants.ShippingDetailConstants;

namespace BarbaKidsShop.Data.Models
{
    public class ShippingDetail
    {
        [Key]
        public int ShippingDetailId { get; set; }

        [Required]
        [MinLength(AddressMinLength)]
        [MaxLength(AddressMaxLength)]
        public string Address { get; set; } = null!;

        [Required]
        [MinLength(CityMinLength)]
        [MaxLength(CityMaxLength)]
        public string City { get; set; } = null!;

        [Required]
        [MinLength(PostalCodeMinLength)]
        [MaxLength(PostalCodeMaxLength)]
        public string PostalCode { get; set; } = null!;

        [Required]
        [MinLength(CountryMinLength)]
        [MaxLength(CountryMaxLength)]
        public string Country { get; set; } = null!;

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }
}
