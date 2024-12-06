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
        private IRepository<ProductOrder, int> productOrderRepository;
        private IRepository<Product, int> productRepository;

        public CartService(IRepository<Order, int> orderRepository, IRepository<ProductOrder, int> productOrderRepository, IRepository<Product, int> productRepository)
        {
            this.orderRepository = orderRepository;
            this.productOrderRepository = productOrderRepository;
            this.productRepository = productRepository;
        }

        public async Task<IEnumerable<CartViewModel>> IndexGetAllProductsForUserInCartAsync(string userId)
        {
            var cartItems = await this.productOrderRepository
                .GetAllAttached()
                .Where(od => od.Order.UserId == userId && !od.Product.IsDeleted)
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
            var product = await this.productRepository.GetByIdAsync(productId);
            if (product.IsDeleted)
            {
                throw new ArgumentException("Product not found.");
            }

            var order = await this.orderRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(o => o.UserId == userId);

            if (order == null)
            {
                order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow
                };

                await this.orderRepository.AddAsync(order);
            }

            var productOrder = await this.productOrderRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(od => od.OrderId == order.OrderId && od.ProductId == productId);

            if (productOrder != null)
            {
                productOrder.Quantity += quantity;

                order.TotalPrice = product.Price * productOrder.Quantity;
                await this.productOrderRepository.UpdateAsync(productOrder);
            }
            else
            {
                productOrder = new ProductOrder
                {
                    OrderId = order.OrderId,
                    ProductId = productId,
                    Quantity = quantity
                };

                order.TotalPrice = product.Price * productOrder.Quantity;

                await this.productOrderRepository.AddAsync(productOrder);
            }           
        }
        public async Task ClearCartAsync(string userId)
        {
            var order = await this.orderRepository
              .GetAllAttached()
              .FirstOrDefaultAsync(o => o.UserId == userId);

            if (order == null)
            {
                throw new ArgumentException("Order not found.");
            }
            else
            {
                var relatedEntities = await this.productOrderRepository
                .GetAllAttached()
                .Where(po => po.OrderId == order.OrderId)
                .ToListAsync();

                foreach (var product in relatedEntities)
                {
                    await this.productOrderRepository.RemoveAsync(product);
                }
            }
        }

        public async Task RemoveFromCartAsync(int productId, string userId)
        {
            var order = await this.orderRepository
               .GetAllAttached()
               .FirstOrDefaultAsync(o => o.UserId == userId);

            if (order == null)
            {
                throw new ArgumentException("Order not found.");
            }
            else
            {
                var cartItem = await this.productOrderRepository
                .GetAllAttached()
                .FirstOrDefaultAsync(od => od.OrderId == order.OrderId && od.ProductId == productId);

                if (cartItem == null)
                {
                    throw new ArgumentException("Item not found.");
                }

                await this.productOrderRepository.RemoveAsync(cartItem);
            }        
        }
    }
}
