using BookStore.API.Data;
using BookStore.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
                .Include(b => b.Book)
                .Select(b => new
                {
                    b.BookId,
                    b.Book.Title,
                    b.Book.ImageUrl,
                    b.Book.Author,
                    b.Book.OnSale,
                    b.Book.Price,
                    b.Book.SalePrice
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
