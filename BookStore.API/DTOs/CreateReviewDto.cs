using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs
{
    public class CreateReviewDto
    {
        public int BookId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }
    }

}
