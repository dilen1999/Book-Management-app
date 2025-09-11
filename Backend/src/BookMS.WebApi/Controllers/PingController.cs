using BookMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace BookMS.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PingController(AppDbContext db) => _db = db;

        [HttpGet("db")]
        public async Task<IActionResult> DbHealth()
        {
            var canConnect = await _db.Database.CanConnectAsync();
            return Ok(new { ok = canConnect, provider = _db.Database.ProviderName });
        }
    }
}
