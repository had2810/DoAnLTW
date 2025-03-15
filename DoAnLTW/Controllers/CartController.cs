using Microsoft.AspNetCore.Mvc;
using DoAnLTW.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTW.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "Cart";
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
         
        private Product GetProductById(int id)
        {
            return _context.Products
                .Include(p => p.Variants)
                .FirstOrDefault(p => p.Id == id);
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart([FromBody] CartItem model)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == model.ProductId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
            }

            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
            var existingItem = cart.FirstOrDefault(x => x.ProductId == model.ProductId && x.Size == model.Size && x.Color == model.Color);

            if (existingItem != null)
            {
                existingItem.Quantity += 1; // Tăng số lượng nếu sản phẩm đã có trong giỏ hàng
            }
            else
            {
                // Thêm sản phẩm mới vào giỏ hàng
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    Name = product.Name,  // ✅ Đảm bảo lấy từ CSDL
                    ImageUrl = product.ImageUrl, // ✅ Đảm bảo lấy từ CSDL
                    Price = product.Price,       // ✅ Đảm bảo lấy từ CSDL
                    Size = model.Size,           // ✅ Lấy từ request
                    Color = model.Color,         // ✅ Lấy từ request
                    Quantity = 1
                });
            }

            // Lưu giỏ hàng vào Session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return Json(new { success = true, cartCount = cart.Sum(x => x.Quantity) });
        }




        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
            }

            HttpContext.Session.SetObjectAsJson(CartSessionKey, cart);
            return RedirectToAction("Index");
        }


        /// AJAX : /Cart/UpdateCart
        [HttpPost]
        public IActionResult UpdateCart([FromBody] UpdateCartRequest model)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.ProductId == model.ProductId && x.Size == model.Size && x.Color == model.Color);

            if (item != null)
            {
                if (model.Action == "increase")
                {
                    item.Quantity += 1;
                }
                else if (model.Action == "decrease" && item.Quantity > 1)
                {
                    item.Quantity -= 1;
                }
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return Json(new
            {
                success = true,
                cartCount = cart.Sum(x => x.Quantity),
                totalPrice = cart.Sum(x => x.Price * x.Quantity),
                itemTotal = item?.Quantity * item?.Price
            });
        }

        public class UpdateCartRequest
        {
            public int ProductId { get; set; }
            public string Size { get; set; }
            public string Color { get; set; }
            public string Action { get; set; }
        }

        [HttpPost]
        public IActionResult RemoveFromCart([FromBody] RemoveCartRequest model)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var item = cart.FirstOrDefault(x => x.ProductId == model.ProductId && x.Size == model.Size && x.Color == model.Color);

            if (item != null)
            {
                cart.Remove(item);
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return Json(new
            {
                success = true,
                cartCount = cart.Sum(x => x.Quantity),
                totalPrice = cart.Sum(x => x.Price * x.Quantity)
            });
        }

        public class RemoveCartRequest
        {
            public int ProductId { get; set; }
            public string Size { get; set; }
            public string Color { get; set; }
        }
    }
}
