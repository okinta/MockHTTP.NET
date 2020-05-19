using System.Threading;
using System;
using RestSharp;

namespace Server
{
    internal class Program
    {
        public static void Main()
        {
            using var server = new Server();

            try
            {
                while (true)
                {
                    Thread.Sleep(5000);
                    var client = new RestClient("http://localhost:8000/");
                    var request = new RestRequest("");
                    Console.WriteLine(client.Get(request).Content);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
