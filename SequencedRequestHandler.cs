using System.Collections.Generic;
using System.Net;
using System;
using Xunit.Sdk;

namespace MockHttp.Net
{
    /// <summary>
    /// Creates a mock request handler that iterates through a collection of handlers in
    /// sequence.
    /// </summary>
    internal class SequencedRequestHandler
    {
        /// <summary>
        /// Gets the mock request handler to use for responding to requests.
        /// </summary>
        public HttpHandler.Handler Handler { get; }

        private int HandlerIndex { get; set; }
        private IList<ValidateRequestHandler> Handlers { get; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="handlers">The collection of handlers to iterate through.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="handlers"/> is
        /// null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="handlers"/> are
        /// empty.</exception>
        public SequencedRequestHandler(params ValidateRequestHandler[] handlers)
        {
            if (handlers is null)
                throw new ArgumentNullException(
                    nameof(handlers), "handlers must be provided");

            if (handlers.Length == 0)
                throw new ArgumentException(
                    "at least one handler must be provided", nameof(handlers));

            Handlers = new List<ValidateRequestHandler>(handlers);
            Handler = OnReceiveRequest;
        }

        /// <summary>
        /// Called when a mock request is received. Calls the next handler in the sequence.
        /// </summary>
        /// <param name="req">The HttpListenerRequest that represents the mock
        /// request.</param>
        /// <param name="rsp">The HttpListenerResponse instance that will be sent back
        /// for the mock request.</param>
        /// <param name="prm">The list of parameters received in the URL.</param>
        /// <returns>The mocked response.</returns>
        /// <exception cref="EqualException">If the request does not have the expected
        /// parameters.</exception>
        /// <exception cref="InvalidOperationException">If there are no more handlers
        /// available for the request.</exception>
        private string OnReceiveRequest(
            HttpListenerRequest req,
            HttpListenerResponse rsp,
            Dictionary<string, string> prm)
        {
            var index = HandlerIndex;
            HandlerIndex += 1;

            ValidateRequestHandler handler;
            try
            {
                handler = Handlers[index];
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new InvalidOperationException(
                    "No more handlers are available for the request", e);
            }

            return handler.Handler(req, rsp, prm);
        }
    }
}
