using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static BarbaKidsShop.Common.EntityValidationConstants.CategoryConstants;

namespace BarbaKidsShop.Web.ViewModels
{
    public class CategoryViewModel
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(CategoryNameMaxLength)]
        public string Name { get; set; } = null!;
    }
}
