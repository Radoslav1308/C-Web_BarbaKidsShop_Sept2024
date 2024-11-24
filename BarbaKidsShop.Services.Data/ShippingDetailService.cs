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

        public ShippingDetailService(IRepository<ShippingDetail, int> shippingDetailRepository, IRepository<Order, int> orderRepository)
        {
            this.shippingDetailRepository = shippingDetailRepository;
            this.orderRepository = orderRepository;
        }

        public async Task CreateShippingDetailAsync(ShippingDetailViewModel model, string userId)
        {
            var order = await this.orderRepository
            .FirstOrDefaultAsync(o => o.UserId == userId && o.ShippingDetailId == null);

            if (order == null)
            {
                throw new InvalidOperationException("No open order found for the user.");
            }

            var shippingDetail = new ShippingDetail
            {
                Address = model.Address,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country
            };

            await this.shippingDetailRepository.AddAsync(shippingDetail);

            order.ShippingDetailId = shippingDetail.ShippingDetailId;
            await this.orderRepository.UpdateAsync(order);
        }
    }
}
