using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using JasperSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JasperSite.Areas.Admin.Controllers
{

    [AllowAnonymous]
    [Area("Admin")]
    public class MailController : Controller
    {
        [HttpPost]
        public IActionResult Index(string sender, string subject, string body, string returnUrl, string email)
        {
            try
            {              

                if(!Configuration.GlobalWebsiteConfig.EnableEmail) // Email services not activated
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    string smtpserver = Configuration.GlobalWebsiteConfig.GetEmailProperties().SmtpServer;
                    string username = Configuration.GlobalWebsiteConfig.GetEmailProperties().Username;
                    string password = Configuration.GlobalWebsiteConfig.GetEmailProperties().Password;
                    string from = Configuration.GlobalWebsiteConfig.GetEmailProperties().From;
                    string to = Configuration.GlobalWebsiteConfig.GetEmailProperties().To;
                    

                    SmtpClient client = new SmtpClient(smtpserver);
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(username, password);

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.Sender = new MailAddress(from);
                    mailMessage.To.Add(to);
                    mailMessage.Body = sender + "\n" + email + "\n" + body;
                    mailMessage.Subject = subject;
                    mailMessage.From = new MailAddress(from, sender);
                    client.Send(mailMessage);
                    TempData["MailSuccess"] = true;

                }              

            }
            catch
            {
                TempData["MailSuccess"] = false;

                /* In case of error the form will remember all the submitted values */
                TempData["Message"] = body != null ? body : string.Empty;
                TempData["Sender"] = sender != null ? sender : string.Empty;
                TempData["Subject"] = subject != null ? subject : string.Empty;
                TempData["Email"] = email != null ? email : string.Empty;

            }

            return Redirect(returnUrl);

        }
        
          
        }
    }
