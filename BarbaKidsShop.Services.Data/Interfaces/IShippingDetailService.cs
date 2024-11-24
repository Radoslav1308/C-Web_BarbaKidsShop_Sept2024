using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Web.ViewModels;

namespace BarbaKidsShop.Services.Data.Interfaces
{
    public interface IShippingDetailService
    {
        Task CreateShippingDetailAsync(ShippingDetailViewModel model, string userId);
    }
}
