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
            Console.Clear();
            GC.Collect();

            List<PropertyInfo> invalidInfos = new();

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
                // display all properties of current process
                StringBuilder sb = new();
                var propType = currentProc.GetType();
                foreach (var prop in propType.GetProperties()) {

                    var fType = prop.PropertyType;

                    if (fType.IsClass) {
                        invalidInfos.Contains(prop);
                        continue;
                    }

                    if (invalidInfos.Contains(prop)) continue;

                    try {
                        var name = prop.Name;
                        string value = $"{prop.GetValue(currentProc, null)}";

                        long longVal;
                        var isLong = long.TryParse($"{value}", out longVal);

                        //// if default value dont bother showing it
                        //if (isLong && longVal == 0) {
                        //    continue;
                        //}

                        if (name.Contains("Memory")) {
                            value = Utilities.Mr_Clean.FormatBytes(longVal);
                        }

                        sb.AppendLine($"{name} : {(string.IsNullOrWhiteSpace(value) ? "<Unkown>" : value)} ");
                    } catch {
                        invalidInfos.Add(prop);
                    }
                }

                Console.WriteLine(($"{sb}").Trim());
            }
        }
    }
}
