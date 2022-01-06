using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            Logger.Info($"=================== Program Started ===================");
            // When debugging (or building as Debug), GDL-3 will be at 127.0.0.1:8080 for testing reasons.
#if DEBUG
            Constants.IP = "127.0.0.1";
            Constants.Port = 8080;
#endif

            Console.CancelKeyPress += ConsoleExiting;

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
                Console.WriteLine($"[A] Free and Check Memory Usage, [B] Ban an IP, [C] Sessions, [X] Exit");
                var ckey = Console.ReadKey(true).KeyChar;
                ckey = char.ToUpper(ckey);

                var sceneClass = typeof(Scenes.Scene);
                foreach (Type type in Assembly.GetAssembly(sceneClass).GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(sceneClass))) {
                    var sceneInstance = (Scenes.Scene)Activator.CreateInstance(type);
                    if (sceneInstance.Key == ckey) {
                        // pause loggers
                        List<log4net.Core.Level> originalLoggingLevels = new();
                        var activeLoggers = logRepository.GetAppenders().Where(x => x is log4net.Appender.ConsoleAppender);
                        foreach (var logger in activeLoggers) {
                            var appenderSkeleton = logger as log4net.Appender.AppenderSkeleton;
                            originalLoggingLevels.Add(appenderSkeleton.Threshold);
                            appenderSkeleton.Threshold = log4net.Core.Level.Off;
                        }

                        await sceneInstance.StartAsync();

                        int i = 0;
                        foreach (var logger in activeLoggers) {
                            var appenderSkeleton = logger as log4net.Appender.AppenderSkeleton;
                            appenderSkeleton.Threshold = originalLoggingLevels[i];
                            i++;
                        }

                        Console.Clear();
                    }
                }

                //switch (key.Key) {
                //    case ConsoleKey.C:
                //        var accounts = Database.Database.Accounts;
                //        Logger.Info($"Accounts (Total: {accounts.Count})");
                //        foreach (var account in accounts) {
                //            Logger.Info($"{accounts.IndexOf(account)}: {account.Username} Created: {account.LoginDate}");
                //        }
                //        break;
                //    case ConsoleKey.X:
                //        ConsoleExiting(null, null);
                //        break;
                // }
            }

        }

        private static void ConsoleExiting(object sender, ConsoleCancelEventArgs e) {
            Logger.Info($"Stopping server.");
            if (s != null)
                if (s.IsListening) {
                    s.Stop();
                    s.Dispose();
                }

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
            Logger.Debug($"{message}");

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
