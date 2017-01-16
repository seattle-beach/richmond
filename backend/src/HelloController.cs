using Microsoft.AspNetCore.Mvc;

namespace Richmond
{
    public class HelloController
    {
        [HttpGet("/")]
        public IActionResult HelloWorld()
        {
            var response = new HelloResponse { Response = "hello, world!" };
            return new OkObjectResult(response);
        }
    }
}
