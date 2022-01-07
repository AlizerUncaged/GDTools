using log4net;
using log4net.Appender;
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
        private static Likebot_3.Watson_Server.Watson webServer = new();
        static async Task Main(string[] args) {
            Console.WriteLine(Banner);
            Utilities.Logging.Initialize();

            Logger.Info($"=================== Program Started ===================");

            Console.CancelKeyPress += ConsoleExiting;

            await Database.Database.Read();

            await Likebot_3.Boomlings_Networking.Proxies.InitializeProxies();

            _ = webServer.StartAsync();

            while (true) {
                Console.WriteLine($"[A] Free and Check Memory Usage, [B] Ban an IP, [C] Sessions, [X] Exit");
                var ckey = Console.ReadKey(true);
                var ckeyChar = char.ToUpper(ckey.KeyChar);
                var ckeyKey = ckey.Key;

                switch (ckeyKey) {
                    case ConsoleKey.X:
                        Exit();
                        break;
                    default:
                        var sceneClass = typeof(Scenes.Scene);
                        foreach (Type type in Assembly.GetAssembly(sceneClass).GetTypes().Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(sceneClass))) {
                            var sceneInstance = (Scenes.Scene)Activator.CreateInstance(type);
                            if (sceneInstance.Key == ckeyChar) {
                                var consolePauser = Utilities.Logging.PauseConsoleAppenders;
                                await sceneInstance.StartAsync();
                                consolePauser.ResumeInterfaceOfType<ConsoleAppender>();
                                Console.Clear();
                            }
                        }
                        break;
                }
            }

        }

        private static void ConsoleExiting(object sender, ConsoleCancelEventArgs e) {
            Exit();
        }

        private static void Exit() {
            webServer.Stop();
            Database.Database.Save();

            Logger.Info($"Exiting...");
            Environment.Exit(Environment.ExitCode);

        }
    }
}
