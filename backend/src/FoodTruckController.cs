using System;
using Microsoft.AspNetCore.Mvc;


namespace Richmond
{
    public class FoodTruckController
    {
        private readonly IFoodTruckRepository _sessionRepository;

        public FoodTruckController(IFoodTruckRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        [HttpGet("foodtrucks")]
        public IActionResult FoodTrucks()
        {

            var response = new FoodTruckResponse
            {
                FoodTrucks = _sessionRepository.ParseFoodTruckSite(
                _sessionRepository.RequestFoodTruckWebsite())
            };
            return new OkObjectResult(response);

        }


    }
}
