// Create TestController.cs in your Controllers folder
using Microsoft.AspNetCore.Mvc;

namespace habyx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Test controller is working!");
        }
    }
}