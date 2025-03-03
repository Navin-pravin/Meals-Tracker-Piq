using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AljasAuthApi.Services
{
    public class EmailService
    {
        private readonly string _smtpHost = "smtp.gmail.com"; // Change to your SMTP host (e.g., Gmail SMTP)
        private readonly string _smtpUser = "alijasmohammed@gmail.com"; // Your email
        private readonly string _smtpPass = "xvdp imli ossu cave"; // Use an app password if you have 2FA enabled

        public async Task<bool> SendOtpAsync(string email, string otp)
        {
            try
            {
                using var smtpClient = new SmtpClient(_smtpHost)
                {
                    Port = 587, // Gmail SMTP port
                    Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                    EnableSsl = true // SSL enabled for security
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUser),
                    Subject = "Your OTP Code",
                    Body = $"Your OTP code is: {otp}",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("OTP sent successfully.");
                return true;
            }
            catch (SmtpException smtpEx)
            {
                // Log SMTP-related exceptions
                Console.WriteLine($"SMTP Error: {smtpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Log general exceptions
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> TestEmailAsync(string email)
        {
            try
            {
                using var smtpClient = new SmtpClient(_smtpHost)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUser),
                    Subject = "Test Email",
                    Body = "This is a test email.",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("Test email sent successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending test email: {ex.Message}");
                return false;
            }
        }
    }
}
