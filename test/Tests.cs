using System;
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
        	var response = richmond.HelloWorld();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            // body is like JSON: {"response":"hello, world!"}
        }
    }
}
