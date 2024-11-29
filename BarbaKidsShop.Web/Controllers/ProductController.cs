using BarbaKidsShop.Data;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Services.Data.Interfaces;
using BarbaKidsShop.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarbaKidsShop.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IProductService productService;

        public ProductController(ApplicationDbContext dbContext,IProductService productService)
        {
            this.dbContext = dbContext;
            this.productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 9)
        {
            var model = await this.productService.GetPaginatedProductsAsync(pageNumber, pageSize);
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var model = await this.productService.GetAddProductModelByIdAsync();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await this.productService.AddProductAsync(model);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await productService.GetEditProductModelByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await productService.UpdateProductAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var product = await productService.GetProductDetailsByIdAsync(id); 

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await productService.GetProductDeleteByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(ProductDeleteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await productService.SoftDeleteProductAsync(model);
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
