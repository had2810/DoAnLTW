using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using DoAnLTW.Models;

namespace DoAnLTW.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách sản phẩm
        public IActionResult Index(string category = null, string search = null)
        {
            IQueryable<Product> products = _context.Products.Include(p => p.Variants);

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
            }

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search));
            }

            return View(products.ToList());
        }

        // Chi tiết sản phẩm
        public IActionResult Detail(int id)
        {
            var product = _context.Products
                .Include(p => p.Variants)
                .Include(p => p.RelatedProducts)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Lọc sản phẩm theo danh mục
        public IActionResult Category(string category)
        {
            var products = _context.Products.Where(p => p.Category == category).ToList();
            return View("Index", products);
        }

        // Tìm kiếm sản phẩm
        public IActionResult Search(string searchTerm)
        {
            var products = _context.Products
                .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
                .ToList();

            return View("Index", products);
        }

        // Hiển thị sản phẩm liên quan
        public IActionResult RelatedProducts(int productId)
        {
            var product = _context.Products.Include(p => p.RelatedProducts).FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            return PartialView("_RelatedProducts", product.RelatedProducts);
        }
    }
}
