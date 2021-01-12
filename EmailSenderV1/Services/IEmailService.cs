using EmailSenderV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailSenderV1.Services
{
    public interface IEmailService
    {
        Task SendToDefaultEmailAsync(EmailDetails emailDetails);
        Task SendToBackupEmailAsync(EmailDetails emailDetails);
    }
}
