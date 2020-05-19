using System.Threading;
using System;

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
                    Thread.Sleep(10000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
