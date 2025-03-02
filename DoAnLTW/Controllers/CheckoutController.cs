using Microsoft.AspNetCore.Mvc;

namespace DoAnLTW.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
