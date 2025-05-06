using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStore.API.Data;
using Microsoft.EntityFrameworkCore;
using BookStore.API.Models;
using System.Security.Claims;

namespace BookStore.API.Controllers.MemberController
{
    [ApiController]
    [Route("member/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // ➕ Add to cart by one at a time
        [HttpPost("{bookId}")]
        public async Task<IActionResult> AddToCart(int bookId)
        {
            var userId = GetUserId();

            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                return NotFound("Book not found.");

            var existing = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

            if (existing != null)
            {
                if (existing.Quantity + 1 > book.StockQuantity)
                {
                    return BadRequest($"Cannot add more. Only {book.StockQuantity} in stock.");
                }

                existing.Quantity += 1;
                await _context.SaveChangesAsync();
                return Ok("Book already in cart. Quantity updated.");
            }

            if (book.StockQuantity < 1)
                return BadRequest("Book is currently out of stock.");

            _context.CartItems.Add(new CartItem
            {
                BookId = bookId,
                UserId = userId,
                Quantity = 1
            });

            await _context.SaveChangesAsync();
            return Ok("Book added to cart.");
        }


        // 📋 View cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();

            var items = await _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Book)
                .Select(c => new
                {
                    c.BookId,
                    c.Book.Title,
                    c.Book.ImageUrl,
                    c.Book.Price,
                    c.Book.OnSale,
                    c.Book.SalePrice,
                    c.Quantity
                })
                .ToListAsync();

            return Ok(items);
        }

        // 🗑 Remove book from cart
        [HttpDelete("{bookId}")]
        public async Task<IActionResult> Remove(int bookId)
        {
            var userId = GetUserId();

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

            if (item == null) return NotFound("Book not in cart.");

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok("Book removed from cart.");
        }

        // 🔽 Decrease quantity by one
        [HttpPut("{bookId}/decrease")]
        public async Task<IActionResult> DecreaseQuantity(int bookId)
        {
            var userId = GetUserId();

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

            if (item == null) return NotFound("Item not in cart.");

            if (item.Quantity <= 1)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
                return Ok("Item removed from cart.");
            }

            item.Quantity -= 1;
            await _context.SaveChangesAsync();

            return Ok("Quantity decreased by 1.");
        }

        // Delete all items in cart
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Your cart is already empty.");

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Ok("All items removed from your cart.");
        }

    }
}
