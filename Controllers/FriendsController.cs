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

        // Existing endpoints remain the same...
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Friend>>> GetFriends()
        {
            // Your existing GetFriends implementation
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

        [HttpPost("send-request/{userId}")]
        public async Task<ActionResult<Friend>> SendFriendRequest(int userId)
        {
            // Your existing SendFriendRequest implementation
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

        [HttpPut("respond/{requestId}")]
        public async Task<IActionResult> RespondToRequest(int requestId, [FromBody] FriendStatus status)
        {
            // Your existing RespondToRequest implementation
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

        [HttpDelete("{friendId}")]
        public async Task<IActionResult> RemoveFriend(int friendId)
        {
            // Your existing RemoveFriend implementation
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

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<Friend>>> GetPendingRequests()
        {
            // Your existing GetPendingRequests implementation
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);
            return await _context.Friends
                .Where(f => f.AddresseeId == userId && f.Status == FriendStatus.Pending)
                .Include(f => f.Requester)
                .ToListAsync();
        }

        // New endpoint for uploading profile image
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

            // Generate unique filename
            var fileName = $"profile_{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            
            // Upload to blob storage
            var imageUrl = await _blobService.UploadImageAsync(file, fileName);

            // Update user profile
            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                await _blobService.DeleteImageAsync(user.ProfileImageUrl);
            }

            user.ProfileImageUrl = imageUrl;
            await _context.SaveChangesAsync();

            return Ok(imageUrl);
        }

        // New endpoint for deleting profile image
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