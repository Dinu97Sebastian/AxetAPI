using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bidding.API.Models;
using System.Net.Mail;
using Bidding.API.Helpers;

namespace Bidding.API.Services
{
    public class EmailService
    {
        private readonly IMongoCollection<EmailTemplate> email;
        private readonly IMongoCollection<OTP> otps;
        private readonly IMongoCollection<User> users;

        public EmailService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            email = database.GetCollection<EmailTemplate>(settings.EmailTemplatesName);
            users = database.GetCollection<User>(settings.UserCollectionName);
            otps = database.GetCollection<OTP>(settings.OtpCollectionName);
        }
        public string GetUser(string id, EmailViewModel model)
        {
            User user = new User();
            if (id == "admin")
            {
                user = users.Find<User>(match => match.UserType == "admin" && match.UserRole == "Admin").FirstOrDefault();
            }
            else
            {
                user = users.Find<User>(match => match.UserID == id).FirstOrDefault();
            }
            if (user == null)
                return null;
            var data = SendEmail(user.Username, model);
            return data;
        }

        public string SendEmail(string ToEmail, EmailViewModel model)
        {
            EmailTemplate EmailTemplate = email.Find<EmailTemplate>(match => match.TemplateType == model.TemplateType).FirstOrDefault();
            string BodyText = EmailTemplate.Body;
            string Subject = EmailTemplate.Subject;
            string otp = "";
            string senderId = "axet2020india@gmail.com"; // Sender EmailID
            string senderPassword = "axet123456789"; // Sender Password

            if (model.TemplateType == "OTP")
            {
                Random rnd = new Random();
                string OneTimePassword = rnd.Next(1000, 9999).ToString();
                // ViewState["msgotp"] = otp;
                otp = OneTimePassword;
                BodyText = BodyText.Replace("[SenderId]", senderId.Trim())
                .Replace("[OTP]", otp.Trim());
            }
            else
            {
                if(model.BuyerId != null || model.Code!=null || model.SupplierId != null) 
                {
                    if(model.TemplateType == "Supplier Accepted your RFQ")
                    {
                        BodyText = BodyText.Replace("[SenderId]", senderId.Trim()).Replace("[Code]", model.Code.Trim()).Replace("[Id]", model.SupplierId);
                    }
                    else
                    {
                        BodyText = BodyText.Replace("[SenderId]", senderId.Trim()).Replace("[Code]", model.Code.Trim());
                    }
                }
                else
                {
                    BodyText = BodyText.Replace("[SenderId]", senderId.Trim());
                }

            }

            bool f = SmtpEmail(BodyText, Subject, ToEmail, senderId, senderPassword);

            if (f)
            {
                if (model.TemplateType == "OTP")
                {
                    OTP Newotp = new OTP();
                    var temp = otps.Find<OTP>(match => match.Email == ToEmail).FirstOrDefault();
                    if (temp == null)
                    {
                        Newotp.Email = ToEmail;
                        Newotp.Otp = otp;
                        otps.InsertOne(Newotp);
                    }
                    else
                    {
                        temp.Otp = otp;
                        otps.ReplaceOne(match => match.Email == ToEmail, temp);
                    }
                }
                return "success";
            }
            else
            {
                return "fail";
            }
            //{ response.write("otp sent successfully"); }
            //else { response.write("otp not sent"); }
        }

        public bool SmtpEmail(string BodyText, string Subject, string ToEmail, string senderId, string senderPassword)//send email function
        {
            try
            {

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.To.Add(ToEmail);
                mailMessage.From = new MailAddress(senderId);

                mailMessage.Subject = Subject;
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;

                mailMessage.Body = BodyText;

                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.IsBodyHtml = true;

                mailMessage.Priority = MailPriority.High;

                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient();
                smtpClient.Credentials = new System.Net.NetworkCredential(senderId, senderPassword);
                smtpClient.Port = 587;
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.EnableSsl = true;

                object userState = mailMessage;

                try
                {
                    smtpClient.Send(mailMessage);
                    return true;
                }
                catch (System.Net.Mail.SmtpException)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<string> GetTemplate()
        {
            var EmailTemplate = email.Find<EmailTemplate>(match => true).ToList();
            List<string> template = new List<string>();
            foreach (var e in EmailTemplate)
            {
                template.Add(e.TemplateType);
            }
            return template;
        }

        public List<EmailTemplate> GetEmailTemplate(string templateType)
        {
            var EmailTemplate = email.Find<EmailTemplate>(match => match.TemplateType == templateType).ToList();
            return EmailTemplate.ToList();
        }
        public void Update(string id, EmailTemplate template)
        {
            email.ReplaceOne(match => match.Id == id, template);
        }

    }
           

}
