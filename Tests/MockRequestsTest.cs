using MockHTTP.NET;
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
    }
}
