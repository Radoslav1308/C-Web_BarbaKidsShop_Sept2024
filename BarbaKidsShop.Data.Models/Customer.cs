using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static BarbaKidsShop.Common.EntityValidationConstants.CustomerConstants;

namespace BarbaKidsShop.Data.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required, MaxLength(UsernameMaxLength)]
        public string Username { get; set; } = null!;

        [Required, MaxLength(FullNameMaxLength)]
        public string FullName { get; set; } = null!;

        [Required, MaxLength(PhoneNumberMaxLength)]
        public string PhoneNumber { get; set; } = null!;

        [Required, MaxLength(AddressMaxLength)]
        public string Address { get; set; } = null!;

        [Required, Range(PasswordMinLength, PasswordMaxLength)]
        public string Password { get; set; } = null!;

        [Required, MaxLength(EmailMaxLength)]
        public string Email { get; set; } = null!;


        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
