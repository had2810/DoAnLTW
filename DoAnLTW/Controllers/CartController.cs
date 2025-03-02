using Microsoft.AspNetCore.Mvc;

namespace DoAnLTW.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
