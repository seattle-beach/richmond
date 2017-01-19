using System.Collections.Generic;
using Xunit;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using System;

namespace Richmond.Tests
{
    public class FoodTruckRepositoryTests
    {
        [Fact]
        public void ParsesFoodTruckData()
        {
            var dateProvider = new MockDateProvider(DateTime.Parse("2017-01-12 12:00"));
            var subject = new FoodTruckRepository(dateProvider);

            var html = GetFoodTruckFixtureData();
            var result = subject.ParseFoodTruckSite(html);

            IList<FoodTruckResponse.FoodTruck> expectedResponse = new FoodTruckResponse.FoodTruck[4];

            expectedResponse[0] = new FoodTruckResponse.FoodTruck { Name = "Beez Kneez Gourmet Sausages", Type = "Hot Dogs" };
            expectedResponse[1] = new FoodTruckResponse.FoodTruck { Name = "Seattle Chicken Over Rice", Type = "Mediterranean" };
            expectedResponse[2] = new FoodTruckResponse.FoodTruck { Name = "Bomba Fusion", Type = "Asian" };
            expectedResponse[3] = new FoodTruckResponse.FoodTruck { Name = "Neema's Comfort Food", Type = "Southern" };


            for (var i = 0; i < 4; i++)
            {
                Assert.Equal(expectedResponse[i], result.FoodTrucks[i]);
            }
        }

        [Fact]
        public void ParsesTomorrowsFoodTruckData()
        {
            var dateProvider = new MockDateProvider(DateTime.Parse("2017-01-12 14:01"));
            var subject = new FoodTruckRepository(dateProvider);

            var html = GetFoodTruckFixtureData();
            var result = subject.ParseFoodTruckSite(html);

            IList<FoodTruckResponse.FoodTruck> expectedResponse = new FoodTruckResponse.FoodTruck[4];

            expectedResponse[0] = new FoodTruckResponse.FoodTruck { Name = "Hallava Falafel", Type = "Mediterrannean" };
            expectedResponse[1] = new FoodTruckResponse.FoodTruck { Name = "El Cabrito Oaxaca", Type = "Mexican" };
            expectedResponse[2] = new FoodTruckResponse.FoodTruck { Name = "Wet Buns", Type = "Sandwiches" };
            expectedResponse[3] = new FoodTruckResponse.FoodTruck { Name = "Bomba Fusion", Type = "Asian" };


            for (var i = 0; i < 4; i++)
            {
                Assert.Equal(expectedResponse[i], result.FoodTrucks[i]);
            }
        }

        [Fact]
        public void ParsesFoodTruckDataOverWeekends()
        {
            var dateProvider = new MockDateProvider(DateTime.Parse("2017-01-14"));
            var subject = new FoodTruckRepository(dateProvider);

            var html = GetFoodTruckFixtureData();
            var result = subject.ParseFoodTruckSite(html);

            IList<FoodTruckResponse.FoodTruck> expectedResponse = new FoodTruckResponse.FoodTruck[4];

            expectedResponse[0] = new FoodTruckResponse.FoodTruck { Name = "Bread &amp; Circuses", Type = "Burgers / Gastropub" };
            expectedResponse[1] = new FoodTruckResponse.FoodTruck { Name = "Chickn Fix", Type = "Southern / Asian" };
            expectedResponse[2] = new FoodTruckResponse.FoodTruck { Name = "Hallava Falafel", Type = "Mediterrannean" };
            expectedResponse[3] = new FoodTruckResponse.FoodTruck { Name = "Where Ya At Matt?", Type = "Southern / Creole" };


            for (var i = 0; i < 4; i++)
            {
                Assert.Equal(expectedResponse[i], result.FoodTrucks[i]);
            }
        }

        [Fact]
        public void HandlesMissingNextWeekDataGracefully()
        {

        }

        private string GetFoodTruckFixtureData()
        {
            var module = this.GetType().GetTypeInfo().Module;
            var fullPath = module.FullyQualifiedName;
            var assemblyDir = fullPath.Substring(0, fullPath.Length - module.Name.Length);
            return File.ReadAllText(Path.Combine(assemblyDir, "food_truck_fixture.html"));
        }

        [Fact]
        public void RequestsFoodTruckWebsite()
        {
            MockHttpMessageHandler httpHandler = new MockHttpMessageHandler();
            var subject = new FoodTruckRepository(null, httpHandler, "https://food.example.com/trucks");
            var result = subject.RequestFoodTruckWebsite();

            Assert.Equal("my content", result);
            Assert.Equal("https://food.example.com/trucks", httpHandler.requestedUri.ToString());
        }

        public class MockHttpMessageHandler : HttpMessageHandler
        {
            public Uri requestedUri;

            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken
            )
            {
                requestedUri = request.RequestUri;
                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("my content")
                };

                return await Task.FromResult(responseMessage);
            }
        }
    }
}