using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace DoAnLTW.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; } // Giá sau khuyến mãi
        public string Description { get; set; }
        public int Stock { get; set; }
        public string Brand { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string Category { get; set; }

        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public List<Product> RelatedProducts { get; set; } = new List<Product>();

        public string ImageUrl { get; set; }

        // 🔹 Dùng chuỗi JSON để lưu danh sách ảnh trong database
        [Column(TypeName = "nvarchar(max)")]
        public string ImageUrlsJson { get; set; }

        // 🔹 Không ánh xạ thuộc tính này vào database, chỉ dùng trong code
        [NotMapped]
        public List<string> ImageUrls
        {
            get => string.IsNullOrEmpty(ImageUrlsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(ImageUrlsJson);
            set => ImageUrlsJson = JsonSerializer.Serialize(value);
        }
    }
}
