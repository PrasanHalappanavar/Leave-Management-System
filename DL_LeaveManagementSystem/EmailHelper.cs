using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DL_LeaveManagementSystem
{
    public sealed class EmailHelper
    {
        private readonly IConfiguration _config;
        private static EmailHelper _instance = null;

        public EmailHelper(IConfiguration config)
        {
            _config = config;
        }

        public static EmailHelper GetEmailInstance(IConfiguration config)
        {
            if (_instance == null)
            {
                _instance = new EmailHelper(config);
            }

            return _instance;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var smtp = new SmtpClient
            {
                Host = _config["EmailSettings:SmtpServer"],
                Port = int.Parse(_config["EmailSettings:Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _config["EmailSettings:Username"],
                    _config["EmailSettings:Password"]
                )
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:From"], "SSDB Tech"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);
            smtp.Send(mail);
        }
    }

}
