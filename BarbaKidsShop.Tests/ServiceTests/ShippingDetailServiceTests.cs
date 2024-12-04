using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Data;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Data.Repository;
using BarbaKidsShop.Data.Repository.Interfaces;
using BarbaKidsShop.Services.Data;
using BarbaKidsShop.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace BarbaKidsShop.Tests.ServiceTests
{
    [TestFixture]
    public class ShippingDetailServiceTests
    {
        private ApplicationDbContext _context;
        private ShippingDetailService _shippingDetailService;
        private IRepository<Order, int> _orderRepository;
        private IRepository<ShippingDetail, int> _shippingDetailRepository;
        private IRepository<ProductOrder, int> _productOrderRepository;

        private string _validUserId;
        private Order _order;

        [SetUp]
        public void Setup()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Initialize the context
            _context = new ApplicationDbContext(options);
            _orderRepository = new BaseRepository<Order, int>(_context); // In-memory repository for Order
            _shippingDetailRepository = new BaseRepository<ShippingDetail, int>(_context); // In-memory repository for ShippingDetail
            _productOrderRepository = new BaseRepository<ProductOrder, int>(_context); // In-memory repository for ProductOrder

            // Initialize the service
            _shippingDetailService = new ShippingDetailService(

                _shippingDetailRepository,
                _orderRepository,
                _productOrderRepository
            );

            // Seed the database with a user and an order
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _validUserId = "d402e413-4d10-4a92-8eab-c1eec022e8bc";

            // Add an order for the valid user
            _order = new Order
            {
                OrderId = 1,
                UserId = _validUserId,
                ShippingDetail = null // Initially no shipping detail
            };
            _context.Orders.Add(_order);
            _context.SaveChanges();
        }

        [Test]
        public async Task CreateShippingDetailAsync_ShouldCreateShippingDetailSuccessfully()
        {
            // Arrange
            var model = new ShippingDetailViewModel
            {
                Id = 1,
                Address = "123 Test St",
                City = "Test City",
                PostalCode = "12345",
                Country = "Test Country"
            };

            // Act
            await _shippingDetailService.CreateShippingDetailAsync(model, _validUserId);

            // Assert: Verify the order now has the shipping detail
            var order = await _context.Orders.Include(o => o.ShippingDetail).FirstOrDefaultAsync(o => o.OrderId == _order.OrderId);
            Assert.That(order.ShippingDetail, Is.Not.Null); // Ensure ShippingDetail was added
            Assert.That(model.Address, Is.EqualTo(order.ShippingDetail.Address)); // Ensure the address matches
        }

        [Test]
        public async Task CreateShippingDetailAsync_ShouldThrowException_WhenNoOrderExistsForUser()
        {
            // Arrange
            var model = new ShippingDetailViewModel
            {
                Id = 1,
                Address = "123 Test St",
                City = "Test City",
                PostalCode = "12345",
                Country = "Test Country"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _shippingDetailService.CreateShippingDetailAsync(model, "invalid-user-id")
            );
            Assert.That("No open order found for the user.", Is.EqualTo(ex.Message));
        }

        [Test]
        public async Task CreateShippingDetailAsync_ShouldRemoveProductOrders_ForTheOrder()
        {
            // Arrange
            var model = new ShippingDetailViewModel
            {
                Id = 1,
                Address = "123 Test St",
                City = "Test City",
                PostalCode = "12345",
                Country = "Test Country"
            };

            var productOrder = new ProductOrder
            {
                ProductId = 1,
                OrderId = _order.OrderId,
                Quantity = 1
            };

            // Add product order to the in-memory database
            _context.ProductOrders.Add(productOrder);
            await _context.SaveChangesAsync();

            // Act
            await _shippingDetailService.CreateShippingDetailAsync(model, _validUserId);

            // Assert: Ensure ProductOrder is removed
            var removedProductOrder = await _context.ProductOrders
                .FirstOrDefaultAsync(po => po.OrderId == _order.OrderId);

            Assert.That(removedProductOrder, Is.Null); // Verify the ProductOrder was removed
        }
    }
}
