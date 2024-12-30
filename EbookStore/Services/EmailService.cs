namespace EbookStore.Services
{
    using System.Net;
    using System.Net.Mail;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;

    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(_configuration["EmailSettings:Port"]);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];

            var mail = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true // Set to true for HTML content
            };

            mail.To.Add(toEmail);

            using (var smtpClient = new SmtpClient(smtpServer, port))
            {
                smtpClient.Credentials = new NetworkCredential(username, password);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(mail);
            }
        }
    }

}
