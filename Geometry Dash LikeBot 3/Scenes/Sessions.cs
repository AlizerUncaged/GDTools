using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Scenes {
    public class Sessions : Scene {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Sessions() : base('C', "Sessions") {
        }

        public override async Task StartAsync() {
            Console.Clear();

            Console.WriteLine(
                $"Press ESC to exit.");

            var accounts = Database.Database.Accounts;
            Console.WriteLine($"Accounts (Total: {accounts.Count})");

            foreach (var account in accounts) {
                Console.WriteLine($"{accounts.IndexOf(account)}: {account.Username} Created: {account.LoginDate}");
            }

            while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
        }
    }
}
