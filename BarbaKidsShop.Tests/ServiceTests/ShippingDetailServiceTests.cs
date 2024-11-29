using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Data.Repository.Interfaces;
using BarbaKidsShop.Services.Data;
using BarbaKidsShop.Web.ViewModels;
using Moq;
using NUnit.Framework;

namespace BarbaKidsShop.Tests.ServiceTests
{
    [TestFixture]
    public class ShippingDetailServiceTests
    {
        private Mock<IRepository<ShippingDetail, int>> mockShippingDetailRepository;
        private Mock<IRepository<Order, int>> mockOrderRepository;
        private Mock<IRepository<ProductOrder, int>> mockProductOrderRepository;
        private ShippingDetailService shippingDetailService;

        [SetUp]
        public void SetUp()
        {
            // Correctly mock the repositories with their specific types
            mockShippingDetailRepository = new Mock<IRepository<ShippingDetail, int>>();
            mockOrderRepository = new Mock<IRepository<Order, int>>();
            mockProductOrderRepository = new Mock<IRepository<ProductOrder, int>>();

            // Pass the mocks to the ShippingDetailService constructor in the correct order
            shippingDetailService = new ShippingDetailService(
                mockShippingDetailRepository.Object,
                mockOrderRepository.Object,
                mockProductOrderRepository.Object
            );
        }

        [Test]
        public async Task CreateShippingDetailAsync_ShouldCreateShippingDetail_WhenOrderExists()
        {
            // Arrange
            var model = new ShippingDetailViewModel
            {
                Address = "123 Street",
                City = "City",
                PostalCode = "12345",
                Country = "Country"
            };

            var userId = "user123";
            var order = new Order
            {
                UserId = userId,
                OrderId = 1,
                ShippingDetail = null // No shipping detail initially
            };

            var shippingDetail = new ShippingDetail
            {
                ShippingDetailId = 1,
                Address = model.Address,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country,
                OrderId = order.OrderId
            };

            // Mock repositories
            mockOrderRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(order);
            mockShippingDetailRepository.Setup(repo => repo.AddAsync(It.IsAny<ShippingDetail>())).Returns(Task.CompletedTask);
            mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<ProductOrder>().AsQueryable());

            // Act
            await shippingDetailService.CreateShippingDetailAsync(model, userId);

            // Assert
            mockShippingDetailRepository.Verify(repo => repo.AddAsync(It.IsAny<ShippingDetail>()), Times.Once);
            mockOrderRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Order>()), Times.Once);
            mockProductOrderRepository.Verify(repo => repo.RemoveAsync(It.IsAny<ProductOrder>()), Times.Once);
            Assert.That(order.ShippingDetail, Is.Not.Null);
            Assert.That(order.ShippingDetail.Address, Is.EqualTo(model.Address));
            Assert.That(order.ShippingDetailId, Is.EqualTo(shippingDetail.ShippingDetailId));
        }

        [Test]
        public void CreateShippingDetailAsync_ShouldThrowException_WhenNoOrderFound()
        {
            // Arrange
            var model = new ShippingDetailViewModel
            {
                Address = "123 Street",
                City = "City",
                PostalCode = "12345",
                Country = "Country"
            };

            var userId = "user123";

            // Mock repository to return null for no order found
            mockOrderRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync((Order)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => shippingDetailService.CreateShippingDetailAsync(model, userId));
            Assert.That(ex.Message, Is.EqualTo("No open order found for the user."));
        }

        [Test]
        public async Task CreateShippingDetailAsync_ShouldUpdateOrderShippingDetailId()
        {
            // Arrange
            var model = new ShippingDetailViewModel
            {
                Address = "123 Street",
                City = "City",
                PostalCode = "12345",
                Country = "Country"
            };

            var userId = "user123";

            var order = new Order
            {
                UserId = userId,
                OrderId = 1,
                ShippingDetail = null // No shipping detail initially
            };

            var shippingDetail = new ShippingDetail
            {
                ShippingDetailId = 1, // Assume this ID is generated after insertion
                Address = model.Address,
                City = model.City,
                PostalCode = model.PostalCode,
                Country = model.Country
            };

            // Mock repositories
            mockOrderRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(order);
            mockShippingDetailRepository.Setup(repo => repo.AddAsync(It.IsAny<ShippingDetail>())).Returns(Task.CompletedTask);
            mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<ProductOrder>().AsQueryable());

            // Act
            await shippingDetailService.CreateShippingDetailAsync(model, userId);

            // Assert
            Assert.That(order.ShippingDetailId, Is.EqualTo(shippingDetail.ShippingDetailId));
            mockOrderRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Order>()), Times.Once);
        }

        [Test]
        public async Task CreateShippingDetailAsync_ShouldRemoveRelatedProductOrders_WhenOrderExists()
        {
            // Arrange
            var model = new ShippingDetailViewModel
            {
                Address = "123 Street",
                City = "City",
                PostalCode = "12345",
                Country = "Country"
            };

            var userId = "user123";

            var order = new Order
            {
                UserId = userId,
                OrderId = 1,
                ShippingDetail = null // No shipping detail initially
            };

            var productOrderList = new List<ProductOrder>
        {
            new ProductOrder { OrderId = 1, ProductId = 1 },
            new ProductOrder { OrderId = 1, ProductId = 2 }
        };

            // Mock repositories
            mockOrderRepository.Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(order);
            mockShippingDetailRepository.Setup(repo => repo.AddAsync(It.IsAny<ShippingDetail>())).Returns(Task.CompletedTask);
            mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(productOrderList.AsQueryable());

            // Act
            await shippingDetailService.CreateShippingDetailAsync(model, userId);

            // Assert
            mockProductOrderRepository.Verify(repo => repo.RemoveAsync(It.IsAny<ProductOrder>()), Times.Exactly(productOrderList.Count)); // Verify RemoveAsync is called for each product order
        }
    }
}
