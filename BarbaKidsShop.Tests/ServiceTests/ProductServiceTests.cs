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
using BarbaKidsShop.Services.Data.Interfaces;
using BarbaKidsShop.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace BarbaKidsShop.Tests.ServiceTests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private ApplicationDbContext _context;
        private ProductService _productService;
        private IRepository<Product, int> _productRepository;
        private IRepository<Category, int> _categoryRepository;

        [SetUp]
        public void Setup()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Initialize the context
            _context = new ApplicationDbContext(options);
            _productRepository = new BaseRepository<Product, int>(_context);  // In-memory repository for Product
            _categoryRepository = new BaseRepository<Category, int>(_context); // In-memory repository for Category
            _productService = new ProductService(_productRepository, _categoryRepository); // Inject repositories into the service

            // Seed the database with categories
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Add categories to the in-memory database
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { CategoryId = 1, Name = "Toys" });
                _context.Categories.Add(new Category { CategoryId = 2, Name = "Clothing" });
                _context.Categories.Add(new Category { CategoryId = 3, Name = "Books" });
                _context.SaveChanges();
            }

            // Add products to the in-memory database
            if (!_context.Products.Any())
            {
                _context.Products.Add(new Product
                {
                    Id = 1,
                    ProductName = "Car",
                    Description = "Fast as the light",
                    Price = 15.99m,
                    ImageUrl = "http://example.com/car.jpg",
                    CategoryId = 1,
                    IsDeleted = false,
                    AddedOn = DateTime.UtcNow
                });

                _context.Products.Add(new Product
                {
                    Id = 2,
                    ProductName = "T-Shirt",
                    Description = "Cotton T-shirt",
                    Price = 19.99m,
                    ImageUrl = "http://example.com/tshirt.jpg",
                    CategoryId = 2,
                    IsDeleted = false,
                    AddedOn = DateTime.UtcNow
                });

                _context.Products.Add(new Product
                {
                    Id = 3,
                    ProductName = "Product 3",
                    Description = "Product 3 Desc",
                    Price = 10.99m,
                    ImageUrl = "http://example.com/product3.jpg",
                    CategoryId = 1,
                    IsDeleted = false,
                    AddedOn = DateTime.UtcNow
                });

                _context.Products.Add(new Product
                {
                    Id = 4,
                    ProductName = "Product 4",
                    Description = "Product 4 Desc",
                    Price = 39.99m,
                    ImageUrl = "http://example.com/product4.jpg",
                    CategoryId = 2,
                    IsDeleted = false,
                    AddedOn = DateTime.UtcNow
                });

                _context.Products.Add(new Product
                {
                    Id = 5,
                    ProductName = "Product 5",
                    Description = "Product 5 Desc",
                    Price = 15.99m,
                    ImageUrl = "http://example.com/product5.jpg",
                    CategoryId = 1,
                    IsDeleted = false,
                    AddedOn = DateTime.UtcNow
                });

                _context.Products.Add(new Product
                {
                    Id = 6,
                    ProductName = "Product 6",
                    Description = "Product 6 Desc",
                    Price = 16.99m,
                    ImageUrl = "http://example.com/product6.jpg",
                    CategoryId = 2,
                    IsDeleted = false,
                    AddedOn = DateTime.UtcNow
                });

                _context.Products.Add(new Product
                {
                    Id = 7,
                    ProductName = "Product 7",
                    Description = "Product 7 Desc",
                    Price = 100.99m,
                    ImageUrl = "http://example.com/product7.jpg",
                    CategoryId = 1,
                    IsDeleted = false,
                    AddedOn = DateTime.UtcNow
                });

                _context.Products.Add(new Product
                {
                    Id = 8,
                    ProductName = "Product 8",
                    Description = "Product 8 Desc",
                    Price = 14.99m,
                    ImageUrl = "http://example.com/product8.jpg",
                    CategoryId = 2,
                    IsDeleted = false,
                    AddedOn = DateTime.UtcNow
                });

                _context.Products.Add(new Product
                {
                    Id = 9,
                    ProductName = "Product 9",
                    Description = "Product 9 Desc",
                    Price = 9.99m,
                    ImageUrl = "http://example.com/product9.jpg",
                    CategoryId = 1,
                    IsDeleted = false,
                    AddedOn = DateTime.UtcNow
                });

                _context.Products.Add(new Product
                {
                    Id = 10,
                    ProductName = "Product 10",
                    Description = "Product 10 Desc",
                    Price = 12.99m,
                    ImageUrl = "http://example.com/product10.jpg",
                    CategoryId = 2,
                    IsDeleted = false,
                    AddedOn = DateTime.UtcNow
                });

                _context.SaveChanges();
            }
        }

        [Test]
        public async Task GetAddProductModelByIdAsync_ShouldReturnProductViewModelWithCategories()
        {
            // Act: Call the method
            var result = await _productService.GetAddProductModelByIdAsync();

            // Assert: Check if the result is not null and has the expected categories
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Categories, Is.Not.Empty);
            Assert.That(result.Categories.Count, Is.EqualTo(3)); // Should be 3 categories in the seed

            Assert.That(result.Categories[0].CategoryId, Is.EqualTo(1));
            Assert.That(result.Categories[0].Name, Is.EqualTo("Toys"));

            Assert.That(result.Categories[1].CategoryId, Is.EqualTo(2));
            Assert.That(result.Categories[1].Name, Is.EqualTo("Clothing"));

            Assert.That(result.Categories[2].CategoryId, Is.EqualTo(3));
            Assert.That(result.Categories[2].Name, Is.EqualTo("Books"));
        }

        [Test]
        public async Task AddProductAsync_ShouldAddProductToDatabase()
        {
            // Arrange: Create a ProductViewModel to add
            var model = new ProductViewModel
            {
                ProductName = "New Product",
                Description = "Description of the new product",
                Price = 100.0m,
                ImageUrl = "http://example.com/image.jpg",
                AddedOn = DateTime.UtcNow,
                CategoryId = 1, // Using the first category
                IsDeleted = false
            };

            // Act: Call the method to add the product
            await _productService.AddProductAsync(model);

            // Assert: Verify the product is in the database
            var addedProduct = await _context.Products
                .Where(p => p.ProductName == model.ProductName)
                .FirstOrDefaultAsync();

            Assert.That(addedProduct, Is.Not.Null);
            Assert.That(addedProduct.ProductName, Is.EqualTo(model.ProductName));
            Assert.That(addedProduct.Description, Is.EqualTo(model.Description));
            Assert.That(addedProduct.Price, Is.EqualTo(model.Price));
            Assert.That(addedProduct.ImageUrl, Is.EqualTo(model.ImageUrl));
            Assert.That(addedProduct.CategoryId, Is.EqualTo(model.CategoryId));
            Assert.That(addedProduct.IsDeleted, Is.EqualTo(model.IsDeleted));
        }

        [Test]
        public async Task GetEditProductModelByIdAsync_ShouldReturnCorrectProductEditViewModel()
        {
            // Arrange: Get the product with ID = 1
            var productId = 1;

            // Act: Call the method to get the EditProductViewModel for the product
            var result = await _productService.GetEditProductModelByIdAsync(productId);

            // Assert: Verify the returned ProductEditViewModel matches the expected values
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(productId));
            Assert.That(result.ProductName, Is.EqualTo("Car"));
            Assert.That(result.Description, Is.EqualTo("Fast as the light"));
            Assert.That(result.Price, Is.EqualTo(15.99m));
            Assert.That("http://example.com/car.jpg", Is.Not.Null);
            Assert.That(result.CategoryId, Is.EqualTo(1)); // Toys category

            // Verify categories list
            Assert.That(result.Categories.Count, Is.EqualTo(3)); 
        }

        [Test]
        public async Task GetEditProductModelByIdAsync_ShouldThrowExceptionForNonExistingProduct()
        {
            // Arrange: Try to get a product with a non-existing ID
            var productId = 999; // Non-existing product ID

            // Act: Call the method
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _productService.GetEditProductModelByIdAsync(productId));

            Assert.That("Product not found.", Is.EqualTo(ex.Message));
        }

        [Test]
        public async Task GetProductDetailsByIdAsync_ShouldReturnProductDetails_WhenProductExists()
        {
            // Arrange
            var productId = 1; // The ID of the product added in SetUp

            // Act
            var result = await _productService.GetProductDetailsByIdAsync(productId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(productId, Is.EqualTo(result.Id));
            Assert.That("Car", Is.EqualTo(result.ProductName));
            Assert.That("Fast as the light", Is.EqualTo(result.Description));
            Assert.That(15.99m, Is.EqualTo(result.Price));
            Assert.That("http://example.com/car.jpg", Is.EqualTo(result.ImageUrl));
            Assert.That("Toys", Is.EqualTo(result.CategoryName));
        }

        [Test]
        public async Task GetProductDetailsByIdAsync_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 999; // Non-existing product ID

            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _productService.GetProductDetailsByIdAsync(productId));

            Assert.That("Product not found.", Is.EqualTo(ex.Message));
        }

        [Test]
        public async Task GetPaginatedProductsAsync_ShouldReturnPaginatedList_WhenProductsExist()
        {
            // Act
            var result = await _productService.GetPaginatedProductsAsync(1, 5);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Items.Count, Is.EqualTo(5)); // Check if 5 items are returned
            Assert.That(10, Is.EqualTo(result.TotalItems)); // Total products in the database
            Assert.That(1, Is.EqualTo(result.PageNumber)); // Correct page number
            Assert.That(5, Is.EqualTo(result.PageSize)); // Correct page size
        }

        [Test]
        public async Task GetPaginatedProductsAsync_ShouldHandleEmptyProducts_WhenNoProductsExist()
        {
            // Arrange: Clear products from database
            _context.Products.RemoveRange(_context.Products);
            _context.SaveChanges();

            // Act
            var result = await _productService.GetPaginatedProductsAsync(1, 5);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Items.Count, Is.EqualTo(0)); // Should return no items since database is empty
            Assert.That(0, Is.EqualTo(result.TotalItems)); // Total items should be 0
            Assert.That(1, Is.EqualTo(result.PageNumber)); // Correct page number
            Assert.That(5, Is.EqualTo(result.PageSize)); // Correct page size
        }

        [Test]
        public async Task GetPaginatedProductsAsync_ShouldReturnEmptyPage_WhenPageNumberExceedsTotalPages()
        {
            // Act: Request page number that exceeds total pages
            var result = await _productService.GetPaginatedProductsAsync(3, 5); // Page 3 with 5 items per page

            // Assert: Since we have only 10 items, page 3 should return no items
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Items.Count, Is.EqualTo(0)); // Page 3 should be empty
            Assert.That(10, Is.EqualTo(result.TotalItems)); // Total items in the database
            Assert.That(3, Is.EqualTo(result.PageNumber)); // Correct page number
            Assert.That(5, Is.EqualTo(result.PageSize)); // Correct page size
        }

        [Test]
        public async Task GetPaginatedProductsAsync_ShouldApplyPaginationCorrectly()
        {
            // Act
            var result = await _productService.GetPaginatedProductsAsync(2, 3); // Request 2nd page with 3 items per page

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Items.Count, Is.EqualTo(3)); // Should return exactly 3 items
            Assert.That(10, Is.EqualTo(result.TotalItems)); // Total items in the database
            Assert.That(2, Is.EqualTo(result.PageNumber)); // Correct page number
            Assert.That(3, Is.EqualTo(result.PageSize)); // Correct page size

            // Check if the items on the second page are the right ones based on price sorting
            var productNames = result.Items.Select(p => p.ProductName).ToList();
            Assert.That("Product 8", Is.EqualTo(productNames[0])); // Second page starts from the 4th product in sorted order
            Assert.That("Car", Is.EqualTo(productNames[1]));
            Assert.That("Product 5", Is.EqualTo(productNames[2]));
        }

        [Test]
        public async Task UpdateProductAsync_ShouldUpdateProduct_WhenProductExists()
        {
            // Arrange
            var updatedProduct = new ProductEditViewModel
            {
                Id = 1,
                ProductName = "Updated Product 1",
                Price = 120,
                Description = "Updated Description",
                ImageUrl = "updated_image1.jpg",
                CategoryId = 1
            };

            // Act
            await _productService.UpdateProductAsync(updatedProduct);

            // Assert
            var product = await _context.Products.FindAsync(1);
            Assert.That(product, Is.Not.Null);
            Assert.That("Updated Product 1", Is.EqualTo(product.ProductName));
            Assert.That(120, Is.EqualTo(product.Price));
            Assert.That("Updated Description", Is.EqualTo(product.Description));
            Assert.That("updated_image1.jpg", Is.EqualTo(product.ImageUrl));
        }

        [Test]
        public async Task UpdateProductAsync_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            var nonExistingProduct = new ProductEditViewModel
            {
                Id = 999, // Non-existing product ID
                ProductName = "Non-existing Product",
                Price = 100,
                Description = "Non-existing Description",
                ImageUrl = "non_existing_image.jpg",
                CategoryId = 1
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _productService.UpdateProductAsync(nonExistingProduct));

            Assert.That("Product not found.", Is.EqualTo(ex.Message));
        }

        [Test]
        public async Task UpdateProductAsync_ShouldNotModifyOtherProperties_WhenOnlySomePropertiesAreUpdated()
        {
            // Arrange
            var product = await _context.Products.FindAsync(1);

            var updatedProduct = new ProductEditViewModel
            {
                Id = 1,
                ProductName = "Partially Updated Product 1", // Only updating the product name
                Price = product.Price, // Keeping price the same
                Description = product.Description, // Keeping description the same
                ImageUrl = product.ImageUrl, // Keeping image URL the same
                CategoryId = product.CategoryId // Keeping Category ID the same
            };

            // Act
            await _productService.UpdateProductAsync(updatedProduct);

            // Assert
            var updatedEntity = await _context.Products.FindAsync(1);
            Assert.That(updatedEntity, Is.Not.Null);
            Assert.That("Partially Updated Product 1", Is.EqualTo(updatedEntity.ProductName)); // Name should be updated
            Assert.That(product.Price, Is.EqualTo(updatedEntity.Price)); // Price should remain the same
            Assert.That(product.Description, Is.EqualTo(updatedEntity.Description)); // Description should remain the same
            Assert.That(product.ImageUrl, Is.EqualTo(updatedEntity.ImageUrl)); // Image URL should remain the same
        }

        [Test]
        public async Task GetProductDeleteByIdAsync_ShouldReturnProductDeleteViewModel_WhenProductExists()
        {
            // Arrange
            int productId = 1;

            // Act
            var result = await _productService.GetProductDeleteByIdAsync(productId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(productId, Is.EqualTo(result.Id));
            Assert.That("Car", Is.EqualTo(result.ProductName));
        }

        [Test]
        public async Task GetProductDeleteByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            int nonExistingProductId = 999; // ID that does not exist

            // Act
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _productService.GetProductDeleteByIdAsync(nonExistingProductId));

            Assert.That("Product not found.", Is.EqualTo(ex.Message));
        }

        [Test]
        public async Task SoftDeleteProductAsync_ShouldMarkProductAsDeleted_WhenProductExists()
        {
            // Arrange
            var model = new ProductDeleteViewModel { Id = 1 };

            // Act
            await _productService.SoftDeleteProductAsync(model);

            // Assert
            var product = await _context.Products.FindAsync(1);
            Assert.That(product, Is.Not.Null);
            Assert.That(product.IsDeleted, Is.True); // Check if the product is marked as deleted
        }

        [Test]
        public async Task SoftDeleteProductAsync_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            var model = new ProductDeleteViewModel { Id = 999 }; // Non-existing product ID

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () =>
                await _productService.SoftDeleteProductAsync(model));

            Assert.That("Product not found.", Is.EqualTo(exception.Message)); // Assert that the exception message is correct
        }
    }
}
