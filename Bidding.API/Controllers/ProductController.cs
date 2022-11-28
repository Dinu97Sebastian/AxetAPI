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
    public class ProductController : Controller
    {
        private readonly ProductService productService;

        public ProductController(ProductService service)
        {
            productService = service;
        }

        [HttpGet]
        public ActionResult<List<Product>> Get()
        {
           var productData = productService.Get();
            return Json(new { productData });
        }

        [AuthorizePermission]
        [Route("controller/{id}")]
        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        public ActionResult<Product> Get(string id)
        {
            var productData = productService.Get(id);

            if (productData == null)
            {
                return NotFound();
            }

            return Json(new { productData });
        }

        [AuthorizePermission]
        [Route("GetSupplierProduct/{userid}")]
        public ActionResult GetSupplierProduct(string userid)
        {
            //string suserid = userid.ToString();
            var productList = productService.GetSupplierProduct(userid);
            return Json(new { productList });
        }

        [AuthorizePermission]
        [HttpPost]
        public ActionResult<Product> Create(Product product)
        {
            product.Rating = new List<int>();
            productService.Create(product);
            //return CreatedAtRoute("GetProduct", new { id = product.Id.ToString() }, product);
            return Json(new { data="Success"});
        }

        [AuthorizePermission]
        [Route("controller/{id}")]
        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, Product product)
        {
            if (productService.Get(id) == null)
            {
                return NotFound();
            }
            productService.Update(id, product);
            return Json(new { data = "Success" });
        }

        [Route("controller/{id}")]
        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var product = productService.Get(id);
            if (product == null)
            {
                return NotFound();
            }
           // productService.Remove(product.Id);
            return NoContent();
        }

        [AuthorizePermission]
        [HttpPost("UpdateProductstatus")]
        public IActionResult UpdateProductstatus([FromBody]ProductStatus model)
        {
            var product = productService.UpdateProductStatus(model);
            if (product == null)
                return Json(new { data = "Error" });
            return Json(new { data = "Success" });
        }
    }
}
