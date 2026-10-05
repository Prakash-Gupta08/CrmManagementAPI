using CrmManagementAPI.Common;
using CrmManagementAPI.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CrmManagementAPI.Services
{
    /// <summary>
    /// Sends real emails through any SMTP server (Gmail, Outlook / Office 365, company mail server).
    /// Uses the built-in System.Net.Mail, so no NuGet package is required.
    /// </summary>
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public SmtpEmailSender(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public bool IsConfigured =>
            _settings.Enabled
            && !string.IsNullOrWhiteSpace(_settings.Host)
            && !string.IsNullOrWhiteSpace(_settings.Username)
            && !string.IsNullOrWhiteSpace(_settings.Password)
            && !string.IsNullOrWhiteSpace(_settings.FromEmail);

        public async Task SendAsync(string toEmail, string? toName, string subject, string body, string? ccEmail = null)
        {
            if (!IsConfigured)
                throw new InvalidOperationException("Email is not configured. Fill the EmailSettings section in appsettings.json.");

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = false,
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8
            };

            message.To.Add(string.IsNullOrWhiteSpace(toName)
                ? new MailAddress(toEmail)
                : new MailAddress(toEmail, toName));

            if (!string.IsNullOrWhiteSpace(ccEmail))
            {
                foreach (var cc in ccEmail.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (MailAddress.TryCreate(cc, out var ccAddress))
                        message.CC.Add(ccAddress);
                }
            }

            await client.SendMailAsync(message);
        }
    }
}