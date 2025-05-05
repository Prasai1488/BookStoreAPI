using BookStore.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.API.Controllers.PublicController
{
    [ApiController]
    [Route("public/announcements")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnnouncementsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveAnnouncements()
        {
            var now = DateTime.UtcNow;
            var banners = await _context.Announcements
                .Where(a => a.StartTime <= now && a.EndTime >= now)
                .ToListAsync();

            return Ok(banners);
        }
    }
}
