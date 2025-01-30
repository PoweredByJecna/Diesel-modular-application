using Diesel_modular_application.Models;
using Diesel_modular_application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;



namespace Diesel_modular_application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] TableDieslovani dieslovani,string emailResult)
        {
            // Zavoláme metodu, která v EmailService sestaví 
            // text a předmět a e-mail odešle
            await _emailService.SendDieslovaniEmailAsync(dieslovani, emailResult);

            // Vrátíme 200 OK
            return Ok(new { message = "E‑mail byl odeslán." });
        }
    }
}
