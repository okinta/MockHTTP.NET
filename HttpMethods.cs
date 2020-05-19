namespace MockHttp.Net
{
    /// <summary>
    /// The types of HTTP methods available to mock
    /// </summary>
    public enum HttpMethods
    {
        Get,
        Post,
        Put
    }

    /// <summary>
    /// Provides extensions for the HttpMethods enum.
    /// </summary>
    public static class HttpMethodsExtensions
    {
        /// <summary>
        /// Gets the name of the HTTP method the value represents.
        /// </summary>
        /// <param name="methods">The HttpMethods value to get the method for.</param>
        /// <returns>The HTTP method.</returns>
        public static string GetMethod(this HttpMethods methods)
        {
            return methods.ToString().ToUpper();
        }
    }
}
