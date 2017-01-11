using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Richmond.Tests
{
    public class HelloControllerTests
    {
        [Fact]
        public void ReturnsResponse() 
        {
            var subject = new HelloController();
            IActionResult response = subject.HelloWorld();

            var result = response as OkObjectResult;

            Assert.NotNull(result);

            var payload = result.Value as HelloResponse;
            Assert.Equal("hello, world!", payload.Response);
        }
    }
}
