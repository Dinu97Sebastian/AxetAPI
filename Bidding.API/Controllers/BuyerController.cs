using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bidding.API.Models;
using Bidding.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bidding.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[AuthorizePermission]
    public class BuyerController : Controller
    {
        private readonly BuyerService buyerService;

        public BuyerController(BuyerService service)
        {
            buyerService = service;
        }

        [AuthorizePermission]
        [HttpGet]
        public ActionResult<List<Buyer>> Get()
        {
            var buyer = buyerService.Get();
            return buyer.ToList();
        }
      

        [Route("GetBuyer/{id}")]
        public ActionResult<Buyer> Get(string id)
        {
            var buyer = buyerService.Get(id);

            if (buyer == null)
            {
                return NotFound();
            }

            return Json(new { buyer });
        }

        [HttpPost]
        public ActionResult<Buyer> Create(Buyer buyer)
        {
            buyerService.Create(buyer);
            return Json(new { data = "Success" });
            //return CreatedAtRoute("GetBuyer", new { id = buyer.Id.ToString() }, buyer);
        }

        [Route("GetBuyer/{id}")]
        [HttpPut]
        public IActionResult Update(string id, Buyer buyer)
        {
            if (buyerService.Get(id) == null)
            {
                return NotFound();
            }
            buyerService.Update(id, buyer);
            return Json(new { data = "Success" });
        }

        [Route("controller/{id}")]
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var buyer = buyerService.Get(id);
            if (buyer == null)
            {
                return NotFound();
            }
            //buyerService.Remove(buyer.Id);
            return NoContent();
        }
    }
}
