using Microsoft.EntityFrameworkCore;
using BookStore.API.Models;
using BookStore.API.Enums;

namespace BookStore.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Store enum as string for readability
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Book>()
    .Property(b => b.Genre)
    .HasConversion<string>();

            modelBuilder.Entity<Book>()
                .Property(b => b.Format)
                .HasConversion<string>();

        }
    }
}
