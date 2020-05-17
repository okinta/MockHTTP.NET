using MockHttpServer;
using System.Collections.Generic;
using System.Linq;

namespace MockHTTP.NET
{
    /// <summary>
    /// Provides an extension method to convert CustomMockHttpHandler instances
    /// to MockHttpHandler for use by MockServer.
    /// </summary>
    internal static class HttpHandlerExtensions
    {
        /// <summary>
        /// Converts to a collection of MockHttpHandler instances.
        /// </summary>
        /// <param name="handlers">The handlers to convert.</param>
        /// <returns>The converted instances.</returns>
        public static IEnumerable<MockHttpHandler> GetMockHttpHandlers(
            this IEnumerable<HttpHandler> handlers)
        {
            return handlers.Select(handler => handler.MockHttpHandler).ToList();
        }
    }
}
