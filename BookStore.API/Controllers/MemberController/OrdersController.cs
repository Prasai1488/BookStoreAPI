using BookStore.API.Data;
using BookStore.API.Enums;
using BookStore.API.Models;
using BookStore.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStore.API.Controllers.MemberController
{
    [ApiController]
    [Route("member/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailSender _emailSender;

        public OrdersController(AppDbContext context, EmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var userId = GetUserId();
            var user = await _context.Users.FindAsync(userId);
            

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Book)
                .ToListAsync();

            var bookTitles = cartItems.Select(c => c.Book.Title).ToList();

            if (!cartItems.Any())
                return BadRequest(new { message = "Your cart is empty." });

            // Validate stock
            foreach (var item in cartItems)
            {
                if (item.Quantity > item.Book.StockQuantity)
                    return BadRequest(new { message = $"Not enough stock for {item.Book.Title}." });
            }

            // Calculate total
            var subtotal = cartItems.Sum(item =>
            {
                var price = item.Book.OnSale && item.Book.SalePrice.HasValue
                    ? item.Book.SalePrice.Value
                    : item.Book.Price;

                return price * item.Quantity;
            });

            // Apply 5% discount if 5+ books
            var totalQuantity = cartItems.Sum(c => c.Quantity);
            decimal discount = 0;
            if (totalQuantity >= 5)
                discount = subtotal * 0.05m;

            // Check if user has 10+ completed orders
            var completedOrders = await _context.Orders
                .Where(o => o.UserId == userId && o.Status == OrderStatus.Completed)
                .CountAsync();

            bool isEligibleForStackable = completedOrders >= 10;

            // Apply 10% stackable discount on top of previous discount
            if (isEligibleForStackable)
            {
                // Apply 10% on already discounted subtotal
                var stackableDiscountBase = subtotal - discount;
                var stackableDiscount = stackableDiscountBase * 0.10m;
                discount += stackableDiscount;
            }

            var finalTotal = subtotal - discount;

            // Generate claim code
            var claimCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            // Create order
            var order = new Order
            {
                UserId = userId,
                TotalAmount = finalTotal,
                ClaimCode = claimCode,
                Items = cartItems.Select(item => new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.Book.OnSale && item.Book.SalePrice.HasValue
                        ? item.Book.SalePrice.Value
                        : item.Book.Price
                }).ToList()
            };

            _context.Orders.Add(order);

            // Decrease book stock
            foreach (var item in cartItems)
            {
                item.Book.StockQuantity -= item.Quantity;
            }

            // Clear cart
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            await _emailSender.SendOrderConfirmationEmail(
    toEmail: user.Email,
    userName: user.Name,
    claimCode: claimCode,
    total: finalTotal,
    bookTitles: bookTitles
);

            return Ok(new
            {
                message = "Order placed successfully.",
                orderId = order.OrderId,
                claimCode,
                total = finalTotal,
                discountsApplied = new
                {
                    base5Percent = totalQuantity >= 5,
                    extra10Percent = isEligibleForStackable
                }
            });
        }

        // 🔹 GET Orders
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
        {
            var userId = GetUserId();

            var query = _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .ThenInclude(i => i.Book)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(o => o.Status == parsedStatus);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = orders.Select(order => new
            {
                order.OrderId,
                order.TotalAmount,
                order.ClaimCode,
                order.Status,
                order.CreatedAt,
                Books = order.Items.Select(i => new
                {
                    i.BookId,
                    i.Book.Title,
                    i.Quantity,
                    i.PriceAtPurchase
                })
            });

            return Ok(new
            {
                currentPage = page,
                pageSize,
                totalPages,
                totalCount,
                orders = result
            });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = GetUserId();

            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Book)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (order == null)
                return NotFound(new { message = "Order not found." });

            if (order.Status != OrderStatus.Pending)
                return BadRequest(new { message = "Only pending orders can be cancelled." });

            // Restock books
            foreach (var item in order.Items)
            {
                item.Book.StockQuantity += item.Quantity;
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order cancelled successfully." });
        }


    }

}
