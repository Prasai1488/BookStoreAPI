using BookStore.API.Enums;
using System.Text.Json.Serialization;

namespace BookStore.API.DTOs.Books
{
    public class UpdateBookDto
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BookGenre? Genre { get; set; }
        public string Language { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BookFormat? Format { get; set; }
        public string Publisher { get; set; }
        public DateTime? PublicationDate { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public bool? IsExclusive { get; set; }
        public bool? OnSale { get; set; }
        public decimal? SalePrice { get; set; }
        public DateTime? SaleStart { get; set; }
        public DateTime? SaleEnd { get; set; }
        public string? ImageUrl { get; set; }
    }
}
