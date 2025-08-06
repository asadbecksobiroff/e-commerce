using easy_shop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace easy_shop.Controllers
{
    [Route("Category")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{category}")]
        public async Task<IActionResult> Index(string category)
        {
            var products = await _context.Products
                .Where(p => p.Category == category)
                .OrderByDescending(o => o.Id)
                .ToListAsync();
            return View(products);
        }

        [HttpGet("search/{item}")]
        public async Task<IActionResult> Search(string item)
        {
            var products = await _context.Products
                .Where(p => p.Category.ToLower().Contains(item.ToLower()) || p.Name.ToLower().Contains(item.ToLower()))
                .OrderByDescending(o => o.Id)
                .ToListAsync();
            return View(products);
        }
    }
}
