using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProductServiceTests
    {
        private Mock<IRepository<Product, int>> mockProductRepository;
        private Mock<IRepository<Category, int>> mockCategoryRepository;
        private ProductService productService;

        [SetUp]
        public void SetUp()
        {
            mockProductRepository = new Mock<IRepository<Product, int>>();
            mockCategoryRepository = new Mock<IRepository<Category, int>>();
            productService = new ProductService(mockProductRepository.Object, mockCategoryRepository.Object);
        }

        [Test]
        public async Task GetAddProductModelByIdAsync_ReturnsValidModel()
        {
            // Arrange
            var mockCategories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Clothes" },
                new Category { CategoryId = 2, Name = "Toys" },
                new Category { CategoryId = 3, Name = "Shoes" },
                new Category { CategoryId = 4, Name = "Accessories" },
                new Category { CategoryId = 5, Name = "Educational Materials" }
            }.AsQueryable();

            mockCategoryRepository.Setup(repo => repo.GetAllAttached()).Returns(mockCategories);

            var service = new ProductService(mockProductRepository.Object, mockCategoryRepository.Object);

            // Act
            var result = await service.GetAddProductModelByIdAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Categories.Count, Is.EqualTo(5)); // Check if there are 5 categories returned
            Assert.That(result.Categories.First().Name, Is.EqualTo("Clothes")); // Check if first category is 'Clothes'
        }

        [Test]
        public async Task AddProductAsync_ShouldAddProduct_WhenValidModel()
        {
            // Arrange
            var model = new ProductViewModel
            {
                ProductName = "Test Product",
                Description = "Test Description",
                Price = 100.00M,
                ImageUrl = "http://example.com/image.jpg",
                AddedOn = DateTime.Now,
                CategoryId = 1,
                IsDeleted = false
            };

            mockProductRepository.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            await productService.AddProductAsync(model);

            // Assert
            mockProductRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
        }

        [Test]
        public async Task GetEditProductModelByIdAsync_ShouldReturnProductEditModel_WhenProductExists()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Test Product",
                Description = "Test Description",
                Price = 100.00M,
                ImageUrl = "http://example.com/image.jpg",
                CategoryId = 1
            };

            mockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            mockCategoryRepository.Setup(r => r.GetAllAttached()).Returns(new List<Category>
            {
                new Category { CategoryId = 1, Name = "Clothes" },
                new Category { CategoryId = 2, Name = "Toys" },
                new Category { CategoryId = 3, Name = "Shoes" },
                new Category { CategoryId = 4, Name = "Accessories" },
                new Category { CategoryId = 5, Name = "Educational Materials" }
            }.AsQueryable());

            // Act
            var result = await productService.GetEditProductModelByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(product.Id)); // Ensure Id is the same
            Assert.That(result.ProductName, Is.EqualTo(product.ProductName)); // Ensure ProductName is the same
            Assert.That(result.Description, Is.EqualTo(product.Description)); // Ensure Description is the same
        }

        [Test]
        public async Task GetProductDetailsByIdAsync_ShouldReturnProductDetails_WhenProductExists()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Test Product",
                Description = "Test Description",
                Price = 100.00M,
                ImageUrl = "http://example.com/image.jpg",
                CategoryId = 1,
                Category = new Category { CategoryId = 1, Name = "Category1" }
            };

            mockProductRepository.Setup(r => r.GetAllAttached()).Returns(new List<Product> { product }.AsQueryable());

            // Act
            var result = await productService.GetProductDetailsByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductName, Is.EqualTo(product.ProductName));
            Assert.That(result.CategoryName, Is.EqualTo(product.Category.Name)); // Ensure CategoryName matches the product's category
        }

        //[Test]
        //public async Task IndexGetAllProductsOrderedByPriceAsync_ShouldReturnOrderedProducts()
        //{
        //    // Arrange
        //    var products = new List<Product>
        //    {
        //        new Product { Id = 1, ProductName = "Product1", Price = 50, IsDeleted = false },
        //        new Product { Id = 2, ProductName = "Product2", Price = 150, IsDeleted = false }
        //    };

        //    mockProductRepository.Setup(r => r.GetAllAttached()).Returns(products.AsQueryable());

        //    // Act
        //    var result = await productService.IndexGetAllProductsOrderedByPriceAsync();

        //    // Assert
        //    Assert.That(result, Is.Not.Null);
        //    Assert.That(result.Count(), Is.EqualTo(2)); // Ensure we get two products
        //    Assert.That(result.First().ProductName, Is.EqualTo("Product1")); // Check if the first product is "Product1"
        //    Assert.That(result.Last().ProductName, Is.EqualTo("Product2")); // Check if the last product is "Product2"
        //}

        [Test]
        public async Task UpdateProductAsync_ShouldUpdateProduct_WhenValidModel()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Old Product",
                Description = "Old Description",
                Price = 50.00M,
                ImageUrl = "http://oldexample.com/image.jpg",
                CategoryId = 1
            };

            var model = new ProductEditViewModel
            {
                Id = 1,
                ProductName = "Updated Product",
                Description = "Updated Description",
                Price = 60.00M,
                ImageUrl = "http://newexample.com/image.jpg",
                CategoryId = 2
            };

            mockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            // Simulate the update behavior
            mockProductRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
                .Returns(Task.FromResult(true)); // Simulate a successful update

            // Act
            await productService.UpdateProductAsync(model);

            // Assert
            Assert.That(product.ProductName, Is.EqualTo(model.ProductName));
            Assert.That(product.Price, Is.EqualTo(model.Price));
            Assert.That(product.Description, Is.EqualTo(model.Description));
            Assert.That(product.ImageUrl, Is.EqualTo(model.ImageUrl));
            Assert.That(product.CategoryId, Is.EqualTo(model.CategoryId));

            // Verify UpdateAsync was called
            mockProductRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Test]
        public async Task GetProductDeleteByIdAsync_ShouldReturnProductDeleteModel_WhenProductExists()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Test Product"
            };

            mockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await productService.GetProductDeleteByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(product.ProductName, Is.EqualTo(result.ProductName));
        }

        [Test]
        public async Task SoftDeleteProductAsync_ShouldMarkProductAsDeleted_WhenProductExists()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Test Product",
                IsDeleted = false // Initial state
            };

            var model = new ProductDeleteViewModel { Id = 1 };

            // Setup mock to return the product when queried
            mockProductRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            // Simulate the state change when UpdateAsync is called
            mockProductRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
                .Returns(Task.FromResult(true)); // Simulate a successful update
            // Act
            await productService.SoftDeleteProductAsync(model);

            // Assert
            Assert.That(product.IsDeleted, Is.True); // Assert the state change
            mockProductRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once); // Ensure UpdateAsync was called once
        }

        [Test]
        public async Task SoftDeleteProductAsync_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var model = new ProductDeleteViewModel { Id = 999 }; // Non-existent product

            mockProductRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () => await productService.SoftDeleteProductAsync(model));
            Assert.That("Product not found.", Is.EqualTo(ex.Message));
        }
    }
}
