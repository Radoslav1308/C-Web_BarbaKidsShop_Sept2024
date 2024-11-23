using BarbaKidsShop.Data;
using BarbaKidsShop.Data.Models;
using BarbaKidsShop.Services.Data.Interfaces;
using BarbaKidsShop.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BarbaKidsShop.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ICartService cartService;

        public CartController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, ICartService cartService)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = userManager.GetUserId(User);

            if (currentUserId == null)
            {
                throw new InvalidOperationException("Invalid user.");
            }

            IEnumerable<CartViewModel> cartItems =
                await this.cartService.IndexGetAllProductsForUserInCartAsync(currentUserId);

            return View(cartItems);
        }
    }
}
