using MockHttpServer;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace MockHTTP.NET
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
        /// Gets the number of times the MockHttpHandler was called.
        /// </summary>
        public int Called { get; private set; }

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

        private string ExpectedResponse { get; } = "";
        private string ValidatedRequestResponse { get; } = "";

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
        /// Creates a HTTP mock request that returns the given string.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="response">The response to return when a mock request is
        /// received.</param>
        /// <param name="httpMethod">The HTTP method to mock.</param>
        public HttpHandler(string url, string response, HttpMethods httpMethod)
        {
            Url = url;
            HttpMethod = httpMethod.ToString();
            HandlerFunction = (req, rsp, prm) => response;

            MockHttpHandler = new MockHttpHandler(
                url, HttpMethod, HandlerFunctionWithCounter);
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
        /// Creates a HTTP mock request that calls the given function when a request is
        /// received.
        /// </summary>
        /// <param name="url">The URL to mock.</param>
        /// <param name="handlerFunction">The function to call when a mock request is
        /// received.</param>
        /// <param name="httpMethod">The HTTP method to mock.</param>
        public HttpHandler(string url, Handler handlerFunction,
            HttpMethods httpMethod)
        {
            Url = url;
            HttpMethod = httpMethod.ToString();
            HandlerFunction = handlerFunction;

            MockHttpHandler = new MockHttpHandler(
                url, HttpMethod, HandlerFunctionWithCounter);
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
        public HttpHandler(string url, string expectedContent, string response)
        {
            Url = url;
            HttpMethod = HttpMethods.Post.ToString();
            ExpectedResponse = expectedContent;
            ValidatedRequestResponse = response;
            HandlerFunction = ValidateRequest;

            MockHttpHandler = new MockHttpHandler(
                url, HttpMethod, HandlerFunctionWithCounter);
        }

        private string HandlerFunctionWithCounter(
            HttpListenerRequest req,
            HttpListenerResponse rsp,
            Dictionary<string, string> prm)
        {
            Called += 1;
            return HandlerFunction(req, rsp, prm);
        }

        private string ValidateRequest(
            HttpListenerRequest req,
            HttpListenerResponse rsp,
            Dictionary<string, string> prm)
        {
            Assert.Equal(ExpectedResponse, req.GetContent());
            return ValidatedRequestResponse;
        }
    }
}
