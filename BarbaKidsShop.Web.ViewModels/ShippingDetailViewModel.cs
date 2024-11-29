using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static BarbaKidsShop.Common.EntityValidationConstants.ApplicationUserConstants;
using static BarbaKidsShop.Common.EntityValidationConstants.ShippingDetailConstants;

namespace BarbaKidsShop.Web.ViewModels
{
    public class ShippingDetailViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(FullNameMinLength)]
        [MaxLength(FullNameMaxLength)]
        public string FullName { get; set; } = null!;

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
    }
}
