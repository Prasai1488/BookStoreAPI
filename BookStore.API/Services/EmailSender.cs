using System.Net;
using System.Net.Mail;
using System.Text;

namespace BookStore.API.Services
{
    public class EmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOrderConfirmationEmail(string toEmail, string userName, string claimCode, decimal total, List<string> bookTitles)
        {
            var subject = "Your BookStore Order Confirmation";
            var body = new StringBuilder();
            body.AppendLine($"Dear {userName},");
            body.AppendLine();
            body.AppendLine("Thank you for your order. Here are the details:");
            body.AppendLine();
            body.AppendLine($"📌 Claim Code: {claimCode}");
            body.AppendLine($"💰 Total Amount: {total:C}");
            body.AppendLine();
            body.AppendLine("📚 Books:");
            foreach (var title in bookTitles)
                body.AppendLine($"- {title}");
            body.AppendLine();
            body.AppendLine("Please present this claim code at the store to complete your pickup.");
            body.AppendLine();
            body.AppendLine("Happy Reading!");
            body.AppendLine("📖 BookStore Team");

            using var smtp = new SmtpClient(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]))
            {
                Credentials = new NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"]),
                EnableSsl = true
            };

            var mail = new MailMessage(_config["Smtp:From"], toEmail, subject, body.ToString());

            await smtp.SendMailAsync(mail);
        }
    }
}
