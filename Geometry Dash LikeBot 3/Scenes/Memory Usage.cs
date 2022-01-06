using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Scenes {
    public class Memory_Usage : Scene {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Memory_Usage() : base('A', "Memory Usage") {

        }
        public override async Task StartAsync() {

            GC.Collect();
            Console.Clear();

            while (true) {
                await Task.Delay(100);

                Console.SetCursorPosition(0, 0);

                if (Console.KeyAvailable) {
                    var keyHit = Console.ReadKey(true);
                    var keyHitkey = keyHit.Key;
                    if (keyHitkey == ConsoleKey.Escape) {
                        return;
                    }
                }


                Console.WriteLine(
                    $"Press ESC to exit.");

                Console.WriteLine(
                    $"Memory Usage: {Utilities.Mr_Clean.FormatBytes(Process.GetCurrentProcess().PrivateMemorySize64)} " +
                    $"Collection Count: {GC.CollectionCount(0)} ");
                Console.WriteLine(
                    $"Accounts: {Database.Database.Accounts.Count} " +
                    $"Threads: {Process.GetCurrentProcess().Threads.Count}");
            }
        }
    }
}
