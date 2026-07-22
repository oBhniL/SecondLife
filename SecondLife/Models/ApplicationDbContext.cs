using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace SecondLife.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false) { }

        public DbSet<Category> DanhMucs { get; set; }
        public DbSet<Product> SanPhams { get; set; }
        public DbSet<ProductImage> AnhSanPhams { get; set; }
        public DbSet<Order> DonHangs { get; set; }
        public DbSet<OrderDetail> ChiTietDonHangs { get; set; }
        public DbSet<Review> DanhGias { get; set; }

        public DbSet<CartItem> GioHangs { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}