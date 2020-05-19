using MockHttp.Net.Exceptions;
using MockHttp.Net;
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

        [Fact]
        public void TestRetryWhenPortInUse()
        {
            var handler = new HttpHandler("/");
            var random = new PredictableNumberGenerator(1, 1, 1, 2, 3, 3, 4);

            using var requests1 = new MockRequests(random.Next, handler);
            Assert.Equal("http://localhost:7101/", requests1.Url);
            var client = new RestClient(requests1.Url);
            var request = new RestRequest("");
            Assert.Equal("", client.Get(request).Content);

            using var requests2 = new MockRequests(random.Next, handler);
            Assert.Equal("http://localhost:7102/", requests2.Url);
            client = new RestClient(requests2.Url);
            Assert.Equal("", client.Get(request).Content);

            using var requests3 = new MockRequests(random.Next, handler);
            Assert.Equal("http://localhost:7103/", requests3.Url);
            client = new RestClient(requests3.Url);
            Assert.Equal("", client.Get(request).Content);

            using var requests4 = new MockRequests(random.Next, handler);
            Assert.Equal("http://localhost:7104/", requests4.Url);
            client = new RestClient(requests4.Url);
            Assert.Equal("", client.Get(request).Content);
        }
    }
}
