using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bidding.API.Services;
using Bidding.API.Models;

namespace Bidding.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DashboardController : Controller
    {
        private readonly DashboardService dashboardService;
        public DashboardController(DashboardService service)
        {
            dashboardService = service;
        }

        //High demanding categories
        [AuthorizePermission]
        [HttpPost("GetCategoryCount")]
        public IActionResult GetCategoryRfqCount([FromBody]DashboardViewModel model)
        {
            var count = dashboardService.GetCategoryRfqCount(model);
            return Json(new { count });
        }
        //Supplier Id - Number of Products
        [AuthorizePermission]
        [Route("GetProductCount/{id}")]
        public IActionResult GetProductCount(string id)
        {
            var product = dashboardService.GetProductCount(id);
            return Json(new { product });
        }

        //Supplier - Total RFQs received
        [AuthorizePermission]
        [Route("GetSupplierRfqCount/{id}")]
        public IActionResult GetSupplierRfq(string id)
        {
            var count = dashboardService.GetSupplierRfqCount(id);
            return Json(new { count });
        }

        //High demanding products
        [AuthorizePermission]
        [HttpPost("GetProductRfqCount")]
        public IActionResult GetProductRfqCount([FromBody]DashboardViewModel model) 
        {
            var count = dashboardService.GetProductRfqCount(model);
            return Json(new { count });
        }
        [AuthorizePermission]
        [Route("GetBuyerRfqCount/{id}")]
        public IActionResult GetBuyerRfqCount(string id)
        {
            var rfqCount = dashboardService.GetBuyerRfqCount(id);
            return Json(new { rfqCount });
        }

        [AuthorizePermission]
        [HttpPost("GetBuyerProductRfqCount")]
        public IActionResult GetBuyerProductRfqCount([FromBody]DashboardViewModel model)
        {
            var rfqCount = dashboardService.GetBuyerProductRfqCount(model);
            return Json(new { rfqCount });
        }

        [AuthorizePermission]
        [HttpPost("GetBuyerMonthlyRfqData")]
        public IActionResult GetMonthlyRfqData([FromBody]DashboardViewModel model)
        {
            var monthlyRfqData = dashboardService.GetBuyerMonthlyRfqData(model);
            return Json(new { monthlyRfqData });
        }
        [AuthorizePermission]
        [HttpPost("GetSupplierMonthlyRfqData")]
        public IActionResult GetSupplierMonthlyRfqData([FromBody]DashboardViewModel model)
        {
            var monthlyRfqData = dashboardService.GetSupplierMonthlyRfqData(model);
            return Json(new { monthlyRfqData });
        }

        [AuthorizePermission]
        [HttpPost("GetProductRating")]
        public IActionResult GetProductRating([FromBody]DashboardViewModel model)
        {
            var productData = dashboardService.GetProductRating(model);
            return Json(new { productData });
        }
        [AuthorizePermission]
        [Route("GetRFQRating/{id}")]
        public IActionResult GetRFQRating(string id)
        {
            var rfqData = dashboardService.GetRFQRating(id);
            return Json(new { rfqData });
        }
    }
}
