using Microsoft.AspNetCore.Mvc;

namespace DoAnLTW.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
