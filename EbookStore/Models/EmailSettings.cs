namespace EbookStore.Models
{
    public class EmailSettings
    {
        public string Host { get; set; } // The SMTP server address (e.g., smtp.gmail.com)
        public int Port { get; set; } // SMTP server port (usually 587 or 465)
        public string Username { get; set; } // The email username (e.g., your email address)
        public string Password { get; set; } // The email password (or app-specific password)
        public string FromEmail { get; set; } // The "from" email address (the email that will appear as the sender)
    }
}
