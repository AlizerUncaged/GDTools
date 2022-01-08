using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Scenes {
    public class Sessions : Scene {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Sessions() : base('C', "Sessions") {
        }

        public override async Task StartAsync() {
            Console.Clear();

            Console.WriteLine(
                $"Press ESC to exit.");

            var accounts = Database.Database.Accounts;
            Console.WriteLine($"Accounts (Total: {accounts.Count()})");

            foreach (var owner in Database.Database.Owners) {
                Console.WriteLine($"{Database.Database.Owners.IndexOf(owner)}, {owner.Username}'s GD Accounts: ");
                var ownerAccounts = owner.GDAccounts;
                foreach (var account in ownerAccounts) {
                    Console.WriteLine($"\t {ownerAccounts.IndexOf(account)}: {account.Username}");
                }

            }
            while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
        }
    }
}
