using System;

namespace MockHttp.Net.Exceptions
{
    /// <summary>
    /// Thrown when a request was called too many times.
    /// </summary>
    public class RequestCalledTooOftenException : SystemException
    {
        /// <summary>
        /// Returns the URL that was not called.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Returns the number of calls that was expected.
        /// </summary>
        public int ExpectedCalls { get; }

        /// <summary>
        /// Returns the number of calls that were made.
        /// </summary>
        public int ActualCalls { get; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="url">The URL that was called.</param>
        /// <param name="expectedCalls">The number of calls expected.</param>
        /// <param name="actualCalls">The number of calls that were made.</param>
        /// <param name="message">The message describing the error.</param>
        public RequestCalledTooOftenException(
            string url, int expectedCalls, int actualCalls, string message) :
            base(message)
        {
            Url = url;
            ExpectedCalls = expectedCalls;
            ActualCalls = actualCalls;
        }
    }
}
