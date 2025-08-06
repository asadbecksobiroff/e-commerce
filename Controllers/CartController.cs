using easy_shop.Data;
using easy_shop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace easy_shop.Controllers
{
    [Route("Cart")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                ViewBag.Message = "Savatchangiz bo‘sh.";
                return View(new List<CartItem>());
            }

            return View(cart.CartItems.ToList());
        }


        [HttpPost("AddToCart")]
        [Authorize]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto model)
        {
            var userId = _userManager.GetUserId(User); // Identity user ID

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, TotalPrice = 0 };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var product = await _context.Products.FindAsync(model.ProductId);
            if (product == null)
                return NotFound();

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == model.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += model.Quantity;
                existingItem.Price += product.Price * model.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = model.ProductId,
                    Quantity = model.Quantity,
                    Price = product.Price * model.Quantity
                });
            }

            cart.TotalPrice = cart.CartItems.Sum(ci => ci.Price);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Mahsulot savatchaga qo‘shildi" });
        }

        [HttpPost("DecreaseQuantity")]
        [Authorize]
        public async Task<IActionResult> DecreaseQuantity([FromBody] CartActionDto model)
        {
            var userId = _userManager.GetUserId(User);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return NotFound();

            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == model.ProductId);
            if (item == null) return NotFound();

            if (item.Quantity > 1)
            {
                item.Quantity--;
                item.Price = item.Quantity * _context.Products.First(p => p.Id == model.ProductId).Price;
            }
            else
            {
                cart.CartItems.Remove(item);
            }

            cart.TotalPrice = cart.CartItems.Sum(i => i.Price);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpPost("RemoveFromCart")]
        [Authorize]
        public async Task<IActionResult> RemoveFromCart([FromBody] CartActionDto model)
        {
            var userId = _userManager.GetUserId(User);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return NotFound();

            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == model.ProductId);
            if (item != null)
            {
                cart.CartItems.Remove(item);
                cart.TotalPrice = cart.CartItems.Sum(i => i.Price);
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true });
        }


        [HttpPost("PlaceOrder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string payment, string location)
        {
            if (location == null) return RedirectToAction("Index");
            if (payment == null) return RedirectToAction("Index");

            var userId = _userManager.GetUserId(User);

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Savatchangiz bo‘sh.";
                return RedirectToAction("Index");
            }

            var order = new Order
            {
                UserId = userId,
                TotalPrice = cart.TotalPrice,
                Status = "Yig'ilmoqda",
                OrderDate = DateTime.Now,
                Location = location,
                PaymentStatus = payment
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };                
                _context.OrderItems.Add(orderItem);

                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null && product.Available >= item.Quantity)
                {
                    product.Available -= item.Quantity;
                }
                else
                {
                    TempData["Error"] = $"Mahsulot yetarli emas: {product?.Name ?? "Noma'lum"}";
                    return RedirectToAction("Index");
                }
            }

            _context.CartItems.RemoveRange(cart.CartItems);
            cart.TotalPrice = 0;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Buyurtma muvaffaqiyatli joylandi!";
            return RedirectToAction("Index", "Orders");
        }

    }
}

