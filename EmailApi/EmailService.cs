namespace EmailApi
{
    using MailKit.Net.Smtp;
    using MimeKit;
    using Microsoft.Extensions.Configuration;
    using Org.BouncyCastle.Crypto.Digests;

    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly EmailDbContext _context;
        public EmailService(IConfiguration configuration, EmailDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            var emailInfo = new EmailMessage()
            {
                Id = Guid.NewGuid(),
                Subject = subject,
                Body = body,
                ToEmail = toEmail,
                IsSent = true,
                SentDate = DateTime.Now
            };
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Notification: ", smtpSettings["FromEmail"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(smtpSettings["Host"], int.Parse(smtpSettings["Port"]), false);
                await client.AuthenticateAsync(smtpSettings["Username"], smtpSettings["Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                await _context.EmailMessage.AddAsync(emailInfo);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}
