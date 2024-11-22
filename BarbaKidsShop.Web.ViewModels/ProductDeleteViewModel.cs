using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static BarbaKidsShop.Common.EntityValidationConstants.ProductConstants;

namespace BarbaKidsShop.Web.ViewModels
{
    public class ProductDeleteViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(ProductNameMinLength)]
        [MaxLength(ProductNameMaxLength)]
        public string ProductName { get; set; } = null!;
    }
}
