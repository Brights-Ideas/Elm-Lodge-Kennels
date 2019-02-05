using System;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using Elm_Lodge_Kennels.Models;
using Umbraco.Web.Mvc;

namespace Elm_Lodge_Kennels.Controllers
{
    public class ContactSurfaceController : SurfaceController
    {
        [HttpPost]
        public ActionResult SendEmail(ContactFormModel model)
        {
            //model not valid, do not save, but return current umbraco page
            if (!ModelState.IsValid)
            {
                //Perhaps you might want to add a custom message to the TempData or ViewBag
                //which will be available on the View when it renders (since we're not 
                //redirecting)          
                return CurrentUmbracoPage();
            }

            // Get current page properties
            var recipientProp = CurrentPage.GetProperty("recipientEmailAddress");
            var subjectProp = CurrentPage.GetProperty("emailSubject");
            var thankYouPageProp = CurrentPage.GetProperty("thankYouPage");
            var senderProp = CurrentPage.GetProperty("senderEmailAddress");

            if (recipientProp == null || recipientProp.Value == null || recipientProp.Value.ToString().Length == 0)
            {
                throw new MissingFieldException("The 'Recipient Email Address' property has not been completed");
            }

            if (subjectProp == null || subjectProp.Value == null || subjectProp.Value.ToString().Length == 0)
            {
                throw new MissingFieldException("The 'Email Subject' property has not been completed");
            }

            if (thankYouPageProp == null || thankYouPageProp.Value == null || thankYouPageProp.Value.ToString().Length == 0)
            {
                throw new MissingFieldException("The 'Thank You Page' property has not been completed");
            }

            if (senderProp == null || senderProp.Value == null || senderProp.Value.ToString().Length == 0)
            {
                throw new MissingFieldException("The 'Sender Email Address' property has not been completed");
            }

            SendSmtpEmail(recipientProp.Value.ToString(), senderProp.Value.ToString(), subjectProp.Value.ToString(), model);

            //Perhaps you might want to store some data in TempData which will be available 
            //in the View after the redirect below. An example might be to show a custom 'submit
            //successful' message on the View, for example:
            //TempData.Add("SubmissionMessage", "Your form was successfully submitted");

            //redirect to current page to clear the form
            //return RedirectToCurrentUmbracoPage();

            //Or redirect to specific page
            return RedirectToUmbracoPage(Convert.ToInt32(thankYouPageProp.Value));

        }

        private void SendSmtpEmail(string recipientEmail, string senderEmail, string subject, ContactFormModel model)
        {
            StringBuilder body = new StringBuilder();
            body.AppendLine("Hi,");
            body.AppendLine();
            body.AppendLine("Name: " + model.Name);
            body.AppendLine("Email: " + model.Email);
            body.AppendLine("Comment: " + model.Comment);
            body.AppendLine();
            body.AppendLine();
            body.AppendLine("Regards,");
            body.AppendLine();
            body.AppendLine("Elm Lodge Kennels");


            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(new MailAddress(recipientEmail));
            mailMessage.Sender = new MailAddress(senderEmail);
            mailMessage.From = new MailAddress(senderEmail);

            mailMessage.Body = body.ToString();
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = false;

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Send(mailMessage);
        }
    }
}