using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using habyx.Data;
using habyx.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace habyx.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HousingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HousingController> _logger;

        public HousingController(ApplicationDbContext context, ILogger<HousingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Housing
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HousingListing>>> GetListings()
        {
            try
            {
                var listings = await _context.HousingListings.ToListAsync();
                return Ok(listings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving housing listings");
                return StatusCode(500, new { message = "An error occurred while retrieving listings" });
            }
        }

        // GET: api/Housing/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HousingListing>> GetListing(int id)
        {
            try
            {
                var listing = await _context.HousingListings.FindAsync(id);

                if (listing == null)
                {
                    return NotFound(new { message = "Listing not found" });
                }

                return Ok(listing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving housing listing {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving the listing" });
            }
        }

        // POST: api/Housing
        [HttpPost]
        public async Task<ActionResult<HousingListing>> CreateListing(HousingListing listing)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                
                listing.OwnerId = userId;
                listing.CreatedAt = DateTime.UtcNow;
                
                _context.HousingListings.Add(listing);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetListing), new { id = listing.Id }, listing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating housing listing");
                return StatusCode(500, new { message = "An error occurred while creating the listing" });
            }
        }
    }
}