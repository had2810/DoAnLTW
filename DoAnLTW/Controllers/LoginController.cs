using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DoAnLTW.Models.Repositories;
using DoAnLTW.Models;
namespace DoAnLTW.Controllers
{
    public class LoginController : Controller
    {
        private readonly EAccount _account;
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
            if (!result.Succeeded)
            {
                return RedirectToAction("SignUp");
            }
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(
                claims => new
                {
                    claims.Issuer,
                    claims.OriginalIssuer,
                    claims.Type,
                    claims.Value
                });
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            string emailName = email.Split('@')[0]; /// hoanganhduc42115 @gmail.com
            string password = "123456";
            //kiểm tra xem tài khoản đã tồn tại chưa
            var existingAccount = _account.GetAccountByUsername(emailName);
            if (existingAccount == null)
            {
                _account.CreateAccount(new Account
                {
                    username = emailName,
                    password = "123456",
                    email = email
                });

                // sửa lại trong EAccount là trả về giao diện chính ẩn nút sign in sign out thay bằng log out
                if (_account.ValidateLogin(emailName, password))
                {
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    return RedirectToAction("SignUp");
                }
            }
            else 
            {
                _account.ValidateLogin(emailName, password);

            }
        }
    }
}
