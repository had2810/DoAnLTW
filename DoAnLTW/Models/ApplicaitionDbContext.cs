using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace DoAnLTW.Models
{
   public class ApplicationDbContext : IdentityDbContext
    {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext>
            options) : base(options)
            {
            }
            public DbSet<Product> products { get; set; }
           
        }
}
    