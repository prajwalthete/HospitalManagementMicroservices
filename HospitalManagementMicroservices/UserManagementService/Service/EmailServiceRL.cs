using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using UserManagementService.Dto;
using UserManagementService.GlobleExceptionhandler;
using UserManagementService.Interface;

namespace UserManagementService.Service
{
    public class EmailServiceRL : IEmail
    {
        private readonly EmailSettings _emailSettings;

        public EmailServiceRL(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task<bool> SendEmailAsync(string to, string subject, string htmlMessage)
        {
            try
            {
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_emailSettings.FromEmail),
                        Subject = subject,
                        Body = htmlMessage,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(to);

                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                throw new EmailSendingException("SMTP error occurred while sending email", smtpEx);
            }
            catch (InvalidOperationException invalidOpEx)
            {
                throw new EmailSendingException("Invalid operation occurred while sending email", invalidOpEx);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}