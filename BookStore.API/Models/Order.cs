using BookStore.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string ClaimCode { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int BookId { get; set; }

        public int Quantity { get; set; }

        public decimal PriceAtPurchase { get; set; }

        public Book Book { get; set; }
        public Order Order { get; set; }  // 🔥 Missing navigation property

    }

}
