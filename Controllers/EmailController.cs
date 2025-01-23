using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/email")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly EmailServices _emailService;

    public EmailController(EmailServices emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail()
    {
        await _emailService.SendEmailAsync("Testovací předmět", "<h1>Ahoj!</h1><p>Toto je testovací e-mail.</p>");
        return Ok("E-mail byl úspěšně odeslán na dieselmodapp@gmail.com.");
    }
}
