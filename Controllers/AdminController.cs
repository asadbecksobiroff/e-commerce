using easy_shop.Data;
using easy_shop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace easy_shop.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> OrdersList()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.Status == "Yig'ilmoqda")
                .OrderBy(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> DeliveringOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.Status == "Yetkazilmoqda")
                .OrderBy(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> OrdersHistory()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderBy(o => o.OrderDate)
                .ToListAsync();

            if (orders == null) return NotFound();
            return View(orders);
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imagePath = null;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string wwwRootPath = _env.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    string path = Path.Combine(wwwRootPath, "images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    imagePath = "/images/" + fileName;
                }

                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    ImageUrl = imagePath,
                    Available = model.Available,
                    Category = model.Category
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // mahsulotlar ro‘yxati sahifasi
            }

            return View(model);
        }

        [HttpGet("Admin/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(AllProducts));
        }

        [HttpGet("Admin/UpdateStatus/{id}/{status}")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (!(new[] { "Yig'ilmoqda", "Yetkazilmoqda", "Yetkazildi", "Cancel" }.Contains(status)) ) return NotFound();
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }

            var returnUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAvailable(int id, int available)
        {
            Product product = await _context.Products.FindAsync(id);
            if (product == null) return RedirectToAction(nameof(AllProducts));

            product.Available = available;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllProducts));
        }

    }
}