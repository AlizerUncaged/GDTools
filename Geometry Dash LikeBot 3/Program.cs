using log4net;
using log4net.Config;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Geometry_Dash_LikeBot_3 {
    internal class Program {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string Banner = @"

 _._     _,-'""""`-._
(,-.`._,'(       |\`-/|  Geometry Dash Likebot - 3
    `-.-' \ )-`( , o o)    Git: github.com/AlizerUncaged/LikeBot-3
          `-    \`_`""'-
";

        private static Server s;
        static async Task Main(string[] args) {
            Console.WriteLine(Banner);

            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // When debugging (or building as Debug), GDL-3 will be at 127.0.0.1:8080 for testing reasons.
#if DEBUG
            Constants.IP = "127.0.0.1";
            Constants.Port = 8080;
#endif

            Console.CancelKeyPress += Console_CancelKeyPress;

            await Database.Database.Read();

            await Likebot_3.Boomlings_Networking.Proxies.InitializeProxies();

            s = new Server(Constants.IP, Constants.Port, false, DefaultRoute);
            s.Settings.AccessControl.Mode = AccessControlMode.DefaultPermit;
            s.Events.Logger = LogReceived;
            s.Events.RequestReceived += Events_RequestReceived;
            s.Events.ExceptionEncountered += Events_ExceptionEncountered;

            var serverStartTask = s.StartAsync();
            Logger.Info($"Server started at {Constants.IP}:{Constants.Port}.");


            while (true) {
                Logger.Info($"[A] Free and Check Memory Usage, [B] Ban an IP, [C] Sessions");
                var key = Console.ReadKey(true);
                switch (key.Key) {
                    case ConsoleKey.A:
                        GC.Collect();
                        Logger.Info(
                            $"Memory Usage: {Utilities.Mr_Clean.FormatBytes(Process.GetCurrentProcess().PrivateMemorySize64)} " +
                            $"Collection Count: {GC.CollectionCount(0)}");
                        Logger.Info(
                            $"Accounts: {Database.Database.Accounts.Count} " +
                            $"Threads: {Process.GetCurrentProcess().Threads.Count}");
                        break;
                    case ConsoleKey.C:
                        var accounts = Database.Database.Accounts;
                        Logger.Info($"Accounts (Total: {accounts.Count})");
                        foreach (var account in accounts) {
                            Logger.Info($"{accounts.IndexOf(account)}: {account.Username} Last Login: {account.LastSuccessfulContribution}");
                        }
                        break;
                }
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            Logger.Info($"Stopping server.");
            if (s != null)
                if (s.IsListening) {
                    s.Stop();
                    s.Dispose();
                }

            Logger.Info($"Saving database.");
            Database.Database.Save();

            Logger.Info($"Exiting...");

            Environment.Exit(Environment.ExitCode);
        }

        // maybe write these in logs
        private static void Events_ExceptionEncountered(object sender, ExceptionEventArgs e) {
            Logger.Info($"Error occured: {e.Exception}");
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
