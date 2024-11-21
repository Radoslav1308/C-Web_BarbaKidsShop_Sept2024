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

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductEditViewModel> GetEditProductModelAsync(int id)
        {
            var product = await this.productRepository
                .GetByIdAsync(id);

            if (product == null)
            {
                return null;
            }

            return new ProductEditViewModel
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
        }

        public Task<ProductDetailsViewModel> GetProductDetailsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProductDetailsViewModel> GetProductDetailsByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductIndexViewModel>> IndexGetAllProductsOrderedByPriceAsync()
        {
            IEnumerable<ProductIndexViewModel> products = await this.productRepository
                .GetAllAttached()
                .OrderBy(p => p.Price)
                .Select(p => new ProductIndexViewModel
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                })                
                .ToListAsync();

            return products;
        }

        public async Task UpdateProductAsync(ProductViewModel model)
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
    }
}
