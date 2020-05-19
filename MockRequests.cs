using MockHttp.Net.Exceptions;
using MockHttpServer;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System;

[assembly: InternalsVisibleTo("Tests")]

namespace MockHttp.Net
{
    /// <summary>
    /// A function to use to generate a random number between the two given values.
    /// </summary>
    /// <param name="minValue">The inclusive lower bound of the random number
    /// returned.</param>
    /// <param name="maxValue">The exclusive upper bound of the random number
    /// returned.</param>
    /// <returns>A random number between the two given values.</returns>
    internal delegate int RandomNumber(int minValue, int maxValue);

    /// <summary>
    /// Describes methods to help mock HTTP requests.
    /// </summary>
    public class MockRequests : IDisposable
    {
        /// <summary>
        /// Gets the mocked URL that requests should be sent to.
        /// </summary>
        public string Url { get; }

        private const int TestPortRangeEnd = 8200;
        private const int TestPortRangeStart = 8100;
        private HttpHandler[] Handlers { get; }
        private MockServer MockServer { get; }
        private static readonly int[] PortInUseErrorCodes = { 183, 400 };

        /// <summary>
        /// Instantiates a new instance. Starts the mock HTTP server.
        /// </summary>
        /// <param name="handlers">The list of handlers to use for serving the mock HTTP
        /// requests.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="handlers"/> is
        /// null.</exception>
        public MockRequests(params HttpHandler[] handlers) :
            this(new Random().Next, handlers)
        {
        }

        /// <summary>
        /// Instantiates a new instance with a custom random number generator. Starts
        /// the mock HTTP server.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <param name="handlers">The list of handlers to use for serving the mock HTTP
        /// requests.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="handlers"/> is
        /// null.</exception>
        internal MockRequests(RandomNumber random, params HttpHandler[] handlers)
        {
            Handlers = handlers ?? throw new ArgumentNullException(
                nameof(handlers), @"handlers must not be null");

            var mockHandlers = handlers.GetMockHttpHandlers().ToArray();
            int port;
            do
            {
                port = random(TestPortRangeStart, TestPortRangeEnd);
                try
                {
                    MockServer = new MockServer(port, mockHandlers);
                }
                catch (HttpListenerException e)
                {
                    Console.WriteLine("Got error {0}", e.ErrorCode);
                    if (!PortInUseErrorCodes.Contains(e.ErrorCode)) throw;
                }
            } while (MockServer is null);

            Url = $"http://localhost:{port}/";
        }

        /// <summary>
        /// Retrieves a CustomMockHttpHandler instance passed into the constructor.
        /// </summary>
        /// <param name="index">The index to retrieve.</param>
        /// <returns>The CustomMockHttpHandler instance at the specified index.</returns>
        public HttpHandler this[int index] => Handlers[index];

        /// <summary>
        /// Stops the mock HTTP server.
        /// </summary>
        public void Dispose()
        {
            MockServer.Dispose();
        }

        /// <summary>
        /// Asserts that all requests have been called exactly once.
        /// </summary>
        /// <exception cref="RequestNotCalledException">If a request was not
        /// called.</exception>
        /// <exception cref="RequestCalledTooOftenException">If a request was called
        /// more than once.</exception>
        public void AssertAllCalledOnce()
        {
            foreach (var handler in Handlers)
            {
                if (handler.Called == 0)
                    throw new RequestNotCalledException(
                        handler.Url, $"{handler.Url} was not called");

                if (handler.Called > 1)
                    throw new RequestCalledTooOftenException(
                        handler.Url, 1, handler.Called,
                        $"{handler.Url} was only expected to be called once. " +
                        $"Instead, was called {handler.Called} times");
            }
        }
    }
}
