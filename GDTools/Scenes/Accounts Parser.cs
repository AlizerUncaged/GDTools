using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Scenes {
    public class Accounts_Parser : Scene {

        public Accounts_Parser() : base('B', "Add Accounts") {

        }
        public override async Task StartAsync() {
            Console.Clear();
            GC.Collect();


            Console.WriteLine(
                $"Paste new accounts and press Tab to start.");


            List<string> accountsList = new();

            StringBuilder buffer = new StringBuilder();

            ConsoleKeyInfo info = Console.ReadKey(true);

            while (info.Key != ConsoleKey.Tab) {
                string consoleKeyString = info.KeyChar.ToString();

                if (info.Key == ConsoleKey.Enter) {
                    consoleKeyString = Environment.NewLine;
                }

                Console.Write(consoleKeyString);
                buffer.Append(consoleKeyString);

                info = Console.ReadKey(true);
            }

            var stringBuffer = buffer.ToString();
            accountsList = stringBuffer.Split(Environment.NewLine).ToList();
            accountsList = accountsList
                .Where(x => !string.IsNullOrWhiteSpace(x) && x.Contains(":"))
                .ToList();


            Console.WriteLine($"Checking {accountsList.Count()} accounts...");

            // check accounts
            List<(Core.Boomlings_Networking.Login_GJ_Account loginAcc, string username, string password)> accountLogins = new();
            foreach (var account in accountsList) {
                try {
                    var accountSplit = account.Split(':');

                    var username = accountSplit[0];
                    var password = accountSplit[1];
                    var udid = Utilities.Random_Generator.RandomUDID();
                    var uuid = Utilities.Random_Generator.RandomUUID();

                    Core.Boomlings_Networking.Login_GJ_Account accountLogin = new(username, password, udid, uuid);
                    accountLogins.Add((accountLogin, username, password));
                } catch { }
            }

            Console.WriteLine($"Valid {accountLogins.Count()} combinations...");

            // get results 
            List<Database.Account> validAccounts = new();


            const int sub = 20;
            List<(Task<Core.Boomlings_Networking.Account_Data_Result> task, string username, string password)> accountLoginTasks = new();
            for (int i = 0; i < accountLogins.Count; i++) {
                var accountLogin = accountLogins[i];
                var loginTask = accountLogin.loginAcc.GetResult();
                accountLoginTasks.Add((loginTask, accountLogin.username, accountLogin.password));

                if (i % sub == 0) {
                    Console.WriteLine($"Sleeping...");
                    await Task.Delay(10000);
                }
            }
            foreach (var accountLoginTask in accountLoginTasks) {
                var result = await accountLoginTask.task;
                if (result.Success) {
                    string username = accountLoginTask.username, password = accountLoginTask.password;
                    var generatedGJP = Utilities.Robcryptions.PasswordToGJP(password);
                    var validAcc = Database.Database.AddAccount(result, username, password, generatedGJP, "alizer");
                    validAccounts.Add(validAcc);
                }
            }


            Console.WriteLine($"Valid {validAccounts.Count()} accounts...\r\nPress any key to continue.");

            Console.ReadKey();
        }
    }
}
