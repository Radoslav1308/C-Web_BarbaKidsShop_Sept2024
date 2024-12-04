using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Data.Repository.Interfaces;
using BarbaKidsShop.Services.Data.Interfaces;
using BarbaKidsShop.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BarbaKidsShop.Services.Data
{
    public class ProductService : IProductService
    {
        private IRepository<Product, int> productRepository;
        private IRepository<Category, int> categoryRepository;

        public ProductService(IRepository<Product, int> productRepository, IRepository<Category, int> categoryRepository)
        {
            this.productRepository = productRepository;
            this.categoryRepository = categoryRepository;
        }

        public async Task<ProductViewModel> GetAddProductModelByIdAsync()
        {
            var model = new ProductViewModel();

            model.Categories = await this.categoryRepository.GetAllAttached()
                    .Select(c => new CategoryViewModel
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name
                    }).ToListAsync();

            return model;
        }
        public async Task AddProductAsync(ProductViewModel model)
        {
            var productToAdd = new Product
            {
                ProductName = model.ProductName,
                Description = model.Description,
                Price = model.Price,
                ImageUrl = model.ImageUrl,
                AddedOn = model.AddedOn,
                CategoryId = model.CategoryId,
                IsDeleted = model.IsDeleted,
            };

            await this.productRepository.AddAsync(productToAdd);
        }

        public async Task<ProductEditViewModel> GetEditProductModelByIdAsync(int id)
        {
            var product = await this.productRepository
                .GetByIdAsync(id);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            var productEditViewModel =  new ProductEditViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                Categories = await this.categoryRepository.GetAllAttached()
                    .Select(c => new CategoryViewModel
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name
                    }).ToListAsync()
            };

            return productEditViewModel;
        }

        public async Task<ProductDetailsViewModel> GetProductDetailsByIdAsync(int id)
        {
            var product = await this.productRepository
                .GetAllAttached()
                .Where(p => p.Id == id)
                .Include(p => p.Category)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            var productDetailsViewModel = new ProductDetailsViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category.Name
            };

            return productDetailsViewModel;
        }

        public async Task<PaginatedListViewModel<ProductIndexViewModel>> GetPaginatedProductsAsync(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 9 : pageSize;

            var query = this.productRepository.GetAllAttached()
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Price);

            var totalItems = await query.CountAsync();

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductIndexViewModel
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            return new PaginatedListViewModel<ProductIndexViewModel>
            {
                Items = products,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task UpdateProductAsync(ProductEditViewModel model)
        {
            var product = await this.productRepository.GetByIdAsync(model.Id);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }
               
            product.ProductName = model.ProductName;
            product.Price = model.Price;
            product.Description = model.Description;
            product.ImageUrl = model.ImageUrl;
            product.CategoryId = model.CategoryId;

            await productRepository.UpdateAsync(product);
        }

        public async Task<ProductDeleteViewModel> GetProductDeleteByIdAsync(int id)
        {
            var product = await this.productRepository.GetByIdAsync(id);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            var productDeleteViewModel =  new ProductDeleteViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName
            };

            return productDeleteViewModel;
        }

        public async Task SoftDeleteProductAsync(ProductDeleteViewModel model)
        {
            var product = await this.productRepository.GetByIdAsync(model.Id);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            product.IsDeleted = true; // Mark as deleted
        }
    }
}
