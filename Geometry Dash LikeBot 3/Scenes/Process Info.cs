using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Scenes {
    public class Process_Info : Scene {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Process_Info() : base('A', "Process Info") {

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
                var currentProc = Process.GetCurrentProcess();
                List<PropertyInfo> invalidInfos = new();
                // display all properties of current process
                foreach (var prop in currentProc.GetType().GetProperties()) {
                    if (invalidInfos.Contains(prop)) continue;
                    try {
                        var name = prop.Name;
                        string value = $"{prop.GetValue(currentProc, null)}";
                        if (name.Contains("Memory")) {
                            long longVal;
                            var isLong = long.TryParse($"{value}", out longVal);
                            value = Utilities.Mr_Clean.FormatBytes(longVal);
                        }
                        Console.WriteLine($"{name} : {(string.IsNullOrWhiteSpace(value) ? "<Unkown>" : value)}");
                    } catch {
                        invalidInfos.Add(prop);
                    }
                }
            }
        }
    }
}
