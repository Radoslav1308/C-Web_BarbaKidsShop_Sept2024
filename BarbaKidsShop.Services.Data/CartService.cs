using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        private IRepository<Product, int> productRepository;

        public CartService(IRepository<Order, int> orderRepository, IRepository<OrderDetail, int> orderDetailRepository, IRepository<Product, int> productRepository)
        {
            this.orderRepository = orderRepository;
            this.orderDetailRepository = orderDetailRepository;
            this.productRepository = productRepository;
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

        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            //var product = await this.productRepository.GetByIdAsync(productId);
            //if (product == null)
            //{
            //    throw new ArgumentException("Product not found.");
            //}

            //var order = await this.orderRepository
            //    .GetAllAttached()
            //    .FirstOrDefaultAsync(o => o.UserId == userId && o.ShippingDetailId == 0);

            //if (order == null)
            //{
            //    order = new Order
            //    {
            //        UserId = userId,
            //        OrderDate = DateTime.UtcNow,
            //        ShippingDetailId = 1
            //    };

            //    await this.orderRepository.AddAsync(order);
            //}

            //var orderDetail = await this.orderDetailRepository
            //    .GetAllAttached()
            //    .FirstOrDefaultAsync(od => od.OrderId == order.OrderId && od.ProductId == productId);

            //if (orderDetail != null)
            //{
            //    orderDetail.Quantity += quantity;
            //}
            //else
            //{
            //    orderDetail = new OrderDetail
            //    {
            //        OrderId = order.OrderId,
            //        ProductId = productId,
            //        Quantity = quantity,
            //        Price = product.Price
            //    };

            //    await this.orderDetailRepository.AddAsync(orderDetail);
            //}
        }
        public Task ClearCartAsync()
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
