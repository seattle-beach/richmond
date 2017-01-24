using System.Collections.Generic;
using Xunit;

namespace Richmond.Tests
{
    public class PingPongRepositoryTests
    {
        private RepositoryTestHelper repositoryTestHelper;
        
        public PingPongRepositoryTests()
        {
            repositoryTestHelper = new RepositoryTestHelper();
        }

        [Fact]
        public void ParsesPingPongLadder()
        {
            var subject = new PingPongRepository();

            var html = repositoryTestHelper.GetFileFixtureData("ping_pong_fixture.html");
            var result = subject.ParsePingPongWebsite(html);

            List<string> expectedLadder = new List<string>(new string[] { 
                "Gregg Van Hove", 
                "Noah Denton", 
                "Toby Rumans", 
                "Augustus Lidaka", 
                "Alpha Chen", 
                "Steve Gravrock", 
                "Greg Chattin Mcnichols",  
                "Andres Medina", 
                "Sinister Augustus", 
                "Sinister Steve",
                "Sinister Elenore", 
                "Elenore Bastian"});

            Assert.Equal(expectedLadder, result);
        }
    }
}