﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using DoAnLTW.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using DoAnLTW.Models.Repositories;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DoAnLTW.Controllers
{
    public class ShopController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IProductRepository _productRepository;

        public ShopController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IProductRepository productRepository)
        {
            _context = context;
            _userManager = userManager;
            _productRepository = productRepository;
        }

        // Action hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index(decimal? minPrice, decimal? maxPrice, string brandId, string size, int? categoryId)
        {
            SetCartCount();

            // Lấy danh sách sản phẩm với các bộ lọc
            var query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .Include(p => p.Images)
                .AsQueryable();

            // Lọc theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Lọc theo thương hiệu
            if (!string.IsNullOrEmpty(brandId))
            {
                var brandIds = brandId.Split(',').Select(int.Parse).ToList();
                query = query.Where(p => brandIds.Contains(p.BrandId));
            }

            // Lọc theo size
            if (!string.IsNullOrEmpty(size))
            {
                var selectedSizes = size.Split(',').Select(int.Parse).ToList();
                query = query.Where(p => p.ProductSizes.Any(ps => selectedSizes.Contains(ps.Size.size)));
            }

            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await query.ToListAsync();

            // Chuẩn bị dữ liệu cho các bộ lọc
            var brands = await _context.Brands
                .Select(b => new
                {
                    b.Id,
                    b.Name,
                    ProductCount = _context.Products.Count(p => p.BrandId == b.Id)
                })
                .ToListAsync();

            var availableSizes = await _context.Sizes
                .Select(s => new
                {
                    s.size,
                    ProductCount = _context.ProductSizes.Count(ps => ps.Size.size == s.size)
                })
                .OrderBy(s => s.size)
                .ToListAsync();

            // Lấy danh sách danh mục và số lượng sản phẩm
            var categories = await _context.Categories
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    ProductCount = _context.Products.Count(p => p.CategoryId == c.Id)
                })
                .ToListAsync();

            // Tính toán số lượng sản phẩm trong mỗi khoảng giá
            var priceRanges = new List<PriceRange>
            {
                new PriceRange { Min = 0M, Max = 500000M },
                new PriceRange { Min = 500000M, Max = 1000000M },
                new PriceRange { Min = 1000000M, Max = 2000000M },
                new PriceRange { Min = 2000000M, Max = 5000000M },
                new PriceRange { Min = 5000000M, Max = null }
            };

            var priceRangeCounts = priceRanges.Select(range => new
            {
                Range = range,
                Count = _context.Products.Count(p => 
                    p.Price >= range.Min && 
                    (!range.Max.HasValue || p.Price <= range.Max.Value))
            }).ToList();

            // Truyền dữ liệu cho view
            ViewBag.Brands = brands;
            ViewBag.Sizes = availableSizes;
            ViewBag.Categories = categories;
            ViewBag.PriceRanges = priceRangeCounts;
            ViewBag.TotalProducts = await _context.Products.CountAsync();

            // Gán ImageUrl cho mỗi sản phẩm
            foreach (var product in products)
            {
                var firstImage = product.Images.FirstOrDefault();
                product.ImageUrl = firstImage?.ImageUrl ?? "/img/default-product.jpg";
            }

            // Lấy danh sách sản phẩm yêu thích từ session
            var favouriteProducts = HttpContext.Session.GetString("FavouriteProducts");
            ViewBag.FavouriteProducts = string.IsNullOrEmpty(favouriteProducts) 
                ? new List<int>() 
                : JsonConvert.DeserializeObject<List<int>>(favouriteProducts);

            return View(products);
        }
        
        // Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.Products
          .Include(p => p.Brand)
          .Include(p => p.Images)
          .Include(p => p.ProductSizes) // Đảm bảo có dữ liệu size
              .ThenInclude(ps => ps.Size)
          .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(); // Trả về lỗi 404 nếu sản phẩm không tồn tại
            }

            return View(product);
        }

        public async Task<IActionResult> FavouriteProducts()
        {
            SetCartCount();
            // Lấy danh sách sản phẩm yêu thích từ session
            var favouriteProducts = HttpContext.Session.GetString("FavouriteProducts");
            if (string.IsNullOrEmpty(favouriteProducts))
            {
                return View(new List<Product>());
            }

            var productIds = JsonConvert.DeserializeObject<List<int>>(favouriteProducts);
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            return View(products);
        }

        [HttpPost]
        public IActionResult ToggleFavourite(int productId)
        {
            var favouriteProducts = HttpContext.Session.GetString("FavouriteProducts");
            var productIds = new List<int>();

            if (!string.IsNullOrEmpty(favouriteProducts))
            {
                productIds = JsonConvert.DeserializeObject<List<int>>(favouriteProducts);
            }

            if (productIds.Contains(productId))
            {
                productIds.Remove(productId);
            }
            else
            {
                productIds.Add(productId);
            }

            HttpContext.Session.SetString("FavouriteProducts", JsonConvert.SerializeObject(productIds));
            ViewBag.FavouriteCount = productIds.Count;

            return Ok(new { success = true, count = productIds.Count });
        }
    }

    public class PriceRange
    {
        public decimal Min { get; set; }
        public decimal? Max { get; set; }
    }

    public class PriceRangeCount
    {
        public PriceRange Range { get; set; }
        public int Count { get; set; }
    }
}