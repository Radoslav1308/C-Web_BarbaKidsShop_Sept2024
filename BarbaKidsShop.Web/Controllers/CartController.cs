using BarbaKidsShop.Data;
using BarbaKidsShop.Services.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BarbaKidsShop.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICartService cartService;

        public CartController(ApplicationDbContext dbContext, ICartService cartService)
        {
            this.dbContext = dbContext;
            this.cartService = cartService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
