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

        public DbSet<Announcement> Announcements { get; set; }

        public DbSet<BookBookmark> BookBookmarks { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<BookReview> BookReviews { get; set; }



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

            modelBuilder.Entity<Order>()
    .Property(o => o.Status)
    .HasConversion<string>();

        }
    }
}
