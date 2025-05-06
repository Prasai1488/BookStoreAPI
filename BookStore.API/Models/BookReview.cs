using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Models
{
    public class BookReview
    {
        [Key]
        public int Id { get; set; }

        public int BookId { get; set; }

        public int UserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Book Book { get; set; }
        public User User { get; set; }
    }

}
