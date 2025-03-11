using Microsoft.AspNetCore.Mvc;
using habyx.Data;
using habyx.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace habyx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TempHousingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TempHousingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetListings()
        {
            // Use raw SQL query instead of DbSet property
            var result = await _context.Database
                .SqlQueryRaw<HousingListing>("SELECT * FROM HousingListings")
                .ToListAsync();
                
            return Ok(result);
        }
    }
}