using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Login
{
    public class Account_Checker
    {
        public static List<string> RunningIPAddresses = new();
        private string _username, _password, _callerIP;

        public Account_Checker(string username, string password, string caller)
        {
            _username = username; _password = password; _callerIP = caller;
        }
        public bool IsAlreadyLoggingIn()
        {
            return RunningIPAddresses.Contains(_callerIP);
        }
        public async Task<(bool, Boomlings_Networking.Account_Data_Result)> Check()
        {
            Boomlings_Networking.Account_Data_Result result = null;
            bool isSuccess = false;

            if (_username.Length <= 2)
                return (isSuccess, result);
            if (_password.Length <= 2)
                return (isSuccess, result);

            RunningIPAddresses.Add(_callerIP);

            Boomlings_Networking.Login_GJ_Account loginAcc = 
                new(_username, _password, Utilities.Random_Generator.RandomUDID(), Utilities.Random_Generator.RandomUUID());

            result = await loginAcc.GetResult();
            isSuccess = result.Success;

            RunningIPAddresses.Remove(_callerIP);

            return (isSuccess, result);
        }
    }
}
