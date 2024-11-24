using BarbaKidsShop.Data;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Services.Data;
using BarbaKidsShop.Services.Data.Interfaces;
using BarbaKidsShop.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BarbaKidsShop.Web.Controllers
{
    public class ShippingDetailController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IShippingDetailService shippingDetailService;

        public ShippingDetailController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, IShippingDetailService shippingDetailService)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.shippingDetailService = shippingDetailService;
        }

        [HttpGet]
        public IActionResult ShippingDetails()
        {
            return View(new ShippingDetailViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShippingDetails(ShippingDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUserId = userManager.GetUserId(User);

            if (currentUserId == null)
            {
                throw new InvalidOperationException("Invalid user.");
            }
            
            await this.shippingDetailService.CreateShippingDetailAsync(model, currentUserId);
            
            return RedirectToAction("Index");
        }
    }
}
