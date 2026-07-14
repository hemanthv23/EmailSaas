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

            Console.WriteLine("========== WEBHOOK RECEIVED ==========");
            Console.WriteLine($"Signature: {Request.Headers["X-Webhook-Signature"]}");
            Console.WriteLine($"Event: {Request.Headers["X-Webhook-Event"]}");
            Console.WriteLine($"Body: {body}");
            Console.WriteLine("=======================================");

            return Ok(new { received = true });
        }
    }
}