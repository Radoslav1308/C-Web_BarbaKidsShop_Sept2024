using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using static BarbaKidsShop.Common.EntityValidationConstants.ApplicationUserConstants;

namespace BarbaKidsShop.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(FullNameMaxLength)]
        public string FullName { get; set; } = null!;

        [Required, MaxLength(AddressMaxLength)]
        public string Address { get; set; } = null!;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
