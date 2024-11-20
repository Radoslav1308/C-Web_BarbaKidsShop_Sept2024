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

        public ProductService(IRepository<Product, int> productRepository)
        {
            this.productRepository = productRepository;
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

        public Task UpdateProductAsync(ProductViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
