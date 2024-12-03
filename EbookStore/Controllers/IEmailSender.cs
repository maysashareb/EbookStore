using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

public class EmailSender : IEmailSender
{
    private readonly string _smtpServer = "smtp.gmail.com"; // Gmail's SMTP server
    private readonly int _smtpPort = 587; // Port for TLS
    private readonly string _email = "maysaaabu225@gmail.com"; // Sender email address
    private readonly string _password = "nwpy vtcz ztwk zjhs"; // App password from Google

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SmtpClient(_smtpServer, _smtpPort)
        {
            Credentials = new NetworkCredential(_email, _password),
            EnableSsl = true // Use TLS encryption
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_email, "Ebook Store Support"), // Sender name
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        mailMessage.To.Add(email); // Recipient's email

        await client.SendMailAsync(mailMessage); // Send the email
    }
}
