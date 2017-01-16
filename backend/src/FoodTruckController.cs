using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;


namespace Richmond
{

    [EnableCors("SiteCorsPolicy")]
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
            var response = _sessionRepository.ParseFoodTruckSite(_sessionRepository.RequestFoodTruckWebsite());
            return new OkObjectResult(response);
        }

    }
}
