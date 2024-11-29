using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Data.Repository.Interfaces;
using BarbaKidsShop.Services.Data;
using Moq;
using NUnit.Framework;

namespace BarbaKidsShop.Tests.ServiceTests
{
    [TestFixture]
    public class CartServiceTests
    {
        private Mock<IRepository<Order, int>> mockOrderRepository;
        private Mock<IRepository<ProductOrder, int>> mockProductOrderRepository;
        private Mock<IRepository<Product, int>> mockProductRepository;
        private CartService cartService;

        [SetUp]
        public void SetUp()
        {
            mockOrderRepository = new Mock<IRepository<Order, int>>();
            mockProductOrderRepository = new Mock<IRepository<ProductOrder, int>>();
            mockProductRepository = new Mock<IRepository<Product, int>>();

            cartService = new CartService(
                mockOrderRepository.Object,
                mockProductOrderRepository.Object,
                mockProductRepository.Object);
        }

        [Test]
        public async Task IndexGetAllProductsForUserInCartAsync_ShouldReturnProducts_WhenUserHasItemsInCart()
        {
            // Arrange
            var userId = "user123";
            var cartItems = new List<ProductOrder>
        {
            new ProductOrder
            {
                ProductId = 1,
                Quantity = 2,
                Product = new Product { ProductName = "Product1", Price = 10.00M, IsDeleted = false },
                Order = new Order { UserId = userId }
            },
            new ProductOrder
            {
                ProductId = 2,
                Quantity = 1,
                Product = new Product { ProductName = "Product2", Price = 20.00M, IsDeleted = false },
                Order = new Order { UserId = userId }
            }
        }.AsQueryable();

            mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(cartItems);

            // Act
            var result = await cartService.IndexGetAllProductsForUserInCartAsync(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().ProductName, Is.EqualTo("Product1"));
        }

        [Test]
        public async Task AddToCartAsync_ShouldAddNewItem_WhenItemNotInCart()
        {
            // Arrange
            var userId = "user123";
            var productId = 1;
            var quantity = 2;
            var product = new Product { Id = productId, ProductName = "Product1", Price = 10.00M };
            var order = new Order { UserId = userId, OrderId = 1 };

            mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            mockOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<Order> { order }.AsQueryable());
            mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<ProductOrder>().AsQueryable());

            // Act
            await cartService.AddToCartAsync(userId, productId, quantity);

            // Assert
            mockProductOrderRepository.Verify(repo => repo.AddAsync(It.Is<ProductOrder>(po => po.ProductId == productId && po.Quantity == quantity)), Times.Once);
            mockOrderRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Order>()), Times.Once);
        }

        [Test]
        public async Task AddToCartAsync_ShouldUpdateQuantity_WhenItemAlreadyInCart()
        {
            // Arrange
            var userId = "user123";
            var productId = 1;
            var quantity = 2;
            var product = new Product { Id = productId, ProductName = "Product1", Price = 10.00M };
            var order = new Order { UserId = userId, OrderId = 1 };
            var productOrder = new ProductOrder { ProductId = productId, Quantity = 1, OrderId = order.OrderId };

            mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            mockOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<Order> { order }.AsQueryable());
            mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<ProductOrder> { productOrder }.AsQueryable());

            // Act
            await cartService.AddToCartAsync(userId, productId, quantity);

            // Assert
            Assert.That(productOrder.Quantity, Is.EqualTo(3));
            mockProductOrderRepository.Verify(repo => repo.UpdateAsync(It.IsAny<ProductOrder>()), Times.Once);
            mockOrderRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Order>()), Times.Once);
        }

        [Test]
        public void AddToCartAsync_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var userId = "user123";
            var productId = 1;
            var quantity = 1;

            mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => cartService.AddToCartAsync(userId, productId, quantity));
            Assert.That(ex.Message, Is.EqualTo("Product not found."));
        }

        [Test]
        public async Task ClearCartAsync_ShouldRemoveAllItems_WhenOrderExists()
        {
            // Arrange
            var userId = "user123";
            var order = new Order { UserId = userId, OrderId = 1 };
            var productOrders = new List<ProductOrder>
        {
            new ProductOrder { ProductId = 1, OrderId = order.OrderId },
            new ProductOrder { ProductId = 2, OrderId = order.OrderId }
        };

            mockOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<Order> { order }.AsQueryable());
            mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(productOrders.AsQueryable());

            // Act
            await cartService.ClearCartAsync(userId);

            // Assert
            mockProductOrderRepository.Verify(repo => repo.RemoveAsync(It.IsAny<ProductOrder>()), Times.Exactly(productOrders.Count));
        }

        [Test]
        public void ClearCartAsync_ShouldThrowException_WhenOrderNotFound()
        {
            // Arrange
            var userId = "user123";

            mockOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<Order>().AsQueryable());

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => cartService.ClearCartAsync(userId));
            Assert.That(ex.Message, Is.EqualTo("Order not found."));
        }

        [Test]
        public async Task RemoveFromCartAsync_ShouldRemoveItem_WhenItemExists()
        {
            // Arrange
            var userId = "user123";
            var productId = 1;
            var order = new Order { UserId = userId, OrderId = 1 };
            var productOrder = new ProductOrder { ProductId = productId, OrderId = order.OrderId };

            mockOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<Order> { order }.AsQueryable());
            mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<ProductOrder> { productOrder }.AsQueryable());

            // Act
            await cartService.RemoveFromCartAsync(productId, userId);

            // Assert
            mockProductOrderRepository.Verify(repo => repo.RemoveAsync(It.Is<ProductOrder>(po => po.ProductId == productId)), Times.Once);
        }

        [Test]
        public void RemoveFromCartAsync_ShouldThrowException_WhenItemNotFound()
        {
            // Arrange
            var userId = "user123";
            var productId = 1;
            var order = new Order { UserId = userId, OrderId = 1 };

            mockOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<Order> { order }.AsQueryable());
            mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<ProductOrder>().AsQueryable());

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => cartService.RemoveFromCartAsync(productId, userId));
            Assert.That(ex.Message, Is.EqualTo("Item not found."));
        }
    }
}
