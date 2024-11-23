using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Web.ViewModels;

namespace BarbaKidsShop.Services.Data.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<CartViewModel>> IndexGetAllProductsForUserInCartAsync(string userId);
        Task<CartViewModel> GetAddProductToCartAsync();
        Task AddToCartAsync(CartViewModel model);
        Task RemoveFromCartAsync(int productId);
        Task UpdateCartItemAsync(CartViewModel model);
        Task ClearCartAsync();
    }
}
