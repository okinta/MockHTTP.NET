using MockHttp.Net;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace Tests
{
    internal class MyCustomHandler
    {
        private int ResponseNumber { get; set; }

        public string Handler(
            HttpListenerRequest req,
            HttpListenerResponse rsp,
            Dictionary<string, string> prm)
        {
            ResponseNumber += 1;
            return $"Response number: {ResponseNumber}";
        }
    }

    public class MockCustomHandlerTest
    {
        [Fact]
        public void TestCustomHandler()
        {
            var handler = new MyCustomHandler();
            using var requests = new MockRequests(
                new HttpHandler(
                    "/handler", handler.Handler, HttpMethods.Get));
            var client = new RestClient(requests.Url);
            var request = new RestRequest("handler");

            Assert.Equal("Response number: 1", client.Get(request).Content);
            requests.AssertAllCalledOnce();

            Assert.Equal("Response number: 2", client.Get(request).Content);
            Assert.Equal(2, requests[0].Called);

            Assert.Equal("Response number: 3", client.Get(request).Content);
            Assert.Equal(3, requests[0].Called);
        }
    }
}
