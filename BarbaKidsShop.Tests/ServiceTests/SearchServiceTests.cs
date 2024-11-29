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
    public class SearchServiceTests
    {
        private Mock<IRepository<Product, int>> _mockProductRepository;
        private SearchService _searchService;

        [SetUp]
        public void SetUp()
        {
            // Initialize the mock repository
            _mockProductRepository = new Mock<IRepository<Product, int>>();

            // Initialize the service with the mocked repository
            _searchService = new SearchService(_mockProductRepository.Object);
        }

        [Test]
        public async Task FindAProductByNameAsync_ShouldReturnProducts_WhenProductsMatchName()
        {
            // Arrange
            var productName = "TestProduct";
            var products = new List<Product>
        {
            new Product { Id = 1, ProductName = "TestProduct", Price = 10.99m, ImageUrl = "image1.jpg", IsDeleted = false },
            new Product { Id = 2, ProductName = "TestProduct", Price = 15.49m, ImageUrl = "image2.jpg", IsDeleted = false }
        }.AsQueryable();

            // Setup the mock
            _mockProductRepository.Setup(repo => repo.GetAllAttached()).Returns(products);

            // Act
            var result = await _searchService.FindAProductByNameAsync(productName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().ProductName, Is.EqualTo("TestProduct"));
            _mockProductRepository.Verify(repo => repo.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task FindAProductByNameAsync_ShouldReturnEmpty_WhenNoProductsMatchName()
        {
            // Arrange
            var productName = "NonExistentProduct";
            var products = new List<Product>
        {
            new Product { Id = 1, ProductName = "AnotherProduct", Price = 10.99m, ImageUrl = "image1.jpg", IsDeleted = false }
        }.AsQueryable();

            // Setup the mock
            _mockProductRepository.Setup(repo => repo.GetAllAttached()).Returns(products);

            // Act
            var result = await _searchService.FindAProductByNameAsync(productName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            _mockProductRepository.Verify(repo => repo.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task FindAProductByNameAsync_ShouldIgnoreDeletedProducts()
        {
            // Arrange
            var productName = "TestProduct";
            var products = new List<Product>
        {
            new Product { Id = 1, ProductName = "TestProduct", Price = 10.99m, ImageUrl = "image1.jpg", IsDeleted = true },
            new Product { Id = 2, ProductName = "TestProduct", Price = 15.49m, ImageUrl = "image2.jpg", IsDeleted = false }
        }.AsQueryable();

            // Setup the mock
            _mockProductRepository.Setup(repo => repo.GetAllAttached()).Returns(products);

            // Act
            var result = await _searchService.FindAProductByNameAsync(productName);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(2));
            _mockProductRepository.Verify(repo => repo.GetAllAttached(), Times.Once);
        }
    }

}
