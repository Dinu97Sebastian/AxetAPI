using Bidding.API.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bidding.API.Services
{
    public class LocationService
    {
        private readonly IMongoCollection<Country> countries;
        private readonly IMongoCollection<State> states;
        private readonly IMongoCollection<City> cities;

        public LocationService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            countries = database.GetCollection<Country>(settings.CountryCollectionName);
            states = database.GetCollection<State>(settings.StateCollectionName);
            cities = database.GetCollection<City>(settings.CityCollectionName);
        }

        public List<Country> GetCountries() =>
           countries.Find(country => true).ToList();

        public List<State> GetStates(string countryId) =>
            states.FindSync(state => state.CountryId == countryId).ToList();

        public List<City> GetCities(string stateId) =>
            cities.Find(city => city.StateId == stateId).ToList();


    }
}
