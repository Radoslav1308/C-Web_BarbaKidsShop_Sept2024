using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbaKidsShop.Data.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        public DateTime OrderDate { get; set; }
       
        public int ShippingDetailId { get; set; }

        [ForeignKey(nameof(ShippingDetailId))]
        public ShippingDetail ShippingDetail { get; set; } = null!;


        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
