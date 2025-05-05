using BookStore.API.Data;
using BookStore.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers.AdminController;


    [ApiController]
[Route("admin/announcements")]
[Authorize(Roles = "Admin")]
public class AnnouncementsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AnnouncementsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Announcement dto)
    {
        dto.StartTime = dto.StartTime.ToUniversalTime();
        dto.EndTime = dto.EndTime.ToUniversalTime();

        _context.Announcements.Add(dto);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Announcement created", dto.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Announcement dto)
    {
        var existing = await _context.Announcements.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Message = dto.Message;
        existing.StartTime = dto.StartTime.ToUniversalTime();
        existing.EndTime = dto.EndTime.ToUniversalTime();

        await _context.SaveChangesAsync();
        return Ok("Updated");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null) return NotFound();

        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();
        return Ok("Deleted");
    }
}
