namespace EbookStore.Data
{
    using EbookStore.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.BorrowPrice)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.DiscountPrice)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)");
                // Specify decimal precision for TotalAmount and Price
                modelBuilder.Entity<Order>()
                    .Property(o => o.TotalAmount)
                    .HasColumnType("decimal(18,2)");

                modelBuilder.Entity<OrderItem>()
                    .Property(oi => oi.Price)
                    .HasColumnType("decimal(18,2)");

                modelBuilder.Entity<CartItems>()
               .HasKey(ci => ci.CartItemID); // Define the primary key

            });

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; }

        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<CartItems> CartItems { get; set; } = null!;

    }
}