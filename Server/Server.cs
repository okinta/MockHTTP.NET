using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace Server
{
    public class Server : IDisposable
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Task _task;

        public Server()
        {
            _listener.Prefixes.Add("http://localhost:8000/");
            _listener.Start();
            _task = Task.Run(HandleRequests);
        }

        private async Task HandleRequests()
        {
            try
            {
                while (_listener.IsListening)
                {
                    Console.WriteLine("Listening");
                    var context = await _listener.GetContextAsync();
                    Console.WriteLine("got context");
                    var buffer = Encoding.UTF8.GetBytes("hello");
                    context.Response.ContentLength64 += buffer.Length;
                    await context.Response.OutputStream.WriteAsync(
                        buffer, 0, buffer.Length);
                    Console.WriteLine("sent response");
                    context.Response.OutputStream.Close();
                    context.Response.Close();
                    Console.WriteLine("done");
                }
            }
            catch (HttpListenerException ex)
            {
                // When the listener is stopped, it will throw an exception for being
                // cancelled, so just ignore it
                if (ex.ErrorCode != 995)
                    throw;
                Console.WriteLine("goodbye");
            }
        }

        public void Dispose()
        {
            _listener.Stop();
            try
            {
                _task.GetAwaiter().GetResult();
            }
            catch (ObjectDisposedException)
            {
            }
        }
    }
}
