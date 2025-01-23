using Diesel_modular_application.Controllers;
using Diesel_modular_application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/email")]
[ApiController]
public class EmailController : ControllerBase
{
    private readonly EmailServices _emailService;
    private readonly OdstavkyController _odstavky;

    public EmailController(EmailServices emailService, OdstavkyController odstavky)
    {
        _emailService = emailService;
        _odstavky = odstavky;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail(TableDieslovani dieslovani)
    {
        
        await _emailService.SendEmailAsync(
        $"Objednávka DA č. {dieslovani.IdDieslovani} na lokalitu: {dieslovani.Odstavka.Lokality.Lokalita}",
        $@"
            <h1>Dobrý den</h1>
            <p>Toto je objednávka DA na lokalitu: <strong>{dieslovani.Odstavka.Lokality.Lokalita}</strong></p>
   
        ");
        return Ok();
    }
}
