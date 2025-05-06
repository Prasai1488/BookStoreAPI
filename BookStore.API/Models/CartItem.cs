using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        public int Quantity { get; set; } = 1;

        // Optional navigation
        public User User { get; set; }
        public Book Book { get; set; }
    }
}
