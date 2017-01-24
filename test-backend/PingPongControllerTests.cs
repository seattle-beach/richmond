using LightMock;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System.Collections.Generic;

namespace Richmond.Tests
{
    public class PingPongControllerTests
    {
        [Fact]
        public void ReturnsSomeResponse() 
        {
            var mockContext = new MockContext<IPingPongRepository>();
            var pingPongRepositoryMock = new PingPongRepositoryMock(mockContext);
            List<string> ladder = new List<string>(new string[] { 
                "Gregg Van Hove", 
                "Noah Denton", 
                "Toby Rumans", 
                "Augustus Lidaka", 
                "Alpha Chen", 
                "Steve Gravrock", 
                "Greg Chattin Mcnichols",  
                "Andres Medina", 
                "Sinister Augustus", 
                "Sinister Steve"});
            mockContext.Arrange(f => f.RequestPingPongWebsite()).Returns("seattleLadder");
            mockContext.Arrange(f => f.ParsePingPongWebsite("seattleLadder")).Returns(ladder);

            var subject = new PingPongController(pingPongRepositoryMock);

            var result = subject.PingPongLadder();

            var response = result as OkObjectResult;
            Assert.NotNull(response);

            var payload = response.Value as List<string>;
            Assert.NotNull(payload);
            Assert.Equal(ladder, payload);
        }
    }

    public class PingPongRepositoryMock : IPingPongRepository
    {
        private IInvocationContext<IPingPongRepository> context;

        public PingPongRepositoryMock(MockContext<IPingPongRepository> context)
        {
            this.context = context;
        }

        public string RequestPingPongWebsite()
        {
            return context.Invoke(f => f.RequestPingPongWebsite());
        }

        public List<string> ParsePingPongWebsite(string html)
        {
            return context.Invoke(f => f.ParsePingPongWebsite(html));
        }
    }
}