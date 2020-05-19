using System;

namespace MockHttp.Net.Exceptions
{
    /// <summary>
    /// Thrown when a request was not called.
    /// </summary>
    public class RequestNotCalledException : SystemException
    {
        /// <summary>
        /// Returns the URL that was not called.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Instantiates a new instance.
        /// </summary>
        /// <param name="url">The URL that was not called.</param>
        /// <param name="message">The message describing the error.</param>
        public RequestNotCalledException(string url, string message) : base(message)
        {
            Url = url;
        }
    }
}
