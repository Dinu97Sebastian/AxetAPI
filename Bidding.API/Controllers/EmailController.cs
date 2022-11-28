using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bidding.API.Services;
using Bidding.API.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Bidding.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly EmailService emailService;

        public EmailController(EmailService service)
        {
            emailService = service;
        }

        [HttpPost("EmailTemplate/{email}")]
        public IActionResult SendEmail(string email, [FromBody]EmailViewModel model)
        {
            string data = emailService.SendEmail(email, model);
            return Json(new { data = data });
        }

        [AuthorizePermission]
        [HttpPost("GetAndSendEmail/{userId}")]
        public IActionResult GetUserEmail(string userId, [FromBody]EmailViewModel model)
        {
            var data = emailService.GetUser(userId, model);
            return Json(new { data = data });
        }

        [AuthorizePermission]
        [HttpGet]
        [Route("TemplateList")]
        public IActionResult GetTemplateList()
        {
            var data = emailService.GetTemplate();

            return Json(new { data = data });
        }
        [AuthorizePermission]
        [HttpGet]
        [Route("GetTemplate/{templateType}")]
        public IActionResult GetTemplate(string TemplateType)
        {
            var data = emailService.GetEmailTemplate(TemplateType);

            return Json(new { data = data });
        }

        [Route("controller/{type}")]
        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, EmailTemplate template)
        {
            emailService.Update(id, template);
            return Json(new { data = "Success" });
        }

    }
}
