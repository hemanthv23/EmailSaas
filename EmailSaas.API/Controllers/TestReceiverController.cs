using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailSaas.API.Controllers
{
    [ApiController]
    [Route("api/test-receiver")]
    [AllowAnonymous]   // ← add this
    public class TestReceiverController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Receive()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();



            return Ok(new { received = true });
        }
    }
}