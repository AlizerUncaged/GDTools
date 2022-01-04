using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Geometry_Dash_LikeBot_3 {
    internal class Program {
        private static Server s;
        static async Task Main(string[] args) {
            // When debugging (or building as Debug), GDL-3 will be at 127.0.0.1:8080 for testing reasons.
#if DEBUG
            Constants.IP = "127.0.0.1";
            Constants.Port = 8080;
#endif

            Console.CancelKeyPress += Console_CancelKeyPress;

            Database.Data.Read();
            Console.WriteLine($"Account list loaded {Database.Data.Accounts.Count} accounts.\r\nLoading proxies...");

            if (await Likebot_3.Boomlings_Networking.Proxies.InitializeProxies())
                Console.WriteLine($"Loaded {Likebot_3.Boomlings_Networking.Proxies.ProxyList.Count} proxies.");

            s = new Server(Constants.IP, Constants.Port, false, DefaultRoute);
            s.Settings.AccessControl.Mode = AccessControlMode.DefaultPermit;
            s.Events.Logger = LogReceived;
            s.Events.RequestReceived += Events_RequestReceived;
            s.Events.ExceptionEncountered += Events_ExceptionEncountered;

            var serverStartTask = s.StartAsync();
            Console.WriteLine($"Server started at {Constants.IP}:{Constants.Port}.");


            while (true) {
                Console.WriteLine($"[A] Free and Check Memory Usage, [B] Ban an IP, [C] Sessions");
                var key = Console.ReadKey(true);
                switch (key.Key) {
                    case ConsoleKey.A:
                        GC.Collect();
                        Console.WriteLine(
                            $"Memory Usage: {Utilities.Mr_Clean.FormatBytes(Process.GetCurrentProcess().PrivateMemorySize64)} " +
                            $"Collection Count: {GC.CollectionCount(0)}");
                        Console.WriteLine(
                            $"Accounts: {Database.Data.Accounts.Count} " +
                            $"Threads: {Process.GetCurrentProcess().Threads.Count}");
                        break;
                    case ConsoleKey.C:
                        var accounts = Database.Data.Accounts;
                        foreach (var account in accounts) {
                            Console.WriteLine($"{accounts.IndexOf(account)}: {account.Username}'s Session: {account.SessionKey}");
                        }
                        break;
                }
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            Console.WriteLine($"Stopping server.");
            if (s != null)
                if (s.IsListening) {
                    s.Stop();
                    s.Dispose();
                }

            Console.WriteLine($"Saving database.");
            Database.Data.Save();

            Console.WriteLine($"Exiting...");

            Environment.Exit(Environment.ExitCode);
        }

        // maybe write these in logs
        private static void Events_ExceptionEncountered(object sender, ExceptionEventArgs e) {
            Console.WriteLine($"Error occured: {e.Exception}");
        }

        private static void Events_RequestReceived(object sender, RequestEventArgs e) {

        }

        private static void LogReceived(string message) {

        }

        // 404
        static async Task DefaultRoute(HttpContext ctx) {
            ctx.Response.StatusCode = 302;
            ctx.Response.Headers["Location"] = "https://catgirlcare.org/";
            await ctx.Response.Send();
            return;
        }
    }
}
