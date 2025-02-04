using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Diesel_modular_application.Models; 
using static Diesel_modular_application.Services.OdstavkyService;


namespace Diesel_modular_application.Services
{
    public class EmailService(IConfiguration config)
    {
        private readonly IConfiguration _config = config;

        /// <summary>
        /// Veřejná metoda, která bere dieslování a sama sestaví e‑mail
        /// (předmět a tělo) a odešle ho skrze SendEmailAsync
        /// </summary>
        public async Task SendDieslovaniEmailAsync(TableDieslovani dieslovani, string emailResult)
        {
            var subject="";
             var body="";
            if(emailResult=="DA-ok")
            {
                subject = $"Objednávka DA č. {dieslovani.IdDieslovani} " +
                          $"na lokalitu: {dieslovani.Odstavka?.Lokality?.Lokalita}";

            body = $@"
                <h1>Dobrý den</h1>
                <p>
                    Toto je objednávka DA na lokalitu: 
                    <strong>{dieslovani.Odstavka?.Lokality?.Lokalita}</strong>
                </p>
            ";
            }
            else{
                subject = $"Zrušení DA č. {dieslovani.IdDieslovani} " +
                          $"na lokalitu: {dieslovani.Odstavka?.Lokality?.Lokalita}";

            body = $@"
                <h1>Dobrý den</h1>
                <p>
                    Toto je objednávka DA na lokalitu: 
                    <strong>{dieslovani.Odstavka?.Lokality?.Lokalita}</strong>
                </p>
            ";

            }
            

            // A zavoláme níže uvedenou "obecnou" metodu
            await SendEmailAsync(subject, body);
        }

        /// <summary>
        /// Obecná pomocná metoda – pošle e‑mail s daným subjectem a body
        /// (používána v různých scénářích)
        /// </summary>
        public async Task SendEmailAsync(string subject, string body)
        {
            var emailSettings = _config.GetSection("EmailSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                emailSettings["SenderName"],
                emailSettings["SenderEmail"]));

            // Příklad: posíláme "sobě" nebo někam nastaveně
            message.To.Add(new MailboxAddress(
                "", 
                emailSettings["SenderEmail"]));

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(
                    emailSettings["SmtpServer"],
                    int.Parse(emailSettings["SmtpPort"]),
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    emailSettings["Username"], 
                    emailSettings["Password"]);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
