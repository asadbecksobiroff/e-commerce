using easy_shop.Data;
using easy_shop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace easy_shop.Controllers
{
    [Route("Product")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ProductController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var userId = _userManager.GetUserId(User);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            var product = await _context.Products.FindAsync(id);
            if (cart != null)
            {
                var item = cart.CartItems.FirstOrDefault(i => i.ProductId == product.Id);
                ViewBag.Quantity = item != null ? item.Quantity : 0;
            }
            else ViewBag.Quantity = 0;

            return View(product);
        }
    }
}
