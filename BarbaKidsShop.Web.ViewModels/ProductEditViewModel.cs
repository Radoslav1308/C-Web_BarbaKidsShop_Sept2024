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
    public class ProductEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(ProductNameMinLength)]
        [MaxLength(ProductNameMaxLength)]
        public string ProductName { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}
