using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using habyx.Data;
using habyx.Models;
using habyx.Services;

namespace habyx.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blobService;

        public FriendsController(ApplicationDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
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
                           && f.Status == "Accepted")
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
                Status = "Pending", // Changed from enum to string
                CreatedAt = DateTime.UtcNow
            };

            _context.Friends.Add(friendRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFriends), new { id = friendRequest.Id }, friendRequest);
        }

        // PUT: api/Friends/respond/{requestId}
        [HttpPut("respond/{requestId}")]
        public async Task<IActionResult> RespondToRequest(int requestId, [FromBody] string status)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            if (status != "Accepted" && status != "Rejected")
                return BadRequest("Invalid status");

            var userId = int.Parse(userIdClaim);
            var request = await _context.Friends.FindAsync(requestId);

            if (request == null)
                return NotFound();

            if (request.AddresseeId != userId)
                return Forbid();

            request.Status = status;
            request.UpdatedAt = DateTime.UtcNow; // Track update timestamp

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
                .Where(f => f.AddresseeId == userId && f.Status == "Pending")
                .Include(f => f.Requester)
                .ToListAsync();
        }

        // POST: api/Friends/upload-profile-image
        [HttpPost("upload-profile-image")]
        public async Task<ActionResult<string>> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("File must be an image");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
                return NotFound("User not found");

            var fileName = $"profile_{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var imageUrl = await _blobService.UploadImageAsync(file, fileName);

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                await _blobService.DeleteImageAsync(user.ProfileImageUrl);

            user.ProfileImageUrl = imageUrl;
            await _context.SaveChangesAsync();

            return Ok(imageUrl);
        }

        // DELETE: api/Friends/profile-image
        [HttpDelete("profile-image")]
        public async Task<IActionResult> DeleteProfileImage()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null)
                return NotFound("User not found");

            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                await _blobService.DeleteImageAsync(user.ProfileImageUrl);
                user.ProfileImageUrl = null;
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}