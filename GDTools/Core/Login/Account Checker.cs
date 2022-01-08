using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Core.Login {
    public class Account_Checker {
        /// <summary>
        /// IPAddresses with accounts still logging in.
        /// </summary>
        public static List<string> RunningIPAddresses = new();
        private string username, password, callerIP;

        public Account_Checker(string username, string password, string caller) {
            this.username = username; this.password = password; callerIP = caller;
        }
        public bool IsAlreadyLoggingIn() {
            return RunningIPAddresses.Contains(callerIP);
        }
        public async Task<((bool isSuccess, bool wasRouge) Result, Boomlings_Networking.Account_Data_Result ServerResult)> Check() {

            if (IsAlreadyLoggingIn()) return ((false, false), null);

            Boomlings_Networking.Account_Data_Result result = null;
            bool isSuccess = false;

            if (username.Length <= 2)
                return ((isSuccess, false), result);
            if (password.Length <= 2)
                return ((isSuccess, false), result);


            RunningIPAddresses.Add(callerIP);

            Boomlings_Networking.Login_GJ_Account loginAcc =
                new(username, password, Utilities.Random_Generator.RandomUDID(), Utilities.Random_Generator.RandomUUID());

            result = await loginAcc.GetResult();
            isSuccess = result.Success;

            RunningIPAddresses.Remove(callerIP);

            return ((isSuccess, false), result);
        }
    }
}
