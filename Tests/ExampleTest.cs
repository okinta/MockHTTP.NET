using RestSharp;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using Xunit;

namespace Tests
{
    public class ExampleTest : IDisposable
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Task _task;

        public ExampleTest()
        {
            _listener.Prefixes.Add("http://localhost:8000/");
            _listener.Start();
            _task = HandleRequests();
        }

        private async Task HandleRequests()
        {
            var context = await _listener.GetContextAsync();
            var buffer = Encoding.UTF8.GetBytes("hello");
            context.Response.ContentLength64 += buffer.Length;
            await context.Response.OutputStream.WriteAsync(
                buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
            context.Response.Close();
        }

        public void Dispose()
        {
            _listener.Stop();
            _task.Dispose();
        }

        [Fact]
        public void Test()
        {
            var client = new RestClient("http://localhost:8000/");
            var request = new RestRequest("/");

            Assert.Equal("hello", client.Get(request).Content);
        }
    }
}
