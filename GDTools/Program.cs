using log4net;
using log4net.Appender;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using WatsonWebserver;

namespace GDTools {
    internal class Program {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string Banner = @"

 _._     _,-'""""`-._
(,-.`._,'(       |\`-/|  Geometry Dash Likebot - 3
    `-.-' \ )-`( , o o)    Git: github.com/AlizerUncaged/LikeBot-3
          `-    \`_`""'-
";
        private static Core.Watson_Server.Watson webServer = new();

        static async Task Main(string[] args) {


            Console.WriteLine(Banner);

            System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            Utilities.Logging.Initialize();

            Logger.Info($"=================== Program Started ===================");

            Console.CancelKeyPress += ConsoleExiting;

            await Database.Database.Read();

            await Core.Boomlings_Networking.Proxies.InitializeProxies();



            _ = webServer.StartAsync();

            // autostart autostarts
            var autoStarts = typeof(Armitage.Auto_Start);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => autoStarts.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);

            foreach (var autoStart in types) {
                var sceneInstance = (Armitage.Auto_Start)Activator.CreateInstance(autoStart);
                await sceneInstance.StartAsync();
            }

            while (true) {
                Console.WriteLine($"[A] Free and Check Memory Usage, [B] Parse Accounts, [C] Sessions, [D] Disable/Enable Auto DislikeBot [X] Exit");
                var ckey = Console.ReadKey(true);
                var ckeyChar = char.ToUpper(ckey.KeyChar);
                var ckeyKey = ckey.Key;

                switch (ckeyKey) {
                    case ConsoleKey.D:
                        // pause for a while
                        Armitage.Level_Comment_Auto_Disliker.disabled = true;
                        Console.WriteLine("Set level ID empty for ignore.");
                        int newLevelID = 0;
                        if (int.TryParse(Console.ReadLine(), out newLevelID)) {
                            Armitage.Level_Comment_Auto_Disliker.targetLevelId = newLevelID;
                        } else Console.WriteLine("Same level ID.");
                        Console.WriteLine("To disable? false if keep running, true if disabled");

                        var toDisable = bool.Parse(Console.ReadLine());
                        Armitage.Level_Comment_Auto_Disliker.disabled = toDisable;

                        Console.WriteLine("true if like, false if dislikes");
                        var isLike = bool.Parse(Console.ReadLine());
                        Armitage.Level_Comment_Auto_Disliker.isLike = isLike;
                        Logger.Debug($"Comment AutoDislike IsRunning: {!Armitage.Level_Comment_Auto_Disliker.disabled} Level: {Armitage.Level_Comment_Auto_Disliker.targetLevelId} Islike: {Armitage.Level_Comment_Auto_Disliker.isLike}");
                        break;
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

        private static async void Exit() {
            webServer.Stop();
            await Database.Database.Save();

            Logger.Info($"Exiting...");
            Environment.Exit(Environment.ExitCode);

        }
    }
}
