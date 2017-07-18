using System.Collections.Generic;
using Xunit;
using System;
using Newtonsoft.Json;

namespace Richmond.Tests
{
    public class FoodTruckRepositoryTests
    {
        private RepositoryTestHelper repositoryTestHelper;
        
        public FoodTruckRepositoryTests()
        {
            repositoryTestHelper = new RepositoryTestHelper();
        }

        [Fact]
        public void ParsesFoodTruckDataBeforeLunchtime()
        {
            var dateProvider = new MockDateProvider(DateTime.Parse("2017-07-17 12:00"));
            var subject = new FoodTruckRepository(dateProvider);

            var json = repositoryTestHelper.GetFileFixtureData("food_truck_fixture.json");
            var result = subject.ParseFoodTruckSite(json);

            IList<FoodTruckResponse.FoodTruck> expectedResponse = new FoodTruckResponse.FoodTruck[4];

            Assert.Equal("Monday, 17 July 2017", result.Date);
            Assert.Equal("Monday", result.DayOfWeek);

            expectedResponse[0] = new FoodTruckResponse.FoodTruck { Name = "Chick'n Fix", Type = "None" };
            expectedResponse[1] = new FoodTruckResponse.FoodTruck { Name = "Where Ya At Matt", Type = "Southern" };

            for (var i = 0; i < 2; i++)
            {
                Assert.Equal(expectedResponse[i], result.FoodTrucks[i]);
            }
        }

        [Fact]
        public void ParsesTomorrowsFoodTruckDataAfter2pmLocalTime()
        {
            var dateProvider = new MockDateProvider(DateTime.Parse("2017-07-17 14:01"));
            var subject = new FoodTruckRepository(dateProvider);

            var json = repositoryTestHelper.GetFileFixtureData("food_truck_fixture.json");
            var result = subject.ParseFoodTruckSite(json);

            IList<FoodTruckResponse.FoodTruck> expectedResponse = new FoodTruckResponse.FoodTruck[4];

            expectedResponse[0] = new FoodTruckResponse.FoodTruck { Name = "Sam Choy's Poke To The Max", Type = "Hawaiian" };
            expectedResponse[1] = new FoodTruckResponse.FoodTruck { Name = "Snout and Co", Type = "BBQ" };

            for (var i = 0; i < 2; i++)
            {
                Assert.Equal(expectedResponse[i], result.FoodTrucks[i]);
            }
        }

        [Fact]
        public void ReturnsFoodTruckDataSkippingTheWeekend()
        {
            // Shows the next days' Food trucks after 2PM Mon-Thursday except Friday when it shows it all day long. If its a weekend shows Monday data.
            var dateProvider = new MockDateProvider(DateTime.Parse("2017-07-22"));
            var subject = new FoodTruckRepository(dateProvider);

            var json = repositoryTestHelper.GetFileFixtureData("food_truck_fixture.json");
            var result = subject.ParseFoodTruckSite(json);

            IList<FoodTruckResponse.FoodTruck> expectedResponse = new FoodTruckResponse.FoodTruck[4];

            expectedResponse[0] = new FoodTruckResponse.FoodTruck { Name = "Chick'n Fix", Type = "None" };
            expectedResponse[1] = new FoodTruckResponse.FoodTruck { Name = "Where Ya At Matt", Type = "Southern" };

            for (var i = 0; i < 2; i++)
            {
                Assert.Equal(expectedResponse[i], result.FoodTrucks[i]);
            }
        }

        [Fact]
        public void HandlesMissingNextWeekDataGracefully()
        {

        }

        [Fact]
        public void RequestsFoodTruckWebsite()
        {
            var httpHandler = repositoryTestHelper.GetMockHttpMessageHandler("my content");
            var subject = new FoodTruckRepository(null, httpHandler, "https://food.example.com/trucks");
            var result = subject.RequestFoodTruckWebsite();

            Assert.Equal("my content", result);
            Assert.Equal("https://food.example.com/trucks", httpHandler.requestedUri.ToString());
        }
    }
}