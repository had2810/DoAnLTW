using Microsoft.EntityFrameworkCore;
namespace DoAnLTW.Models
{
   public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext>
            options) : base(options)
            {
            }
            public DbSet<Account> Accounts { get; set; }
           
        }
}
