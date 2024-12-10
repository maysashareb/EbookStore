
﻿using EbookStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
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
        });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
}