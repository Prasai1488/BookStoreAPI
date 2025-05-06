using BookStore.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers.PublicController
{
    [ApiController]
    [Route("public/books/{bookId}/reviews")]
    public class BookReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookReviewsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookReviews(int bookId)
        {
            var reviews = await _context.BookReviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Id,
                    r.Rating,
                    r.Comment,
                    Reviewer = r.User.Name,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }
    }
}
