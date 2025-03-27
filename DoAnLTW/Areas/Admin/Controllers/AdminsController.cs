using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnLTW.Models; // Đảm bảo namespace đúng với project của bạn

namespace DoAnLTW.Areas.Admin.Controllers
{   
    [Authorize(Roles = "Admin")] // Chỉ Admin mới truy cập được
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> PhanQuyen()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email
                })
                .ToListAsync();

            var userRoles = new List<dynamic>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(user.Id));
                string role = roles.Contains("Admin") ? "Admin"
                           : roles.Contains("Employee") ? "Employee"
                           : "Customer"; // Nếu không có quyền nào thì mặc định là Customer

                userRoles.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    Role = role
                });
            }

            return View(userRoles);
        }



        [HttpPost]
        public async Task<IActionResult> SetUserRole([FromBody] RoleUpdateModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);

            // Nếu chọn Customer thì không cần thêm role vì nó là mặc định
            if (model.Role != "Customer")
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            return Ok();
        }

        public class RoleUpdateModel
        {
            public string UserId { get; set; }
            public string Role { get; set; }
        }


    }
}
