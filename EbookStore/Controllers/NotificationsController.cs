namespace EbookStore.Controllers
{
    using EbookStore.Services;
    using Microsoft.AspNetCore.Mvc;

    public class NotificationsController : Controller
    {
        private readonly EmailService _emailService;

        public NotificationsController(EmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<IActionResult> SendNotification(string customerEmail)
        {
            if (string.IsNullOrEmpty(customerEmail))
            {
                return BadRequest("Customer email is required.");
            }

            var subject = "Order Confirmation";
            var body = "<h1>Your Order is Confirmed!</h1><p>Thank you for shopping with us.</p>";

            try
            {
                await _emailService.SendEmailAsync(customerEmail, subject, body);
                return Ok("Notification sent successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to send email: {ex.Message}");
            }
        }
    }

}
