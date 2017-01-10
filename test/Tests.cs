using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void ReturnsResponse() 
        {
            var richmond = new Richmond();
            IActionResult response = richmond.HelloWorld();

            var result = response as OkObjectResult;

            Assert.NotNull(result);

            var payload = result.Value as HelloResponse;
            Assert.Equal("hello, world!", payload.Response);
        }
    }
}
