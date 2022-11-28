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
    public class SupplierController : Controller
    {
        private readonly SupplierService supplierService;

        public SupplierController(SupplierService service)
        {
            supplierService = service;
        }

        [AuthorizePermission]
        [HttpGet]
        public ActionResult<List<Supplier>> Get() =>
            supplierService.Get();

        [AuthorizePermission]
        [Route("GetSupplier/{id}")]
        public ActionResult<Supplier> Get(string id)
        {
            var supplier = supplierService.Get(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return Json(new { supplier });
        }

        [HttpPost]
        public ActionResult<Supplier> Create(Supplier supplier)
        {
            supplierService.Create(supplier);
            // return CreatedAtRoute("GetSupplier", new { id = supplier.Id.ToString() }, supplier);
            return Json(new { data = "Success" });
        }

        [AuthorizePermission]
        [Route("GetSupplier/{id}")]
        [HttpPut]
        public IActionResult Update(string id, Supplier supplier)
        {
            if (supplierService.Get(id) == null)
            {
                return NotFound();
            }
            supplierService.Update(id, supplier);
            return Json(new { data = "Success" });
        }

        [Route("controller/{id}")]
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var supplier = supplierService.Get(id);
            if (supplier == null)
            {
                return NotFound();
            }
            //supplierService.Remove(supplier.Id);
            return NoContent();
        }
    }
}
