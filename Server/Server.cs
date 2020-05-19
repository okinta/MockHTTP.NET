using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Server
{
    internal class Server : IDisposable
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Task _task;

        public Server()
        {
            _listener.Prefixes.Add("http://localhost:8000/");
            _listener.Start();
            _task = HandleRequests();
        }

        private async Task HandleRequests()
        {
            while (_listener.IsListening)
            {
                var context = await _listener.GetContextAsync();
                var buffer = Encoding.UTF8.GetBytes("hello");
                context.Response.ContentLength64 += buffer.Length;
                await context.Response.OutputStream.WriteAsync(
                    buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
                context.Response.Close();
            }
        }

        public void Dispose()
        {
            _listener.Stop();
            _task.Dispose();
        }
    }
}
