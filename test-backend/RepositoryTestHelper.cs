using System;
using System.IO;
using System.Reflection;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Richmond.Tests
{
    public class RepositoryTestHelper
    {
        public string GetFileFixtureData(string filename)
        {
            var module = this.GetType().GetTypeInfo().Module;
            var fullPath = module.FullyQualifiedName;
            var assemblyDir = fullPath.Substring(0, fullPath.Length - module.Name.Length);
            return File.ReadAllText(Path.Combine(assemblyDir, filename));
        }

        public MockHttpMessageHandler GetMockHttpMessageHandler(string responseContent)
        {
            return new MockHttpMessageHandler(responseContent);
        }

        public class MockHttpMessageHandler : HttpMessageHandler
        {
            public Uri requestedUri;
            private string responseContent;

            public MockHttpMessageHandler(string responseContent)
            {
                this.responseContent = responseContent;
            }

            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken
            )
            {
                requestedUri = request.RequestUri;
                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(responseContent)
                };

                return await Task.FromResult(responseMessage);
            }
        }
    }
}