using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Data;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Data.Repository;
using BarbaKidsShop.Data.Repository.Interfaces;
using BarbaKidsShop.Services.Data;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace BarbaKidsShop.Tests.ServiceTests
{
    [TestFixture]
    public class CartServiceTests
    {
        private ApplicationDbContext _context;
        private CartService _cartService;
        private IRepository<ProductOrder, int> _productOrderRepository;
        private IRepository<Order, int> _orderRepository;
        private IRepository<Product, int> _productRepository;

        [SetUp]
        public void Setup()
        {
            // Configure the in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_ShoppingCart")
                .Options;

            _context = new ApplicationDbContext(options);
            _productOrderRepository = new BaseRepository<ProductOrder, int>(_context);
            _orderRepository = new BaseRepository<Order, int>(_context);
            _productRepository = new BaseRepository<Product, int>(_context);
            _cartService = new CartService(_orderRepository, _productOrderRepository, _productRepository);

            // Seed the database
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var userId = "test-user-1";

            var order = new Order
            {
                OrderId = 1,
                UserId = userId,
            };

            var product1 = new Product
            {
                Id = 1,
                ProductName = "Product 1",
                Price = 10.99m,
                Description = "Description 1",
                IsDeleted = false,
            };

            var product2 = new Product
            {
                Id = 2,
                ProductName = "Product 2",
                Description = "Description 2",
                Price = 20.50m,
                IsDeleted = false,
            };

            var productOrder1 = new ProductOrder
            {
                ProductOrderId = 1,
                OrderId = order.OrderId,
                ProductId = product1.Id,
                Quantity = 2,
            };

            var productOrder2 = new ProductOrder
            {
                ProductOrderId = 2,
                OrderId = order.OrderId,
                ProductId = product2.Id,
                Quantity = 1,
            };

            _context.Orders.Add(order);
            _context.Products.AddRange(product1, product2);
            _context.ProductOrders.AddRange(productOrder1, productOrder2);

            // Additional products or orders for other tests
            var additionalProduct = new Product
            {
                Id = 3,
                ProductName = "Non-Existent Product",
                Price = 15.00m,
                Description = "This product will be used for non-existing tests",
                IsDeleted = true, // Simulate a deleted product
            };

            var newUserOrder = new Order
            {
                OrderId = 2,
                UserId = "new-user",
                TotalPrice = 0,
            };

            _context.Products.Add(additionalProduct);
            _context.Orders.Add(newUserOrder);

            _context.SaveChanges();
        }

        [Test]
        public async Task IndexGetAllProductsForUserInCartAsync_ShouldReturnEmptyList_WhenNoProductsForUser()
        {
            // Arrange
            var userId = "non-existent-user";

            // Act
            var result = await _cartService.IndexGetAllProductsForUserInCartAsync(userId);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task IndexGetAllProductsForUserInCartAsync_ShouldExcludeDeletedProducts()
        {
            // Arrange
            var userId = "test-user-1";
            var product = _context.Products.First(p => p.Id == 1);
            product.IsDeleted = true;
            await _context.SaveChangesAsync();

            // Act
            var result = await _cartService.IndexGetAllProductsForUserInCartAsync(userId);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1)); // Only one product should remain
            Assert.That(result.First().ProductName, Is.EqualTo("Product 2"));
        }

        [Test]
        public async Task AddToCartAsync_ShouldCreateNewOrderForUserWithoutOrder()
        {
            // Arrange
            var userId = "new-user";
            var productId = 1;
            var quantity = 3;

            // Act
            await _cartService.AddToCartAsync(userId, productId, quantity);

            var newOrder = _context.Orders.FirstOrDefault(o => o.UserId == userId);
            var productOrder = _context.ProductOrders.FirstOrDefault(po => po.OrderId == newOrder.OrderId && po.ProductId == productId);

            // Assert
            Assert.That(newOrder, Is.Not.Null);
            Assert.That(productOrder, Is.Not.Null);
            Assert.That(productOrder.Quantity, Is.EqualTo(quantity));
        }

        [Test]
        public void AddToCartAsync_ShouldThrowException_ForDeletedProduct()
        {
            // Arrange
            var userId = "test-user-1";
            var deletedProductId = 3; // Deleted product
            var quantity = 1;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(() => _cartService.AddToCartAsync(userId, deletedProductId, quantity));
            Assert.That(ex.Message, Is.EqualTo("Product not found."));
        }

        [Test]
        public async Task ClearCartAsync_ShouldClearAllCartItems_ForValidUser()
        {
            // Arrange
            var userId = "test-user-1";

            // Act
            await _cartService.ClearCartAsync(userId);

            // Assert
            var remainingItems = _context.ProductOrders.Where(po => po.OrderId == 1).ToList();
            Assert.That(remainingItems, Is.Empty);
        }

        [Test]
        public void ClearCartAsync_ShouldThrowException_ForNonExistingUser()
        {
            // Arrange
            var nonExistingUserId = "non-existing-user";

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _cartService.ClearCartAsync(nonExistingUserId));

            Assert.That(ex.Message, Is.EqualTo("Order not found."));
        }

        [Test]
        public async Task ClearCartAsync_ShouldDoNothing_WhenNoProductOrders()
        {
            // Arrange
            var userId = "new-userId";
            var emptyOrder = new Order
            {
                OrderId = 6,
                UserId = userId
            };
            _context.Orders.Add(emptyOrder);
            _context.SaveChanges();

            // Act
            await _cartService.ClearCartAsync(userId);

            // Assert
            var remainingOrders = _context.ProductOrders.Where(po => po.OrderId == 2).ToList();
            Assert.That(remainingOrders, Is.Empty); // No products should remain
        }

        [Test]
        public async Task RemoveFromCartAsync_ShouldRemoveItem_ForValidProductAndUser()
        {
            // Arrange
            var userId = "test-user-1";
            var productId = 1;

            // Act
            await _cartService.RemoveFromCartAsync(productId, userId);

            // Assert
            var remainingItems = _context.ProductOrders
                .Where(po => po.OrderId == 1 && po.ProductId == productId)
                .ToList();
            Assert.That(remainingItems, Is.Empty);
        }

        [Test]
        public void RemoveFromCartAsync_ShouldThrowException_ForNonExistingProduct()
        {
            // Arrange
            var userId = "test-user-1";
            var nonExistingProductId = 999;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _cartService.RemoveFromCartAsync(nonExistingProductId, userId));

            Assert.That(ex.Message, Is.EqualTo("Item not found."));
        }

        [Test]
        public void RemoveFromCartAsync_ShouldThrowException_ForNonExistingUser()
        {
            // Arrange
            var nonExistingUserId = "non-existing-user";
            var productId = 1;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _cartService.RemoveFromCartAsync(productId, nonExistingUserId));

            Assert.That(ex.Message, Is.EqualTo("Order not found."));
        }

        [Test]
        public void RemoveFromCartAsync_ShouldThrowException_WhenCartIsEmpty()
        {
            // Arrange
            var userId = "new-user";
            var productId = 1;

            var emptyOrder = new Order
            {
                OrderId = 4,
                UserId = userId
            };
            _context.Orders.Add(emptyOrder);
            _context.SaveChanges();

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _cartService.RemoveFromCartAsync(productId, userId));

            Assert.That(ex.Message, Is.EqualTo("Item not found."));
        }
    }
}
