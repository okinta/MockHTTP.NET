using MockHTTP.NET;
using MockHTTP.NET.Exceptions;
using RestSharp;
using Xunit;

namespace Tests
{
    public class MockRequestsTest
    {
        [Fact]
        public void TestAssertAllCalledOnce()
        {
            using var requests = new MockRequests(
                new HttpHandler(
                    "/custom/endpoint", "sample response"));
            var client = new RestClient(requests.Url);
            var request = new RestRequest("custom/endpoint");
            Assert.Equal("sample response", client.Get(request).Content);
            requests.AssertAllCalledOnce();
        }

        [Fact]
        public void TestCalledTwice()
        {
            using var requests = new MockRequests(
                new HttpHandler(
                    "/custom/endpoint", "sample response"));
            var client = new RestClient(requests.Url);
            var request = new RestRequest("custom/endpoint");
            Assert.Equal("sample response", client.Get(request).Content);
            Assert.Equal("sample response", client.Get(request).Content);

            Assert.Throws<RequestCalledTooOftenException>(() =>
                requests.AssertAllCalledOnce());
            Assert.Equal(2, requests[0].Called);
        }

        [Fact]
        public void TestEmptyResponse()
        {
            using var requests = new MockRequests(new HttpHandler("/"));
            var client = new RestClient(requests.Url);
            var request = new RestRequest("");
            Assert.Equal("", client.Get(request).Content);
            requests.AssertAllCalledOnce();
        }
    }
}
