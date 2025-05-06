using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.API.Models
{
    public class BookBookmark
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int UserId { get; set; }

        // Navigation (optional)
        public Book Book { get; set; }
        public User User { get; set; }
    }
}
