using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DoAnLTW.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // 🔹 Gọi base để cấu hình Identity đúng

            // 🔹 Định nghĩa lại khóa chính cho AspNetUserTokens
            modelBuilder.Entity<IdentityUserToken<string>>()
                .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            // 🔹 Giữ nguyên schema của Identity mặc định
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });

            modelBuilder.Entity<ProductVariant>()
                .HasKey(pv => pv.Id); // 🔹 Thêm khóa chính cho ProductVariant

            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(pv => pv.ProductId);


            modelBuilder.Entity<Product>()
        .Property(p => p.Price)
        .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.DiscountPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductVariant>()
                .Property(pv => pv.Price)
                .HasColumnType("decimal(18,2)");


            modelBuilder.Entity<Product>().HasData(
        new Product
        {
            Id = 1,
            Name = "Laptop Gaming Acer",
            Price = 1500000m,
            Stock = 10,
            Brand = "Acer",
            Category = "Laptop",
            Description = "Laptop gaming mạnh mẽ với card RTX 3060",
            Rating = 4,
            ReviewCount = 120,
            ImageUrl = "/img/product-1.jpg",
            ImageUrlsJson = "[\"/img/product-1.jpg\", \"/img/product-3.jpg\"]"
        },
        new Product
        {
            Id = 2,
            Name = "Điện thoại iPhone 13",
            Price = 1200000m,
            Stock = 20,
            Brand = "Apple",
            Category = "Smartphone",
            Description = "iPhone 13 chính hãng, màu xanh",
            Rating = 5,
            ReviewCount = 300,
            ImageUrl = "/img/product-2.jpg",
            ImageUrlsJson = "[\"/img/product-2.jpg\", \"/img/product-4.jpg\"]"
        }



    );
            // 🔹 Seed dữ liệu cho ProductVariant
            modelBuilder.Entity<ProductVariant>().HasData(
                new ProductVariant { Id = 1, ProductId = 1, Size = "15 inch", Color = "Black", Price = 1500000m },
                new ProductVariant { Id = 2, ProductId = 1, Size = "17 inch", Color = "Silver", Price = 1400000m },
                new ProductVariant { Id = 3, ProductId = 2, Size = "128GB", Color = "Blue", Price = 1200000m },
                new ProductVariant { Id = 4, ProductId = 2, Size = "256GB", Color = "Red", Price = 1100000m }
            );
        }

    }
}
