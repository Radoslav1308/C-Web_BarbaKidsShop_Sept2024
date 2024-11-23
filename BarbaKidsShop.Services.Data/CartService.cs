using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Services.Data.Interfaces;
using BarbaKidsShop.Web.ViewModels;

namespace BarbaKidsShop.Services.Data
{
    public class CartService : ICartService
    {
        public Task AddToCartAsync(CartViewModel model)
        {
            throw new NotImplementedException();
        }

        public Task ClearCartAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CartIndexViewModel>> IndexGetAllProductsInCartAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromCartAsync(int productId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCartItemAsync(CartViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
