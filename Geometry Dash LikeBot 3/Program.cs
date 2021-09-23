using System;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Geometry_Dash_LikeBot_3
{
    internal class Program
    {
        /// <summary>
        /// Main entry point.
        /// </summary>
        static async Task Main(string[] args)
        {
            Server s = new Server("127.0.0.1", 9000, false);
            s.Events.Logger = LogReceived;
            s.Events.RequestReceived += Events_RequestReceived;
            s.Events.ExceptionEncountered += Events_ExceptionEncountered;
            await s.StartAsync();
        }

        private static void Events_ExceptionEncountered(object sender, ExceptionEventArgs e)
        {
            Console.WriteLine($"Error occured: {e.Exception}");
        }

        private static void Events_RequestReceived(object sender, RequestEventArgs e)
        {
            Console.WriteLine($"Request from {e.Ip}:{e.Port} to {e.Url}");
        }

        private static void LogReceived(string message)
        {
            Console.WriteLine(message);
        }

    }
}
