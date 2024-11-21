using System;
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
        Task<ProductEditViewModel> GetEditProductModelByIdAsync(int id);
        Task<ProductDetailsViewModel> GetProductDetailsByIdAsync(int id);
        Task UpdateProductAsync(ProductEditViewModel model);
        Task<ProductDeleteViewModel> GetProductDeleteByIdAsync(int id);
        Task SoftDeleteProductAsync(ProductDeleteViewModel model);
    }
}
