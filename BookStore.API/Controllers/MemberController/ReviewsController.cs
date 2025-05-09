using BookStore.API.Data;
using BookStore.API.DTOs;
using BookStore.API.Enums;
using BookStore.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookStore.API.Controllers.MemberController
{
    [ApiController]
    [Route("member/reviews")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewsController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        [HttpPost]
        public async Task<IActionResult> LeaveReview([FromBody] CreateReviewDto dto)
        {
            var userId = GetUserId();

            var hasPurchased = await _context.Orders
                .Where(o => o.UserId == userId && o.Status == OrderStatus.Completed)
                .SelectMany(o => o.Items)
                .AnyAsync(i => i.BookId == dto.BookId);

            if (!hasPurchased)
                return BadRequest(new { message = "You can only review books you’ve purchased." });

            var alreadyReviewed = await _context.BookReviews
                .AnyAsync(r => r.UserId == userId && r.BookId == dto.BookId);

            if (alreadyReviewed)
                return BadRequest(new { message = "You’ve already reviewed this book." });

            var review = new BookReview
            {
                BookId = dto.BookId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                UserId = userId
            };

            _context.BookReviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Review submitted successfully." });
        }


        // get my reviews
        [HttpGet]
        public async Task<IActionResult> GetMyReviews()
        {
            var userId = GetUserId();

            var reviews = await _context.BookReviews
                .Where(r => r.UserId == userId)
                .Include(r => r.Book)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Id,
                    r.BookId,
                    BookTitle = r.Book.Title,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }



    }

}
