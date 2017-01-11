using Microsoft.AspNetCore.Mvc;

namespace Richmond
{
    public class HelloController
    {
        [HttpGet("world")]
        public IActionResult HelloWorld()
        {
            System.Console.WriteLine("Hello World!");
            return new OkResult();
        }
    }
}
