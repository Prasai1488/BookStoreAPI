using BookStore.API.Enums;
using BookStore.API.Models;
using Microsoft.EntityFrameworkCore;


namespace BookStore.API.Data.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(AppDbContext context)
        {
            // Apply pending migrations automatically
            await context.Database.MigrateAsync();

            // Check if an admin already exists
            if (!await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
            {
                var adminUser = new User
                {
                    Name = "Super Admin",
                    Email = "prasaiprithvi4@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),  // Hash the password
                    Role = UserRole.Admin,
                    MembershipId = null
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
