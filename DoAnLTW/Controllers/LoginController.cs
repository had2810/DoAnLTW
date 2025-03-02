using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace DoAnLTW.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }
        //đăng nhập với goole
        public async Task LoginByGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                   new AuthenticationProperties
                   {
                       RedirectUri = Url.Action("GoogleResponse")
                   });
        }
        //phản hồi từ google
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext
            .AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });
            TempData["success"] = "Dang nhap thanh công";
            return RedirectToAction("Index", "Home");
            //return Json(claims);
        }
    }
}
