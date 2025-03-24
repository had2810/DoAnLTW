using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using DoAnLTW.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace DoAnLTW.Controllers
{

    public class ShopController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ShopController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Action hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = _context.Products
        .Include(p => p.Images) // Include để lấy dữ liệu từ bảng Product_Images
        .Select(p => new Product
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Brand = p.Brand,
            Description = p.Description,
            CategoryId = p.CategoryId,
            ImageUrl = _context.ProductImages
                .Where(img => img.ProductId == p.Id)
                .Select(img => img.ImageUrl)
                .FirstOrDefault()
        })
        .ToList();

            return View(products);
        }


        // Hiển thị danh sách sản phẩm
        // [AllowAnonymous]

        //public async Task<IActionResult> Index(string category = null, string search = null)
        //{
        //    IQueryable<Product> products = _context.Products.Include(p => p.Variants);

        //    if (!string.IsNullOrEmpty(category))
        //    {
        //        products = products.Where(p => p.Category.Name == category);
        //    }

        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        products = products.Where(p => p.Name.Contains(search));
        //    }

        //    var userId = _userManager.GetUserId(User);
        //    var favouriteProductIds = _context.FavouriteProducts
        //        .Where(f => f.UserId == userId)
        //        .Select(f => f.ProductId)
        //        .ToList();

        //    // Tính số lượng sản phẩm yêu thích
        //    ViewBag.FavouriteProducts = favouriteProductIds;
        //    ViewBag.FavouriteCount = userId != null ? await _context.FavouriteProducts.CountAsync(f => f.UserId == userId) : 0;

        //    return View(products.ToList());
        //}

        //// Chi tiết sản phẩm
        //public async Task<IActionResult> Detail(int id)
        //{
        //    var product = _context.Products
        //        .Include(p => p.Variants)
        //        .Include(p => p.RelatedProducts)
        //        .FirstOrDefault(p => p.Id == id);

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    var userId = _userManager.GetUserId(User);
        //    ViewBag.FavouriteCount = userId != null ? await _context.FavouriteProducts.CountAsync(f => f.UserId == userId) : 0;

        //    return View(product);
        //}

        //// Lọc sản phẩm theo danh mục
        //public async Task<IActionResult> Category(string category)
        //{
        //    var products = _context.Products.Where(p => p.Category.Name == category).ToList();
        //    var userId = _userManager.GetUserId(User);
        //    ViewBag.FavouriteCount = userId != null ? await _context.FavouriteProducts.CountAsync(f => f.UserId == userId) : 0;
        //    return View("Index", products);
        //}

        //// Tìm kiếm sản phẩm
        //public async Task<IActionResult> Search(string searchTerm)
        //{
        //    var products = _context.Products
        //        .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
        //        .ToList();

        //    var userId = _userManager.GetUserId(User);
        //    ViewBag.FavouriteCount = userId != null ? await _context.FavouriteProducts.CountAsync(f => f.UserId == userId) : 0;
        //    return View("Index", products);
        //}

        //// Hiển thị sản phẩm liên quan
        //public async Task<IActionResult> RelatedProducts(int productId)
        //{
        //    var product = _context.Products.Include(p => p.RelatedProducts).FirstOrDefault(p => p.Id == productId);

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    var userId = _userManager.GetUserId(User);
        //    ViewBag.FavouriteCount = userId != null ? await _context.FavouriteProducts.CountAsync(f => f.UserId == userId) : 0;
        //    return PartialView("_RelatedProducts", product.RelatedProducts);
        //}

        //// Thêm sản phẩm vào danh sách yêu thích (không dùng AJAX)
        //[HttpPost]
        //public async Task<IActionResult> AddToFavourites(int productId)
        //{
        //    var userId = await _userManager.GetUserIdAsync(await _userManager.GetUserAsync(User));
        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return RedirectToAction("Index", "Shop");
        //    }

        //    var existingFavourite = await _context.FavouriteProducts
        //        .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);

        //    if (existingFavourite != null)
        //    {
        //        return RedirectToAction("Index", "Shop");
        //    }

        //    var favourite = new FavouriteProduct
        //    {
        //        UserId = userId,
        //        ProductId = productId
        //    };

        //    _context.FavouriteProducts.Add(favourite);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Index", "Shop");
        //}

        //// Hiển thị view danh sách yêu thích
        //public async Task<IActionResult> FavouriteProducts()
        //{
        //    var userId = _userManager.GetUserId(User);
        //    if (userId == null)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //    var favouriteProducts = _context.FavouriteProducts
        //        .Where(f => f.UserId == userId)
        //        .Include(f => f.Product)
        //        .Select(f => f.Product)
        //        .ToList();

        //    ViewBag.FavouriteCount = await _context.FavouriteProducts.CountAsync(f => f.UserId == userId);
        //    return View(favouriteProducts);
        //}
    }
}