﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnLTW.Models
{
    public class Size
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kích thước là bắt buộc")]
        [StringLength(10, ErrorMessage = "Kích thước không được quá 10 ký tự")]
        public int size { get; set; } // Ví dụ: "42", "43", "M", "L"

        // Một size có thể được dùng bởi nhiều sản phẩm
        public List<Product> Products { get; set; } = new List<Product>();
    }
}