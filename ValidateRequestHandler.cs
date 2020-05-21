using System.Collections.Generic;
using System.Net;
using Xunit.Sdk;
using Xunit;

namespace MockHttp.Net
{
    /// <summary>
    /// Creates a mock request handler that validates parameters.
    /// </summary>
    internal class ValidateRequestHandler
    {
        /// <summary>
        /// Gets the mock request handler to use for responding to requests.
        /// </summary>
        public HttpHandler.Handler Handler { get; }

        private string ExpectedContent { get; }
        private string ValidatedResponse { get; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="expectedContent">The expected content to be received from the
        /// request.</param>
        /// <param name="response">The response to return after the request as been
        /// validated.</param>
        public ValidateRequestHandler(string expectedContent, string response)
        {
            ExpectedContent = expectedContent;
            ValidatedResponse = response;

            Handler = ValidateRequest;
        }

        /// <summary>
        /// Called when a mock request is received. Validates that the request has the
        /// expected parameters.
        /// </summary>
        /// <param name="req">The HttpListenerRequest that represents the mock
        /// request.</param>
        /// <param name="rsp">The HttpListenerResponse instance that will be sent back
        /// for the mock request.</param>
        /// <param name="prm">The list of parameters received in the URL.</param>
        /// <returns>The mocked response.</returns>
        /// <exception cref="EqualException">If the request does not have the expected
        /// parameters.</exception>
        private string ValidateRequest(
            HttpListenerRequest req,
            HttpListenerResponse rsp,
            Dictionary<string, string> prm)
        {
            Assert.Equal(ExpectedContent, req.GetContent());
            return ValidatedResponse;
        }
    }
}
