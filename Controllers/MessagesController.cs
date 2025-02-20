using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using habyx.Data;
using habyx.Models;

namespace habyx.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Messages/conversation/{userId}
        [HttpGet("conversation/{userId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetConversation(int userId)
        {
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserIdClaim))
                return Unauthorized();

            var currentUserId = int.Parse(currentUserIdClaim);

            return await _context.Messages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                           (m.SenderId == userId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.CreatedAt)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();
        }

        // POST: api/Messages
        [HttpPost]
        public async Task<ActionResult<Message>> SendMessage(Message message)
        {
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserIdClaim))
                return Unauthorized();

            var currentUserId = int.Parse(currentUserIdClaim);
            message.SenderId = currentUserId;
            message.CreatedAt = DateTime.UtcNow;

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetConversation), new { userId = message.ReceiverId }, message);
        }

        // GET: api/Messages/unread
        [HttpGet("unread")]
        public async Task<ActionResult<IEnumerable<Message>>> GetUnreadMessages()
        {
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserIdClaim))
                return Unauthorized();

            var currentUserId = int.Parse(currentUserIdClaim);

            return await _context.Messages
                .Where(m => m.ReceiverId == currentUserId && !m.IsRead)
                .Include(m => m.Sender)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        // PUT: api/Messages/markRead/{messageId}
        [HttpPut("markRead/{messageId}")]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserIdClaim))
                return Unauthorized();

            var currentUserId = int.Parse(currentUserIdClaim);
            var message = await _context.Messages.FindAsync(messageId);

            if (message == null)
                return NotFound();

            if (message.ReceiverId != currentUserId)
                return Forbid();

            message.IsRead = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}