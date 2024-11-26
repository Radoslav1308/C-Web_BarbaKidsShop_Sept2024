using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarbaKidsShop.Web.ViewModels;

namespace BarbaKidsShop.Services.Data.Interfaces
{
    public interface ISearchService
    {
        Task<IEnumerable<ProductIndexViewModel>> FindAProductByNameAsync(string productName);
    }
}
