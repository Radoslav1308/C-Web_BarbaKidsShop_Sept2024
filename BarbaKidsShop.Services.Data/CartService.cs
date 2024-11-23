using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Data.Repository.Interfaces;
using BarbaKidsShop.Services.Data.Interfaces;
using BarbaKidsShop.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BarbaKidsShop.Services.Data
{
    public class CartService : ICartService
    {
        private IRepository<Order, int> orderRepository;
        private IRepository<OrderDetail, int> orderDetailRepository;
        //private IRepository<ApplicationUser, int> userRepository;
        private IRepository<Product, int> productRepository;

        public CartService(IRepository<Order, int> orderRepository, IRepository<OrderDetail, int> orderDetailRepository, /*IRepository<ApplicationUser, int> userRepository*/ IRepository<Product, int> productRepository)
        {
            this.orderRepository = orderRepository;
            this.orderDetailRepository = orderDetailRepository;
            //this.userRepository = userRepository;
            this.productRepository = productRepository;
        }

        public Task AddToCartAsync(CartViewModel model)
        {
            throw new NotImplementedException();
        }

        public Task ClearCartAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CartViewModel>> IndexGetAllProductsForUserInCartAsync(string userId)
        {
            var cartItems = await this.orderDetailRepository
                .GetAllAttached()
                .Where(od => od.Order.UserId == userId)
                .Where(od => !od.Product.IsDeleted)
                .Include(od => od.Product)
                .Select(od => new CartViewModel
                {
                    Id = od.ProductId,
                    ProductName = od.Product.ProductName,
                    Price = od.Product.Price,
                    Quantity = od.Quantity
                })
                .ToListAsync();

            return cartItems;
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
