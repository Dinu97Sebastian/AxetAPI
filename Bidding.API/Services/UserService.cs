using Bidding.API.Helpers;
using Bidding.API.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bidding.API.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> users;
        private readonly IMongoCollection<Supplier> suppliers;
        private readonly IMongoCollection<Buyer> buyers;
        private readonly IMongoCollection<Sequence> sequence;
        private readonly IMongoCollection<OTP> otps;
        private readonly IMongoCollection<UserRole> userRoles;
        private readonly IMongoCollection<TermsAndConditions> termsAndConditions;
        private readonly IMongoCollection<EmailTemplate> EmailTemplate;
        private readonly IMongoCollection<AxetActions> axetAction;
        private readonly IMongoCollection<AxetNotification> axetNotification;

        public UserService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            users = database.GetCollection<User>(settings.UserCollectionName);
            sequence = database.GetCollection<Sequence>(typeof(Sequence).Name);
            suppliers = database.GetCollection<Supplier>(settings.SupplierCollectionName);
            EmailTemplate = database.GetCollection<EmailTemplate>(settings.EmailTemplatesName);
            buyers = database.GetCollection<Buyer>(settings.BuyerCollectionName);
            otps = database.GetCollection<OTP>(typeof(OTP).Name);
            userRoles = database.GetCollection<UserRole>(settings.UserRoleCollectionName);
            axetAction = database.GetCollection<AxetActions>(settings.AxetActionName);
            termsAndConditions = database.GetCollection<TermsAndConditions>(settings.TermsAndConditionsCollectionName);
            axetNotification = database.GetCollection<AxetNotification>(settings.AxetNotification);
        }

        public List<User> Get()
        {
            users.Find(user => true).ToList();

            var userData = from u in users.Find(user => true).ToList()
                           select new User
                           {
                               UserID=u.UserID,
                               Username=u.Username,
                               UserRole = u.UserRole,
                               CreatedDate = u.CreatedDate,
                               UserType = u.UserType,
                               Status=u.Status,
                           };
            return userData.ToList();
        }
           

        public User Get(string id)
        {
            User user = users.Find<User>(match => match.Id == id).FirstOrDefault();
            if (user == null)
                return null;
            return user.WithoutPassword();
        }

        public List<TermsAndConditions> GetTermsAndConditions() =>
            termsAndConditions.Find(termsAndConditions => true).ToList();

        public string CheckEmail(string email)
        {
            User user = users.Find<User>(match => match.Username == email).FirstOrDefault();
            if (user == null)
                return "User not found";
            return "Email exists";

        }

        public Dictionary<string, string> Authenticate(string userType, string username, string password)
        {
            var user = users.Find<User>(match => match.Username == username && match.UserType == userType && match.Status == "Active").FirstOrDefault();

            // return null if user not found
            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                return null;

            Dictionary<string, string> currentUser = new Dictionary<string, string>();

            var supplier = suppliers.Find<Supplier>(match => match.SupplierId == user.UserID).FirstOrDefault();
            var buyer = buyers.Find<Buyer>(match => match.BuyerId == user.UserID).FirstOrDefault();
            string name = null;
            string image = null;

            if (supplier != null)
            {
                name = supplier.SupplierName;
                image = supplier.Logo;
            }
            else
            {
                if (buyer != null)
                {
                    name = buyer.BuyerName;
                    image = buyer.Logo;
                }
            }

            currentUser.Add("UserId", user.UserID);
            currentUser.Add("Name", name);
            currentUser.Add("Role", user.UserRole);
            currentUser.Add("Logo", image);
            currentUser.Add("Email", user.Username);
            currentUser.Add("UserType", user.UserType);
            currentUser.Add("Status", user.Status);
            currentUser.Add("DeletedStatus", user.DeletedStatus.ToString());
            return currentUser;

            //return user.WithoutPassword();
            //return user;
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // Create hash using password salt.
                for (int i = 0; i < computedHash.Length; i++)
                { // Loop through the byte array
                    if (computedHash[i] != passwordHash[i]) return false; // if mismatch
                }
            }
            return true; //if no mismatches.
        }

        public User Create(UserViewModel model)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(model.Password, out passwordHash, out passwordSalt);
            User user = new User();
            user.UserType = model.UserType;
            user.Username = model.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.UserRole = model.UserRole;
            user.Status = model.Status;
            user.CreatedDate = model.CreatedDate;
            if (model.UserId == null)
                user.UserID = GetSequenceValue("UserId").ToString();
            else
                user.UserID = model.UserId;
            users.InsertOne(user);

            return user.WithoutPassword();
        }

        public List<UserRole> GetUserRole()
        {
            IEnumerable<UserRole> userRole = userRoles.Find(userRole => true).ToList();
            return userRole.ToList();
        }

        public int GetSequenceValue(string sequenceName)
        {

            var filter = Builders<Sequence>.Filter.Eq(s => s.SequenceName, sequenceName);
            var update = Builders<Sequence>.Update.Inc(s => s.SequenceValue, 1);

            var result = sequence.FindOneAndUpdate(filter, update, new FindOneAndUpdateOptions<Sequence, Sequence>
            { IsUpsert = true, ReturnDocument = ReturnDocument.After });

            return result.SequenceValue;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        public string SendEmail(string Email)
        {

            Random rnd = new Random();
            string OneTimePassword = rnd.Next(1000, 9999).ToString();
            // ViewState["msgotp"] = otp;
            string msg = OneTimePassword;
            bool f = SendOTP(Email, "OTP", msg);
            if (f)
            {
                OTP otp = new OTP();
                otp.Email = Email;
                otp.Otp = OneTimePassword;
                otps.InsertOne(otp);
                return "success";
            }
            else
            {
                return "fail";
            }
            //{ response.write("otp sent successfully"); }
            //else { response.write("otp not sent"); }
        }

        public bool SendOTP(string receivermailid, string subject, string bodyText)//send email function
        {
            try
            {
                string senderId = "axet2020india@gmail.com"; // Sender EmailID
                string senderPassword = "axet123456789"; // Sender Password

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.To.Add(receivermailid);
                mailMessage.From = new MailAddress(senderId);

                mailMessage.Subject = subject;
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;

                mailMessage.Body = @"<html>
                                    <body>
                                        <div style='border-style: solid;width: 90%;border-color: #black;border-radius: 1px;border-width: 1px;text-align:center'>
                                        <div style='background-color:#0000A0;color:white;height:9%'>
			                            <div style='background-color:#ffb81c;font-size:20px;padding-top:12px'><b>One Time Password</b></div>
		                                </div>
                                        <br/>
		                                <center><b> your otp from Axet team is</b> <h4>" + bodyText + @"</h4>

		                                </center>	<br/><br/>
                                        <div style='font-size:15px;padding-left:3%;padding-right:3%'>
		                                <i>This is an automatically generated email – please do not reply to it. If you have any queries please email to :
<span>" + senderId + @"</span></i>
                                        </ div >
                                        <br/><br/><br/>
		                                <div style='font-size:15px;padding-left:3%;padding-right:3%;text-align: justify;'>
			                            <i>NOTICE: The information contained in this message is proprietary and/or confidential and may be privileged.
			                            If you are not the intended recipient of this communication, you are hereby notified to: 
			                            (i) delete the message and all copies; (ii) do not disclose, distribute or use the message in any manner, 
			                            and (iii) notify the sender immediately.</i>
		                                </div> 
                                        </div>
                                        </body>
                                        </html>";
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

        public OTP CheckOtp(OTP model)
        {
            var otp = otps.Find<OTP>(match => match.Otp == model.Otp && match.Email == model.Email).FirstOrDefault();
            if (otp == null)
                return null;
            return otp;
        }

        //Method to handle Password change
        public User ChangePassword(PasswordChangeViewModel model)
        {
            User user = users.Find<User>(match => match.Username == model.Email).FirstOrDefault();
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(model.NewPassword, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            users.ReplaceOne(match => match.Username == model.Email, user);
            //IEnumerable<OTP> otp = otps.Find<OTP>(match => match.Email == model.Email).ToList();
            otps.DeleteMany(match => match.Email == model.Email);
            return user.WithoutPassword();
        }

        public void Update(string id, User user) =>
            users.ReplaceOne(match => match.Id == id, user);

        public void Remove(User user) =>
            users.DeleteOne(match => match.Id == user.Id);

        public void Remove(string id) =>
            users.DeleteOne(match => match.Id == id);

        public User UpdateUserStatus(UserStatus model)
        {
            User user = users.Find<User>(match => match.UserID == model.UserId).FirstOrDefault();
            user.Status = model.Status;
            user.DeletedStatus = model.DeletedStatus;
            user.ModifiedDate = model.ModifiedDate;
            user.UpdatedBy = model.UpdatedBy;
            users.ReplaceOne(match => match.UserID == model.UserId, user);
            return user.WithoutPassword();
        }

        //get supplier actions
        public List<AxetActions> GetSupplierActions()
        {
            IEnumerable<AxetActions> axetActions = axetAction.Find(userRole => userRole.UserType=="Supplier").ToList();
            return axetActions.ToList();
        }

        //update new users into supplier
        public Supplier UpdateSupplierUser(UsersList model,string Id)
        {
            Supplier supplier = suppliers.Find<Supplier>(match => match.SupplierId == Id).FirstOrDefault();
            var users = supplier.Users;
            List<UsersList> userList =supplier.Users;
            UsersList usr = new UsersList();
            usr.Email = model.Email;
            usr.Role = model.Role;
            usr.Actions = model.Actions;
            userList.Add(usr);
            supplier.Users = userList;
            suppliers.ReplaceOne(match => match.SupplierId == Id, supplier);
            return supplier;
        }

        //check user permission in supplier
        public AxetActions GetSupplierPermission(string id,string email,string action)
        {
            Supplier supplier = suppliers.Find<Supplier>(match => match.SupplierId == id).FirstOrDefault();
            var User = supplier.Users.FirstOrDefault(n => n.Email == email);
            var act = User.Actions.FirstOrDefault(n => n.ActionName == action && n.Permission == true);
            return act;
        }

        public User GetUserDetails( string email)
        {
            User user = users.Find<User>(match => match.Username == email).FirstOrDefault();
            return user.WithoutPassword();
        }
        public UsersList GetSuppleirUserDetails(string id, string email)
        {
            Supplier supplier = suppliers.Find<Supplier>(match => match.SupplierId == id).FirstOrDefault();
            var user = supplier.Users.FirstOrDefault(u => u.Email == email);

            return user;
        }


        public User UpdateUser(UserStatus model,string email)
        {
            User user = users.Find<User>(match => match.UserID == model.UserId && match.Username==email).FirstOrDefault();
            user.Status = model.Status;
            user.DeletedStatus = model.DeletedStatus;
            user.ModifiedDate = model.ModifiedDate;
            user.UpdatedBy = model.UpdatedBy;
            users.ReplaceOne(match => match.UserID == model.UserId && match.Username==email, user);
            return user.WithoutPassword();
        }

        //update new users into supplier
        public Supplier UpdateSupplierUsers(UsersList model, string Id)
        {
            Supplier supplier = suppliers.Find<Supplier>(match => match.SupplierId == Id).FirstOrDefault();

            var user = supplier.Users.Find(u=>u.Email == model.Email);

            user.Actions = model.Actions;
            suppliers.ReplaceOne(match => match.SupplierId == Id,supplier);

            return supplier;
        }

        //get supplier actions
        public List<AxetActions> GetBuyerActions()
        {
            IEnumerable<AxetActions> axetActions = axetAction.Find(userRole => userRole.UserType == "Buyer").ToList();
            return axetActions.ToList();
        }

        //update new users into Buyer
        public Buyer UpdateBuyerUser(UsersList model, string Id)
        {
            Buyer buyer = buyers.Find<Buyer>(match => match.BuyerId == Id).FirstOrDefault();
            var users = buyer.Users;
            List<UsersList> userList = buyer.Users;
            UsersList usr = new UsersList();
            usr.Email = model.Email;
            usr.Role = model.Role;
            usr.Actions = model.Actions;
            userList.Add(usr);
            buyer.Users = userList;
            buyers.ReplaceOne(match => match.BuyerId == Id, buyer);
            return buyer;
        }
        //Get Buyer user details
        public UsersList GetBuyerUserDetails(string id, string email)
        {
            Buyer buyer = buyers.Find<Buyer>(match => match.BuyerId == id).FirstOrDefault();
            var user = buyer.Users.FirstOrDefault(u => u.Email == email);
            return user;
        }

        //update new users into Buyer
        public Buyer UpdateBuyerUsers(UsersList model, string Id)
        {
            Buyer buyer = buyers.Find<Buyer>(match => match.BuyerId == Id).FirstOrDefault();

            var user = buyer.Users.Find(u => u.Email == model.Email);

            user.Actions = model.Actions;
            buyers.ReplaceOne(match => match.BuyerId == Id, buyer);

            return buyer;
        }

        //check user permission in Buyer
        public AxetActions GetBuyerPermission(string id, string email, string action)
        {
            Buyer buyer = buyers.Find<Buyer>(match => match.BuyerId == id).FirstOrDefault();
            var User = buyer.Users.FirstOrDefault(n => n.Email == email);
            var act = User.Actions.FirstOrDefault(n => n.ActionName == action && n.Permission == true);
            return act;
        }
        public List<AxetNotification> GetNotifications(string id)
        {
         //   var notification = axetNotification.Find<AxetNotification>(match => match.User == id);

            IEnumerable<AxetNotification> notification = axetNotification.Find(u => u.User== id).ToList();
            return notification.ToList();
        }


        public List<AxetNotification> UpdateUserNotification(string Id)
        {
            List< AxetNotification> nots = axetNotification.Find(w => w.User == Id).ToList();

            foreach (AxetNotification p in nots)
            {
                p.Status = "Read";
            }
            foreach (AxetNotification p in nots)
            {
               
                axetNotification.ReplaceOneAsync(match => match.Id == p.Id,p );
            }

                return nots;
        }

        public User CheckUser(PasswordChangeViewModel model, out string Result)
        {
            var email = users.Find<User>(match => match.Username == model.Email).FirstOrDefault();
            // return null if email not found
            if (email == null)
            {
                Result = "Email not found";
                return null;
            }
                
            // If user exists, verifying the Old Password
            if (!VerifyPassword(model.OldPassword, email.PasswordHash, email.PasswordSalt))
            {
                Result = "Password does not match";
                return null;
            }
            var user = ChangePassword(model);
            if(user == null)
            {
                Result = "Error in updating";
                return null;
            }
            Result = "Updated successfully";
            return user;
        }
    }
}