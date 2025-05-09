using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStore.API.Data;
using Microsoft.EntityFrameworkCore;
using BookStore.API.Enums;

namespace BookStore.API.Controllers.StaffController
{
    [ApiController]
    [Route("staff/orders")]
    [Authorize(Roles = "Staff")]
    public class OrderProcessingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderProcessingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessOrderByClaimCode([FromQuery] string claimCode)
        {
            if (string.IsNullOrWhiteSpace(claimCode))
                return BadRequest(new { message = "Claim code is required." });

            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Book)
                .FirstOrDefaultAsync(o => o.ClaimCode == claimCode);

            if (order == null)
                return NotFound(new { message = "No order found for this claim code." });

            if (order.Status != OrderStatus.Pending)
                return BadRequest(new { message = $"Order is already marked as {order.Status}." });

            order.Status = OrderStatus.Completed;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Order processed and marked as completed.",
                orderId = order.OrderId,
                total = order.TotalAmount
            });
        }
    }
}
