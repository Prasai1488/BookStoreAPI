using System.ComponentModel.DataAnnotations;
using BookStore.API.Enums;

namespace BookStore.API.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }  // Primary Key

        public string? ImageUrl { get; set; }  // Optional book cover image


        [Required]
        public string Title { get; set; }

        public string ISBN { get; set; }

        public string Description { get; set; }

        [Required]
        public string Author { get; set; }

        public BookGenre Genre { get; set; } 

        public string Language { get; set; }

        public BookFormat Format { get; set; } 

        public string Publisher { get; set; }

        public DateTime? PublicationDate { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }

        public bool IsExclusive { get; set; }  // e.g., Signed, Deluxe, Collector’s

        public bool OnSale { get; set; }

        public decimal? SalePrice { get; set; }

        public DateTime? SaleStart { get; set; }

        public DateTime? SaleEnd { get; set; }
    }
}
