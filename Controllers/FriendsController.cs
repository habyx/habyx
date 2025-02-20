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
    public class FriendsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FriendsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Friends
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Friend>>> GetFriends()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);
            return await _context.Friends
                .Where(f => (f.RequesterId == userId || f.AddresseeId == userId) 
                           && f.Status == FriendStatus.Accepted)
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .ToListAsync();
        }

        // POST: api/Friends/send-request/{userId}
        [HttpPost("send-request/{userId}")]
        public async Task<ActionResult<Friend>> SendFriendRequest(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var requesterId = int.Parse(userIdClaim);
            
            if (requesterId == userId)
                return BadRequest("Cannot send friend request to yourself");

            var existingRequest = await _context.Friends
                .FirstOrDefaultAsync(f => 
                    (f.RequesterId == requesterId && f.AddresseeId == userId) ||
                    (f.RequesterId == userId && f.AddresseeId == requesterId));

            if (existingRequest != null)
                return BadRequest("Friend request already exists");

            var friendRequest = new Friend
            {
                RequesterId = requesterId,
                AddresseeId = userId,
                Status = FriendStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Friends.Add(friendRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFriends), new { id = friendRequest.Id }, friendRequest);
        }

        // PUT: api/Friends/respond/{requestId}
        [HttpPut("respond/{requestId}")]
        public async Task<IActionResult> RespondToRequest(int requestId, [FromBody] FriendStatus status)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            if (status != FriendStatus.Accepted && status != FriendStatus.Rejected)
                return BadRequest("Invalid status");

            var userId = int.Parse(userIdClaim);
            var request = await _context.Friends.FindAsync(requestId);

            if (request == null)
                return NotFound();

            if (request.AddresseeId != userId)
                return Forbid();

            request.Status = status;
            request.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Friends/{friendId}
        [HttpDelete("{friendId}")]
        public async Task<IActionResult> RemoveFriend(int friendId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);
            var friendship = await _context.Friends.FindAsync(friendId);

            if (friendship == null)
                return NotFound();

            if (friendship.RequesterId != userId && friendship.AddresseeId != userId)
                return Forbid();

            _context.Friends.Remove(friendship);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Friends/pending
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Friend>>> GetPendingRequests()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);
            return await _context.Friends
                .Where(f => f.AddresseeId == userId && f.Status == FriendStatus.Pending)
                .Include(f => f.Requester)
                .ToListAsync();
        }
    }
}