using System.ComponentModel.DataAnnotations;

namespace DoAnLTW.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [Range(10000, 1000000, ErrorMessage = "Giá phải nằm trong khoảng từ 10,000 đến 1,000,000")]
        public decimal? Price { get; set; }
    }
}
