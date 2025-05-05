using BookStore.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookStore.API.DTOs.Books
{
    public class CreateBookDto
    {
        [Required]
        public string Title { get; set; }

        public string ISBN { get; set; }

        public string Description { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public BookGenre Genre { get; set; }

        public string Language { get; set; }

        [Required]
        public BookFormat Format { get; set; }

        public string Publisher { get; set; }

        public DateTime? PublicationDate { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsExclusive { get; set; }

        public bool OnSale { get; set; }

        public decimal? SalePrice { get; set; }

        public DateTime? SaleStart { get; set; }

        public DateTime? SaleEnd { get; set; }

        public string? ImageUrl { get; set; }
    }
}
