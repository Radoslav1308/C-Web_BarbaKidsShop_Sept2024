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
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;

namespace BarbaKidsShop.Tests.ServiceTests
{
    [TestFixture]
    public class SearchServiceTests
    {
        private ApplicationDbContext _context;
        private SearchService _searchService;
        private IRepository<Product, int> _productRepository;

        [SetUp]
        public void Setup()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Initialize the context
            _context = new ApplicationDbContext(options);
            _productRepository = new BaseRepository<Product, int>(_context); // In-memory repository for Product
            _searchService = new SearchService(_productRepository); // Inject repository into the service

            // Seed the database with products
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Seed with some products
            var products = new List<Product>
            {
                new Product { Id = 1, ProductName = "Toy Car", Price = 10.99m, ImageUrl = "car.jpg", Description = "A small toy car for children", IsDeleted = false },
                new Product { Id = 2, ProductName = "Toy Train", Price = 12.99m, ImageUrl = "train.jpg", Description = "A miniature train with colorful carriages", IsDeleted = false },
                new Product { Id = 3, ProductName = "Toy Truck", Price = 14.99m, ImageUrl = "truck.jpg", Description = "A large toy truck with moving parts", IsDeleted = true },
                new Product { Id = 4, ProductName = "Doll", Price = 8.99m, ImageUrl = "doll.jpg", Description = "A soft and cuddly doll", IsDeleted = false },
                new Product { Id = 5, ProductName = "Toy Car", Price = 11.99m, ImageUrl = "car2.jpg", Description = "A more advanced version of the toy car", IsDeleted = false },
                new Product { Id = 6, ProductName = "Toy Train", Price = 13.99m, ImageUrl = "train2.jpg", Description = "A larger toy train for collectors", IsDeleted = false }
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        [Test]
        public async Task FindAProductByNameAsync_ShouldReturnCorrectProducts_WhenProductNameExists()
        {
            // Arrange
            string searchTerm = "Toy Car";

            // Act
            var result = await _searchService.FindAProductByNameAsync(searchTerm);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2)); // There are 2 products with the name "Toy Car"
            Assert.That(result.All(p => p.ProductName == "Toy Car")); // All returned products should have "Toy Car" as the name
        }

        [Test]
        public async Task FindAProductByNameAsync_ShouldThrowException_WhenProductNameDoesNotExist()
        {
            // Arrange
            string searchTerm = "Nonexistent Toy";

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await _searchService.FindAProductByNameAsync(searchTerm));
            Assert.That("Products not found.", Is.EqualTo(ex.Message)); // Exception message should be "Products not found."
        }

        [Test]
        public async Task FindAProductByNameAsync_ShouldExcludeDeletedProducts()
        {
            // Arrange
            string searchTerm = "Toy Truck"; // "Toy Truck" is marked as deleted

            // Act & Assert
            
            var ex = Assert.ThrowsAsync<Exception>(async () => await _searchService.FindAProductByNameAsync(searchTerm));
            Assert.That("Products not found.", Is.EqualTo(ex.Message)); // Exception message should be "Products not found."
        }

        [Test]
        public async Task FindAProductByNameAsync_ShouldReturnCorrectProducts_ExcludingDeleted_WhenMultipleWithSameNameExist()
        {
            // Arrange
            string searchTerm = "Toy Train";

            // Act
            var result = await _searchService.FindAProductByNameAsync(searchTerm);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2)); // Only 2 "Toy Train" products should be returned (excluding the deleted one)
            Assert.That(result.All(p => p.ProductName == "Toy Train")); // All returned products should have "Toy Train" as the name
        }
    }
}
