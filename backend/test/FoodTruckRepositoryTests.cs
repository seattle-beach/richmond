using System.Collections.Generic;
using Xunit;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;

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

            expectedResponse[0] = new FoodTruckResponse.FoodTruck { Name = "Beez Kneez Gourmet Sausages", Type = "Hot Dogs" };
            expectedResponse[1] = new FoodTruckResponse.FoodTruck { Name = "Seattle Chicken Over Rice", Type = "Mediterranean" };
            expectedResponse[2] = new FoodTruckResponse.FoodTruck { Name = "Bomba Fusion", Type = "Asian" };
            expectedResponse[3] = new FoodTruckResponse.FoodTruck { Name = "Neema's Comfort Food", Type = "Southern" };


            for (var i = 0; i < 4; i++)
            {
                Assert.Equal(expectedResponse[i], result.FoodTrucks[i]);
            }
        }


        public static class FoodTruckWebsiteFixture
        {
            public static IEnumerable<object[]> HtmlData
            {
                get
                {
                    var module = typeof(FoodTruckWebsiteFixture).GetTypeInfo().Module;
                    var fullPath = module.FullyQualifiedName;
                    var assemblyDir = fullPath.Substring(0, fullPath.Length - module.Name.Length);
                    return new List<object[]>
                    {
                        new object[] {File.ReadAllText(
                            Path.Combine(assemblyDir, "food_truck_fixture.html"))}
                    };
                }
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