using BookStore.API.Data;
using BookStore.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BookStore.API.DTOs.Books;

namespace BookStore.API.Controllers.MemberController
{
    [ApiController]
    [Route("member/bookmarks")]
    [Authorize] // any logged-in user
    public class BookmarksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookmarksController(AppDbContext context)
        {
            _context = context;
        }

        // 📌 Add to Bookmark
        [HttpPost("{bookId}")]
        public async Task<IActionResult> Add(int bookId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            bool alreadyBookmarked = await _context.BookBookmarks
                .AnyAsync(b => b.BookId == bookId && b.UserId == userId);

            if (alreadyBookmarked)
                return BadRequest("Book already bookmarked.");

            var bookmark = new BookBookmark
            {
                BookId = bookId,
                UserId = userId
            };

            _context.BookBookmarks.Add(bookmark);
            await _context.SaveChangesAsync();

            return Ok("Book bookmarked successfully.");
        }

        // 📋 View all bookmarks

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var bookmarks = await _context.BookBookmarks
                .Where(b => b.UserId == userId)
                
                .Select(b => new BookResponseDto
                {
                    BookId = b.Book.BookId,
                    Title = b.Book.Title,
                    ISBN = b.Book.ISBN,
                    Description = b.Book.Description,
                    Author = b.Book.Author,
                    Genre = b.Book.Genre,
                    Language = b.Book.Language,
                    Format = b.Book.Format,
                    Publisher = b.Book.Publisher,
                    PublicationDate = b.Book.PublicationDate,
                    Price = b.Book.Price,
                    StockQuantity = b.Book.StockQuantity,
                    IsExclusive = b.Book.IsExclusive,
                    OnSale = b.Book.OnSale,
                    SalePrice = b.Book.SalePrice,
                    SaleStart = b.Book.SaleStart,
                    SaleEnd = b.Book.SaleEnd,
                    ImageUrl = b.Book.ImageUrl
                })
                .ToListAsync();

            return Ok(bookmarks);
        }

        // ❌ Remove bookmark
        [HttpDelete("{bookId}")]
        public async Task<IActionResult> Remove(int bookId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var bookmark = await _context.BookBookmarks
                .FirstOrDefaultAsync(b => b.BookId == bookId && b.UserId == userId);

            if (bookmark == null) return NotFound("Bookmark not found.");

            _context.BookBookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();

            return Ok("Bookmark removed.");
        }
    }
}
