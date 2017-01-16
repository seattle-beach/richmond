using System.Collections.Generic;
using Xunit;
using System;
using System.IO;
using LightMock;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Richmond.Tests
{
    public class FoodTruckRepositoryTests
    {

        [Theory]
        [MemberData("HtmlData", MemberType = typeof(FoodTruckWebsiteFixture))]
        public void ParsesFoodTruckSite(string html)
        {

            var subject = new FoodTruckRepository();

            var result = subject.ParseFoodTruckSite(html);

            IList<FoodTruckResponse.FoodTruck> expectedResponse = new FoodTruckResponse.FoodTruck[4];

            expectedResponse[0] = new FoodTruckResponse.FoodTruck { Name = "Delfino's Chicago Pizza", Type = "Pizza" };
            expectedResponse[1] = new FoodTruckResponse.FoodTruck { Name = "El Cabrito Oaxaca", Type = "Mexican" };
            expectedResponse[2] = new FoodTruckResponse.FoodTruck { Name = "NOSH", Type = "English" };
            expectedResponse[3] = new FoodTruckResponse.FoodTruck { Name = "Seattle Chicken Over Rice", Type = "Mediterranean" };


            for (var i = 0; i < 4; i++)
            {
                Assert.Equal(expectedResponse[i], result[i]);
            }
        }


        public static class FoodTruckWebsiteFixture
        {
            private static readonly List<object[]> _data
                = new List<object[]>
                {
                    new object[] {File.ReadAllText(Path.Combine(Environment.GetEnvironmentVariable("RICHMOND_BASE_DIRECTORY"),
                "backend/test/food_truck_fixture.html"))}
                };

            public static IEnumerable<object[]> HtmlData
            {
                get { return _data; }
            }
        }

        [Fact]
        public void RequestsFoodTruckWebsite()
        {
            var subject = new FoodTruckRepository(new MockHttpMessageHandler());
            var result = subject.RequestFoodTruckWebsite();

            Assert.Equal("my content", result);

        }

        public class MockHttpMessageHandler : HttpMessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken
            )
            {
                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("my content")
                };

                return await Task.FromResult(responseMessage);

            }
        }
    }
}