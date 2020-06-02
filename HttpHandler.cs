using MockHttpServer;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System;

namespace MockHttp.Net
{
    /// <summary>
    /// Allows counting of calls to a MockHttpHandler.
    /// </summary>
    public class HttpHandler
    {
        /// <summary>
        /// Called when a mock request is received.
        /// </summary>
        /// <param name="req">The HttpListenerRequest that represents the mock
        /// request.</param>
        /// <param name="rsp">The HttpListenerResponse instance that will be sent back
        /// for the mock request.</param>
        /// <param name="prm">The list of parameters received in the URL.</param>
        /// <returns>The response content to send back for the mock request.</returns>
        public delegate string Handler(
            HttpListenerRequest req,
            HttpListenerResponse rsp,
            Dictionary<string, string> prm);

        /// <summary>
        /// Triggered when an exception is thrown within a handler.
        /// </summary>
        public event Action<Exception> OnError;

        /// <summary>
        /// Gets the number of times the handler was called.
        /// </summary>
        public int Called { get; private set; }

        /// <summary>
        /// Gets the number of handlers this instance manages.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the URL of the MockHttpHandler.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Gets the HTTP method of the MockHttpHandler.
        /// </summary>
        public string HttpMethod { get; }

        /// <summary>
        /// Gets the MockHttpHandler instance.
        /// </summary>
        public MockHttpHandler MockHttpHandler { get; }

        /// <summary>
        /// Gets the MockHttpHandler handler function.
        /// </summary>
        public Handler HandlerFunction { get; }

        /// <summary>
        /// Creates a HTTP GET mock request that returns an empty response.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        public HttpHandler(string url) : this(url, "") { }

        /// <summary>
        /// Creates a HTTP GET mock request that returns the given string.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="response">The response to return when a mock request is
        /// received.</param>
        public HttpHandler(string url, string response) :
            this(url, response, HttpMethods.Get)
        {
        }

        /// <summary>
        /// Creates a HTTP mock request that returns an ordered sequence of strings.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="responses">The ordered sequence of responses to return when a
        /// mock request is received.</param>
        public HttpHandler(string url, params string[] responses) :
            this(url, CreateHandler(responses), HttpMethods.Get, responses.Length)
        {
        }

        /// <summary>
        /// Creates a HTTP mock request that returns the given string.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="response">The response to return when a mock request is
        /// received.</param>
        /// <param name="httpMethod">The HTTP method to mock.</param>
        public HttpHandler(string url, string response, HttpMethods httpMethod) :
            this(url, CreateHandler(response), httpMethod)
        {
        }

        /// <summary>
        /// Creates a HTTP POST mock request that calls the given function when a request
        /// is received.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="handlerFunction">The function to call when a mock request is
        /// received.</param>
        public HttpHandler(string url, Handler handlerFunction) :
            this(url, handlerFunction, HttpMethods.Post)
        {
        }

        /// <summary>
        /// Creates a HTTP POST mock request that validates the given parameters were
        /// received from the request and returns the given response.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="expectedContent">The content expected to be received by from
        /// request.</param>
        /// <param name="response">The response to return when a mock request is
        /// received.</param>
        public HttpHandler(string url, string expectedContent, string response) :
            this(url, new ValidateRequestHandler(expectedContent, response).Handler)
        {
        }

        /// <summary>
        /// Creates a HTTP POST mock request that validates the given parameters were
        /// received in order from the request and returns in order the given responses.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="handlers">The list of handlers to respond to the
        /// requests.</param>
        public HttpHandler(string url, params ValidateRequestHandler[] handlers) :
            this(url, new SequencedRequestHandler(handlers).Handler,
                HttpMethods.Post, handlers.Length)
        {
        }

        /// <summary>
        /// Creates a HTTP mock request that calls the given function when a request is
        /// received.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="handlerFunction">The function to call when a mock request is
        /// received.</param>
        /// <param name="httpMethod">The HTTP method to mock.</param>
        public HttpHandler(string url, Handler handlerFunction, HttpMethods httpMethod) :
            this(url, handlerFunction, httpMethod, 1)
        {
        }

        private HttpHandler(
            string url, Handler handlerFunction, HttpMethods httpMethod, int count)
        {
            Count = count;
            HandlerFunction = handlerFunction;
            HttpMethod = httpMethod.GetMethod();
            Url = url;

            MockHttpHandler = new MockHttpHandler(
                url, HttpMethod, HandlerFunctionWithCounter);
        }

        private string HandlerFunctionWithCounter(
            HttpListenerRequest req,
            HttpListenerResponse rsp,
            Dictionary<string, string> prm)
        {
            Called += 1;

            try
            {
                return HandlerFunction(req, rsp, prm);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e);
                throw;
            }
        }

        private static Handler CreateHandler(string response)
        {
            return (req, rsp, prm) => response;
        }

        private static Handler CreateHandler(params string[] responses)
        {
            return new SequencedRequestHandler(responses.Select(
                response => (Handler) (
                    (req, rsp, prm)
                        => response)).ToArray()).Handler;
        }
    }
}
