using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnLTW.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using DoAnLTW.Models.Repositories;

namespace DoAnLTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // 1. Danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).Include(p => p.Images).Include(p=>p.Brand).ToListAsync();
            return View(products);
        }

        // 2. Xem chi tiết sản phẩm
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return StatusCode(404, "Không tìm thấy sản phẩm"); // Trả về lỗi 404 kèm thông báo
            }


            return View(product);
        }


        // 3. Thêm sản phẩm - GET
        public async Task<IActionResult> Create()
        {
            //var products = await _context.Products.Include(p => p.Category).Include(p => p.Images).Include(p => p.Brand).ToListAsync();
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewBag.Brands = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name");
            return View();
        }

        // 4. Thêm sản phẩm - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<IFormFile> ImageFiles)
        {
            if (!ModelState.IsValid)
            {
                // Truyền lại danh sách danh mục và thương hiệu nếu có lỗi
                ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
                ViewBag.Brands = new SelectList(await _context.Brands.ToListAsync(), "Id", "Name");
                return View(product);
            }

            // Gán thương hiệu cho sản phẩm nếu đã chọn
            var selectedBrand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == product.BrandId);
            product.Brand = selectedBrand; // Gán thương hiệu vào sản phẩm
            var selectedCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == product.CategoryId);
            product.Category = selectedCategory; // Gán danh mục vào sản phẩm
            // Thêm sản phẩm vào cơ sở dữ liệu
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Xử lý thêm hình ảnh cho sản phẩm
            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                foreach (var image in ImageFiles)
                {
                    string imageUrl = await SaveImage(image); 
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        _context.ProductImages.Add(new Product_Images { ProductId = product.Id, ImageUrl = imageUrl });
                    }
                }
                await _context.SaveChangesAsync();
            }

            // Thêm thông báo thành công
            TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 5. Sửa sản phẩm - GET
        public IActionResult Edit(int id)
        {
            var product = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Lấy danh sách thương hiệu & danh mục từ database
            ViewBag.BrandList = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewBag.CategoryList = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);

            return View(product);
        }


        // 6. Sửa sản phẩm - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, List<IFormFile> ImageFiles)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();

                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    foreach (var image in ImageFiles)
                    {
                        string imageUrl = await SaveImage(image);
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            _context.ProductImages.Add(new Product_Images { ProductId = product.Id, ImageUrl = imageUrl });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // 7. Xóa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            // Xóa ảnh liên quan
            foreach (var img in product.Images)
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, img.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                _context.ProductImages.Remove(img);
            }

            // Xóa sản phẩm
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // Lưu nhiều ảnh vào thư mục
        private async Task<string> SaveImage(IFormFile image)
        {
            if (image == null || image.Length == 0) return null;

            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            string savePath = Path.Combine(uploadDir, uniqueFileName);

            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return "/images/" + uniqueFileName;
        }
    }
}
