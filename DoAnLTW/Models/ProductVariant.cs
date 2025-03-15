using System.Text.Json.Serialization;

namespace DoAnLTW.Models
{
    public class ProductVariant
    {
        public int Id { get; set; } // 🔹 Khóa chính (Primary Key)
        public int ProductId { get; set; } // 🔹 Khóa ngoại (Foreign Key)

        public string Size { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal? DiscountPrice { get; set; } // Giảm giá (nullable)
        [JsonIgnore] // Loại bỏ tham chiếu vòng lặp
        public Product Product { get; set; } // 🔹 Liên kết đến Product
    }

}
