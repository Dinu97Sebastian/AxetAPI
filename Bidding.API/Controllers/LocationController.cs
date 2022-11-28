using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bidding.API.Models;
using Bidding.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bidding.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    //[AuthorizePermission]
    public class LocationController : Controller
    {
        private readonly LocationService locationService;

        public LocationController(LocationService service)
        {
            locationService = service;
        }

        [HttpGet]
        public ActionResult<List<Country>> GetCountries()
        {
            var countryData = locationService.GetCountries();
            return Json(new { countryData });
        }


        [Route("States/{countryId}")]
        public ActionResult<List<State>> GetStates(string countryId)
        {
            var stateData = locationService.GetStates(countryId);
            return Json(new { stateData });
        }

        [HttpGet]
        [Route("Cities/{stateId}")]
        public ActionResult<List<City>> GetCities(string stateId)
        {
            var cityData = locationService.GetCities(stateId);
            return Json(new { cityData });
        }

    }
}
