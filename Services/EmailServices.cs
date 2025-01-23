using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;


public class EmailServices
{
    public readonly IConfiguration _config;

    public EmailServices(IConfiguration config)
    {
        _config = config;
    }


    public async Task SendEmailAsync (string subject, string body)
    {
        var emailSettings = _config.GetSection("EmailSettings");



        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
        message.To.Add(new MailboxAddress("", emailSettings["SenderEmail"]));
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

            await client.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

    }
}