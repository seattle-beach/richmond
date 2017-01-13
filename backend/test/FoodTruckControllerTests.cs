using Microsoft.AspNetCore.Mvc;
using Xunit;
using LightMock;

namespace Richmond.Tests
{
    public class FoodTruckControllerTests
    {
        [Fact]
        public void ReturnsNonemptyHttpResponse()
        {
            var mockContext = new MockContext<IFoodTruckRepository>();
            var foodTruckRepositoryMock = new FoodTruckRepositoryMock(mockContext);
            mockContext.Arrange(f => f.RequestFoodTruckWebsite()).Returns("");
            mockContext.Arrange(f => f.ParseFoodTruckSite("")).Returns(new FoodTruckResponse { FoodTrucks = new FoodTruckResponse.FoodTruck[4], Date = "Thursday, 12 January 2017" });

            // inject a thing which gets the web site data for us (here, mock the return value)
            var subject = new FoodTruckController(foodTruckRepositoryMock);

            var result = subject.FoodTrucks();

            var response = result as OkObjectResult;
            Assert.NotNull(response);

            var payload = response.Value as FoodTruckResponse;
            Assert.NotNull(payload);

            Assert.Equal(4, payload.FoodTrucks.Count);
        }

    }

    public class FoodTruckRepositoryMock : IFoodTruckRepository
    {
        private readonly IInvocationContext<IFoodTruckRepository> context;
        public FoodTruckRepositoryMock(IInvocationContext<IFoodTruckRepository> context)
        {
            this.context = context;
        }

        public FoodTruckResponse ParseFoodTruckSite(string html)
        {
            return context.Invoke(f => f.ParseFoodTruckSite(html));
        }
        public string RequestFoodTruckWebsite()
        {
            return context.Invoke(f => f.RequestFoodTruckWebsite());
        }
    }
}