using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIWithJwt.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        [HttpGet("Test")]
        public ActionResult Test()
        {
            return Ok("lala");
        }
    }
}