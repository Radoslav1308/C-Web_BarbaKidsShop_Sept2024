﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Web.ViewModels;

namespace BarbaKidsShop.Services.Data.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductIndexViewModel>> IndexGetAllProductsOrderedByPriceAsync();
        Task AddProductAsync(ProductViewModel model);
        Task<ProductDetailsViewModel> GetProductDetailsByIdAsync(int id);
        Task UpdateProductAsync(ProductViewModel model);
        Task DeleteProductAsync(int id);
    }
}
