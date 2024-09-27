using Homeverse.Application.Interfaces;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Homeverse.Infrastructure.Services;

public class MailSettings
{
    public string SenderEmail { get; set; }
    public string Password { get; set; }
}

public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;
    public MailService(IOptions<MailSettings> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendAsync(string email, string subject, string htmlMessage)
    {
        string senderEmail = _mailSettings.SenderEmail;
        string password = _mailSettings.Password;

        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress("Homverse", senderEmail));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = subject;

        var builder = new BodyBuilder();
        builder.HtmlBody = htmlMessage;
        message.Body = builder.ToMessageBody();

        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        try
        {
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(senderEmail, password);
            await smtp.SendAsync(message);
        }
        catch (Exception ex)
        {
            Directory.CreateDirectory("mailssave");
            var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
            await message.WriteToAsync(emailsavefile);
        }

        await smtp.DisconnectAsync(true);
    }
}
