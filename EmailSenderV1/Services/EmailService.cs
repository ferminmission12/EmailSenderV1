using EmailSenderV1.Models;
using EmailSenderV1.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmailSenderV1.Services
{
    public class EmailService:IEmailService
    {
        private readonly DefaultEmailSetting _defaultemailSettings;
        private readonly BackupEmailSetting _backupemailSettings;
        public EmailService(IOptions<DefaultEmailSetting> defaultemailSettings, IOptions<BackupEmailSetting> backupemailSettings)
        {
            _defaultemailSettings = defaultemailSettings.Value;
            _backupemailSettings = backupemailSettings.Value;
        }

        public async Task SendToDefaultEmailAsync(EmailDetails  emailDetails)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_defaultemailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(emailDetails.ToEmail));
            email.Subject = emailDetails.Subject;
            var builder = new BodyBuilder();
            if (emailDetails.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in emailDetails.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = emailDetails.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_defaultemailSettings.Host, _defaultemailSettings.Port,true);
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
            smtp.Authenticate(_defaultemailSettings.Mail, _defaultemailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task SendToBackupEmailAsync(EmailDetails emailDetails)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_backupemailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(emailDetails.ToEmail));
            email.Subject = emailDetails.Subject;
            var builder = new BodyBuilder();
            if (emailDetails.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in emailDetails.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = emailDetails.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_backupemailSettings.Host, _backupemailSettings.Port, SecureSocketOptions.StartTls);
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
            smtp.Authenticate(_backupemailSettings.Mail, _backupemailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
