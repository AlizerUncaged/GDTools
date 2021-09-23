using System;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Geometry_Dash_LikeBot_3
{
    internal class Program
    {
        private static Server s;
        /// <summary>
        /// Main entry point.
        /// </summary>
        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            Database.Data.Read();
            Console.WriteLine($"Account list loaded {Database.Data.Accounts.Count} accounts.");

            s = new Server(Constants.IP, Constants.Port, false, DefaultRoute);
            s.Events.Logger = LogReceived;
            s.Events.RequestReceived += Events_RequestReceived;
            s.Events.ExceptionEncountered += Events_ExceptionEncountered;

            var serverStartTask = s.StartAsync();
            Console.WriteLine($"Server started at {Constants.IP}:{Constants.Port}.");
            await serverStartTask;
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine($"Stopping server.");
            s.Stop();

            Console.WriteLine($"Saving database.");
            Database.Data.Save();

            Console.WriteLine($"Done.");
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

        static async Task DefaultRoute(HttpContext ctx)
        {
            ctx.Response.StatusCode = 302;
            ctx.Response.Headers["Location"] = "https://catgirlcare.org/";
            await ctx.Response.Send("No.");
        }
    }
}
