using BarbaKidsShop.Data;
using BarbaKidsShop.Services.Data.Interfaces;
using BarbaKidsShop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BarbaKidsShop.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ISearchService searchService;

        public SearchController(ApplicationDbContext dbContext, ISearchService searchService)
        {
            this.dbContext = dbContext;
            this.searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string productName)
        {

            IEnumerable<ProductIndexViewModel> products =
                await this.searchService.FindAProductByNameAsync(productName);

            return View(products);
        }
    }
}
