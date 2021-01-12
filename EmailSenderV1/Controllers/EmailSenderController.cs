using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailSenderV1.Models;
using EmailSenderV1.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmailSenderV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailSenderController : ControllerBase
    {
        private readonly IEmailService emailService;
        public EmailSenderController(IEmailService emailService)
        {
            this.emailService = emailService;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromForm] EmailDetails request)
        {
            try
            {
                await emailService.SendToDefaultEmailAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                try
                {
                    await emailService.SendToBackupEmailAsync(request);
                    return Ok();
                }
                catch
                {
                    return BadRequest(ex.Message);
                }
               
            }

        }
    }
}
