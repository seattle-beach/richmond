using Microsoft.AspNetCore.Mvc;

namespace Richmond
{
    public class FoodTruckController
    {
        private readonly IFoodTruckRepository _foodTruckRepository;

        public FoodTruckController(IFoodTruckRepository foodTruckRepository)
        {
            _foodTruckRepository = foodTruckRepository;
        }

        [HttpGet("foodtrucks")]
        public IActionResult FoodTrucks()
        {
            var response = _foodTruckRepository.ParseFoodTruckSite(_foodTruckRepository.RequestFoodTruckWebsite());
            return new OkObjectResult(response);
        }
    }
}
