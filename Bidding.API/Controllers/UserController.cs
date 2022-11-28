using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Bidding.API.Models;
using Bidding.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bidding.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserService userService;

        public UserController(UserService service)
        {
            userService = service;
        }

        [AuthorizePermission]
        [HttpGet("GetUserRole")]
        public ActionResult<List<UserRole>> GetUserRole()
        {
            var userRole = userService.GetUserRole();
            return Json(new { userRole });
        }

        [AuthorizePermission]
        [HttpGet]
        public ActionResult<List<User>> Get() =>
            userService.Get();

        //[AuthorizePermission]
        [Route("controller/{id}")]
        [HttpGet("{id:length(24)}", Name = "GetUser")]
        public async Task<Models.User> Get(string id)
        {
            var user = userService.Get(id);

            if (user == null)
            {
                return null;
            }
            return user;
        }

        [HttpGet("GetTermsAndConditions")]
        public ActionResult<List<TermsAndConditions>> GetTermsAndConditions()
        {
            var termsAndConditions = userService.GetTermsAndConditions();
            return Json(new { termsAndConditions });
        }

        [AuthorizePermission]
        [HttpGet]
        [Route("CheckEmail/{email}")]
        public IActionResult CheckEmail(string email)
        {
            string result = userService.CheckEmail(email);
            if (result == "User not found")
                return Json(new { data = "" });
            return Json(new { data = "User exists" });
        }
        
        //Check for existing email dring registration
        [HttpGet]
        [Route("CheckRegistrationEmail/{email}")]
        public IActionResult CheckRegistrationEmail(string email)
        {
            string result = userService.CheckEmail(email);
            if (result == "User not found")
                return Json(new { data = "" });
            return Json(new { data = "User exists" });
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserViewModel model)
        {
            var user = userService.Authenticate(model.UserType, model.Username, model.Password);
            if (user == null)
                return Json(new { data = "Incorrect login credentials" });

            return Json(new
            {
                data = "Success",
                CurrentUser = user,
                Message = TokenManager.GenerateToken(model.Username)
            });
        }

        [AuthorizePermission]
        [HttpPost]
        public ActionResult<User> Create(UserViewModel userdto)
        {
            User user = userService.Create(userdto);
            //return CreatedAtRoute("GetUser", new { id = user.Id.ToString() }, user);
            //return Json(new { data = "Success"});
            return Json(new { user });
        }

        //[AuthorizePermission]
        [Route("controller/{id}")]
        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, User user)
        {
            if (userService.Get(id) == null)
            {
                return NotFound();
            }
            userService.Update(id, user);
            return NoContent();
        }

        //[AuthorizePermission]
        [Route("controller/{id}")]
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var user = userService.Get(id);
            if (user == null)
            {
                return NotFound();
            }
            userService.Remove(user.Id);
            return NoContent();
        }

        [AuthorizePermission]
        [HttpPost("UpdateStatus")]
        public IActionResult UpdateUserStatus([FromBody]UserStatus model)
        {
            var user = userService.UpdateUserStatus(model);
            if (user == null)
                return null;
            return Json(new { data = "Success" });
        }

        [HttpGet]
        [Route("OTP/{email}")]
        public IActionResult SendOtp(string email)
        {
            string data = userService.SendEmail(email);

            return Json(new { data = data });
        }

        [HttpPost("CheckOtp")]
        public IActionResult CheckOtp([FromBody]OTP model)
        {
            var otp = userService.CheckOtp(model);
            if (otp == null)
                return Json(new { data = "Wrong OTP" });
            return Json(new { data = "Success" });
        }
        //Method used for Forgot Password
        [HttpPost("ChangePassword")]
        public IActionResult ChangePassword([FromBody]PasswordChangeViewModel model)
        {
            var user = userService.ChangePassword(model);
            if (user == null)
                return null;
            return Json(new { data = "Success" });
        }

        //get supplier actions
        [AuthorizePermission]
        [HttpGet]
        [Route("SupplierAction")]
        public IActionResult GetSupplierActions()
        {
            var data = userService.GetSupplierActions();

            return Json(new { data = data });
        }
        
        //update added user into supplier
        [AuthorizePermission]
        [HttpPost("UpdateSupplierUsers/{id}")]
        public IActionResult UpdateUser([FromBody]UsersList model,string id)
        {
            var rfq = userService.UpdateSupplierUser(model,id);
            if (rfq == null)
                return Json(new { data = "Error" });
            return Json(new { data = "Success" });
        }
        

        //check user permission for supplier
        [AuthorizePermission]
        [HttpGet]
        [Route("Permission/{UserId}/{Email}/{act}")]
        public IActionResult GetSupplierPermission(string UserId, string Email, string act)
        {
            var data = userService.GetSupplierPermission(UserId, Email,act);
            if (data == null)
       
                return Json(new { data = "fail" });
         
            return Json(new { data = "success" });
        }
        [AuthorizePermission]
        [HttpGet]
        [Route("SupplierUser/{UserId}/{Email}")]
        public IActionResult GetSuppleirUserDetails(string UserId, string Email)
        {
            var userdata = userService.GetUserDetails(Email);
            var supplierUser= userService.GetSuppleirUserDetails(UserId,Email);

            return Json(new { Userdata = userdata,SupplierUser= supplierUser });
        }

        [HttpPost("UpdateUser/{email}")]
        public IActionResult UpdateUser([FromBody]UserStatus model,string email)
        {
            var user = userService.UpdateUser(model,email);
            if (user == null)
                return null;
            return Json(new { data = "Success" });
        }

        //update added user into supplier
        [AuthorizePermission]
        [HttpPost("UpdateSupplierUsersData/{id}")]
        public IActionResult UpdateSupplierUsers([FromBody]UsersList model, string id)
        {
            var rfq = userService.UpdateSupplierUsers(model, id);
            if (rfq == null)
                return Json(new { data = "Error" });
            return Json(new { data = "Success" });
        }

        //get Buyer actions
        [AuthorizePermission]
        [HttpGet]
        [Route("BuyerActions")]
        public IActionResult GetBuyerActions()
        {
            var data = userService.GetBuyerActions();

            return Json(new { data = data });
        }

        //update added user into supplier
        [AuthorizePermission]
        [HttpPost("UpdateBuyerUsers/{id}")]
        public IActionResult UpdateBuyerUser([FromBody]UsersList model, string id)
        {
            var rfq = userService.UpdateBuyerUser(model, id);
            if (rfq == null)
                return Json(new { data = "Error" });
            return Json(new { data = "Success" });
        }

        [AuthorizePermission]
        [HttpGet]
        [Route("BuyerUser/{UserId}/{Email}")]
        public IActionResult GetBuyerUserDetails(string UserId, string Email)
        {
            var userdata = userService.GetUserDetails(Email);
            var supplierUser = userService.GetBuyerUserDetails(UserId, Email);

            return Json(new { Userdata = userdata, BuyerUser = supplierUser });
        }

        //update added user into supplier
        [AuthorizePermission]
        [HttpPost("UpdateBuyerUsersData/{id}")]
        public IActionResult UpdateBuyerUsers([FromBody]UsersList model, string id)
        {
            var rfq = userService.UpdateBuyerUsers(model, id);
            if (rfq == null)
                return Json(new { data = "Error" });
            return Json(new { data = "Success" });
        }

        //check user permission for supplier
        [AuthorizePermission]
        [HttpGet]
        [Route("BuyerPermission/{UserId}/{Email}/{act}")]
        public IActionResult GetBuyerPermission(string UserId, string Email, string act)
        {
            var data = userService.GetBuyerPermission(UserId, Email, act);
            if (data == null)

                return Json(new { data = "fail" });

            return Json(new { data = "success" });
        }

        [HttpGet]
        [Route("GetNotifications/{UserId}")]
        public IActionResult GetNotifications(string UserId)
        {
            var data = userService.GetNotifications(UserId);
          

            return Json(new {NotificationData = data });
        }

        [HttpGet("UpdateNotification/{id}")]
        public IActionResult UpdateUserNotification(string id)
        {
            var date = userService.UpdateUserNotification(id);

            return Json(new { NotificationData = date });
        }

        [AuthorizePermission]
        [HttpPost("CheckUser")]
        public IActionResult CheckUser([FromBody]PasswordChangeViewModel model)
        {
            var user = userService.CheckUser(model, out var result);
            if (result == "Email not found" || result == "Password does not match")
                return Json(new { message = "Email and Old Password do not match" });
            if(result== "Error in updating")
                return Json(new { message = result });
            return Json(new { message = "Success" });
        }

         [HttpGet]
        [Route("UnAutherizedCheckEmail/{email}")]
        public IActionResult UnAutherizedCheckEmail(string email)
        {
            string result = userService.CheckEmail(email);
            if (result == "User not found")
                return Json(new { data = "" });
            return Json(new { data = "User exists" });
        }

        [HttpPost]
        [Route("UnAutherizedAddUser")]
        public ActionResult<User> UnAutherizedAddUser(UserViewModel userdto)
        {
            User user = userService.Create(userdto);
            //return CreatedAtRoute("GetUser", new { id = user.Id.ToString() }, user);
            //return Json(new { data = "Success"});
            return Json(new { user });
        }

    }
}


