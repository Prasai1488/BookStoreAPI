using BookStore.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }  // Primary Key

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }  // Should be hashed

        [Required]
        public UserRole Role { get; set; } = UserRole.Member;

        public string? MembershipId { get; set; }
    }
}
