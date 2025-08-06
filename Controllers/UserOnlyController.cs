using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace easy_shop.Controllers
{
    [Authorize]
    public class UserOnlyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}