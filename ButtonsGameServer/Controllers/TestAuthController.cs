using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ButtonsGameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestAuthController : ControllerBase
    {
        [HttpGet("userId")]
        public async Task<string> GetUserId()
        {
            return "Cheese";
        }
    }
}
