namespace EbookStore.Data
{
    using EbookStore.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base OnModelCreating to configure Identity-related tables and keys
            base.OnModelCreating(modelBuilder);
            // OrderItems relationship
            modelBuilder.Entity<Book>()
        .Property(b => b.Price)
        .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Book>()
                .Property(b => b.BorrowPrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Book>()
                .Property(b => b.DiscountPrice)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<CartItems>()
                .Property(ci => ci.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18, 2)");
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine); // Logs SQL queries to the console
            base.OnConfiguring(optionsBuilder);

            // Suppress pending model changes warnings
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        // DbSets for your entities
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; }
        public DbSet<UserLibrary> UserLibrary { get; set; }
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItems> CartItems { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        // No need to explicitly define AppUsers DbSet; IdentityDbContext already manages it
        // If you must define it for querying purposes, uncomment the line below
        // public DbSet<AppUser> AppUsers { get; set; }
    }
}
