using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Data;
using BarbaKidsShop.Data.Models;
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
        private CartService _cartService;
        private ApplicationDbContext _context;
        private IRepository<Order, int> _orderRepository;
        private IRepository<Product, int> _productRepository;       
        private IRepository<ProductOrder, int> _productOrderRepository;

        private string _validUserId = "d402e413-4d10-4a92-8eab-c1eec022e8bc";
        private string _invalidUserId = "invalidUserId";

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestCartDatabase") // In-memory database for testing
                .Options;

            _context = new ApplicationDbContext(options);
            _cartService = new CartService(_orderRepository, _productOrderRepository, _productRepository);

            // Seed the database with test data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Add products and cart items
            var product1 = new Product
            {
                Id = 1,
                ProductName = "Product 1",
                Price = 100,
                IsDeleted = false,
                ImageUrl = "image1.jpg",
                Description = "Product 1 Description"
            };

            var product2 = new Product
            {
                Id = 2,
                ProductName = "Product 2",
                Price = 200,
                IsDeleted = true,  // This product is marked as deleted
                ImageUrl = "image2.jpg",
                Description = "Product 2 Description"
            };

            var order1 = new Order
            {
                OrderId = 1,
                UserId = _validUserId,
            };

            var order2 = new Order
            {
                OrderId = 2,
                UserId = _invalidUserId, // Invalid user
            };

            _context.Products.AddRange(product1, product2);
            _context.Orders.AddRange(order1, order2);
            _context.ProductOrders.AddRange(
                new ProductOrder { Order = order1, Product = product1, Quantity = 1, ProductId = 1 },
                new ProductOrder { Order = order1, Product = product2, Quantity = 2, ProductId = 2 },
                new ProductOrder { Order = order2, Product = product1, Quantity = 3, ProductId = 1 }
            );

            _context.SaveChanges();
        }

        //[Test]
        //public async Task IndexGetAllProductsForUserInCartAsync_ShouldReturnCartItems_ForValidUser()
        //{
        //    // Act
        //    var result = await _cartService.IndexGetAllProductsForUserInCartAsync(_validUserId);

        //    // Assert
        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.Count(), Is.EqualTo(2)); // Should return 2 items (Product 1 and Product 2)
        //    Assert.That(result.Any(item => item.ProductName == "Product 1"));
        //    Assert.That(result.Any(item => item.ProductName == "Product 2"));
        //}

        //[Test]
        //public async Task IndexGetAllProductsForUserInCartAsync_ShouldReturnEmpty_ForInvalidUser()
        //{
        //    // Act
        //    var result = await _cartService.IndexGetAllProductsForUserInCartAsync(_invalidUserId);

        //    // Assert
        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.Count(), Is.EqualTo(0)); // Should return 0 items for invalid user
        //}

        //[Test]
        //public async Task IndexGetAllProductsForUserInCartAsync_ShouldExcludeDeletedProducts()
        //{
        //    // Act
        //    var result = await _cartService.IndexGetAllProductsForUserInCartAsync(_validUserId);

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(1, result.Count()); // Should return only 1 item because Product 2 is deleted
        //    Assert.IsTrue(result.Any(item => item.ProductName == "Product 1"));
        //    Assert.IsFalse(result.Any(item => item.ProductName == "Product 2"));
        //}

        //[Test]
        //public async Task IndexGetAllProductsForUserInCartAsync_ShouldReturnProducts_WhenUserHasItemsInCart()
        //{
        //    // Arrange
        //    var userId = "user123";
        //    var cartItems = new List<ProductOrder>
        //{
        //    new ProductOrder
        //    {
        //        ProductId = 1,
        //        Quantity = 2,
        //        Product = new Product { ProductName = "Product1", Price = 10.00M, IsDeleted = false },
        //        Order = new Order { UserId = userId }
        //    },
        //    new ProductOrder
        //    {
        //        ProductId = 2,
        //        Quantity = 1,
        //        Product = new Product { ProductName = "Product2", Price = 20.00M, IsDeleted = false },
        //        Order = new Order { UserId = userId }
        //    }
        //}.AsQueryable();

        //    mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(cartItems);

        //    // Act
        //    var result = await cartService.IndexGetAllProductsForUserInCartAsync(userId);

        //    // Assert
        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.Count(), Is.EqualTo(2));
        //    Assert.That(result.First().ProductName, Is.EqualTo("Product1"));
        //}

        //[Test]
        //public async Task AddToCartAsync_ShouldAddNewItem_WhenItemNotInCart()
        //{
        //    // Arrange
        //    var userId = "user123";
        //    var productId = 1;
        //    var quantity = 2;
        //    var product = new Product { Id = productId, ProductName = "Product1", Price = 10.00M };
        //    var order = new Order { UserId = userId, OrderId = 1 };

        //    mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
        //    mockOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<Order> { order }.AsQueryable());
        //    mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<ProductOrder>().AsQueryable());

        //    // Act
        //    await cartService.AddToCartAsync(userId, productId, quantity);

        //    // Assert
        //    mockProductOrderRepository.Verify(repo => repo.AddAsync(It.Is<ProductOrder>(po => po.ProductId == productId && po.Quantity == quantity)), Times.Once);
        //    mockOrderRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Order>()), Times.Once);
        //}

        //[Test]
        //public async Task AddToCartAsync_ShouldUpdateQuantity_WhenItemAlreadyInCart()
        //{
        //    // Arrange
        //    var userId = "user123";
        //    var productId = 1;
        //    var quantity = 2;
        //    var product = new Product { Id = productId, ProductName = "Product1", Price = 10.00M };
        //    var order = new Order { UserId = userId, OrderId = 1 };
        //    var productOrder = new ProductOrder { ProductId = productId, Quantity = 1, OrderId = order.OrderId };

        //    mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
        //    mockOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<Order> { order }.AsQueryable());
        //    mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<ProductOrder> { productOrder }.AsQueryable());

        //    // Act
        //    await cartService.AddToCartAsync(userId, productId, quantity);

        //    // Assert
        //    Assert.That(productOrder.Quantity, Is.EqualTo(3));
        //    mockProductOrderRepository.Verify(repo => repo.UpdateAsync(It.IsAny<ProductOrder>()), Times.Once);
        //    mockOrderRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Order>()), Times.Once);
        //}

        //[Test]
        //public void AddToCartAsync_ShouldThrowException_WhenProductNotFound()
        //{
        //    // Arrange
        //    var userId = "user123";
        //    var productId = 1;
        //    var quantity = 1;

        //    mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync((Product)null);

        //    // Act & Assert
        //    var ex = Assert.ThrowsAsync<ArgumentException>(() => cartService.AddToCartAsync(userId, productId, quantity));
        //    Assert.That(ex.Message, Is.EqualTo("Product not found."));
        //}

        //[Test]
        //public async Task ClearCartAsync_ShouldRemoveAllItems_WhenOrderExists()
        //{
        //    // Arrange
        //    var userId = "user123";
        //    var order = new Order { UserId = userId, OrderId = 1 };
        //    var productOrders = new List<ProductOrder>
        //{
        //    new ProductOrder { ProductId = 1, OrderId = order.OrderId },
        //    new ProductOrder { ProductId = 2, OrderId = order.OrderId }
        //};

        //    mockOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<Order> { order }.AsQueryable());
        //    mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(productOrders.AsQueryable());

        //    // Act
        //    await cartService.ClearCartAsync(userId);

        //    // Assert
        //    mockProductOrderRepository.Verify(repo => repo.RemoveAsync(It.IsAny<ProductOrder>()), Times.Exactly(productOrders.Count));
        //}

        //[Test]
        //public void ClearCartAsync_ShouldThrowException_WhenOrderNotFound()
        //{
        //    // Arrange
        //    var userId = "user123";

        //    mockOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<Order>().AsQueryable());

        //    // Act & Assert
        //    var ex = Assert.ThrowsAsync<ArgumentException>(() => cartService.ClearCartAsync(userId));
        //    Assert.That(ex.Message, Is.EqualTo("Order not found."));
        //}

        //[Test]
        //public async Task RemoveFromCartAsync_ShouldRemoveItem_WhenItemExists()
        //{
        //    // Arrange
        //    var userId = "user123";
        //    var productId = 1;
        //    var order = new Order { UserId = userId, OrderId = 1 };
        //    var productOrder = new ProductOrder { ProductId = productId, OrderId = order.OrderId };

        //    mockOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<Order> { order }.AsQueryable());
        //    mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<ProductOrder> { productOrder }.AsQueryable());

        //    // Act
        //    await cartService.RemoveFromCartAsync(productId, userId);

        //    // Assert
        //    mockProductOrderRepository.Verify(repo => repo.RemoveAsync(It.Is<ProductOrder>(po => po.ProductId == productId)), Times.Once);
        //}

        //[Test]
        //public void RemoveFromCartAsync_ShouldThrowException_WhenItemNotFound()
        //{
        //    // Arrange
        //    var userId = "user123";
        //    var productId = 1;
        //    var order = new Order { UserId = userId, OrderId = 1 };

        //    mockOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<Order> { order }.AsQueryable());
        //    mockProductOrderRepository.Setup(repo => repo.GetAllAttached()).Returns(new List<ProductOrder>().AsQueryable());

        //    // Act & Assert
        //    var ex = Assert.ThrowsAsync<ArgumentException>(() => cartService.RemoveFromCartAsync(productId, userId));
        //    Assert.That(ex.Message, Is.EqualTo("Item not found."));
        //}
    }
}
