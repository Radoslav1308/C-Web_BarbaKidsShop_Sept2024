using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Data.Repository.Interfaces;
using BarbaKidsShop.Services.Data.Interfaces;
using BarbaKidsShop.Web.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BarbaKidsShop.Services.Data
{
    public class SearchService : ISearchService
    {
        private IRepository<Product, int> productRepository;

        public SearchService(IRepository<Product, int> productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductIndexViewModel>> FindAProductByNameAsync(string productName)
        {
            var products = await this.productRepository
                .GetAllAttached()
                .Where(p => p.ProductName == productName)
                .Where(p => !p.IsDeleted)
                .Select(p => new ProductIndexViewModel
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            if (products.Count == 0)
            {
                throw new Exception("Products not found.");
            }

            return products;
        }
    }
}
