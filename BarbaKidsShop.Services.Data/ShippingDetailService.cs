using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Data.Repository.Interfaces;
using BarbaKidsShop.Services.Data.Interfaces;
using BarbaKidsShop.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BarbaKidsShop.Services.Data
{
    public class ShippingDetailService : IShippingDetailService
    {
        private IRepository<ShippingDetail, int> shippingDetailRepository;
        private IRepository<Order, int> orderRepository;
        private IRepository<ProductOrder, int> productOrderRepository;

        public ShippingDetailService(IRepository<ShippingDetail, int> shippingDetailRepository, IRepository<Order, int> orderRepository, IRepository<ProductOrder, int> productOrderRepository)
        {
            this.shippingDetailRepository = shippingDetailRepository;
            this.orderRepository = orderRepository;
            this.productOrderRepository = productOrderRepository;
        }

        public async Task CreateShippingDetailAsync(ShippingDetailViewModel model, string userId)
        {
            var order = await this.orderRepository
            .FirstOrDefaultAsync(o => o.UserId == userId);

            if (order == null)
            {
                throw new InvalidOperationException("No open order found for the user.");
            }

            var shippingDetail = new ShippingDetail
            {
                ShippingDetailId = model.Id,
                Address = model.Address,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country,
                OrderId = order.OrderId,
            };

            order.ShippingDetail = shippingDetail;
            
            await this.shippingDetailRepository.AddAsync(shippingDetail);
            
            await this.orderRepository.UpdateAsync(order);

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
}
