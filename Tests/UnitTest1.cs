using RestSharp;
using Xunit;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            using var _ = new Server.Server();
            var client = new RestClient("http://localhost:8000/");
            var request = new RestRequest("");
            Assert.Equal("hello", client.Get(request).Content);
        }
    }
}
