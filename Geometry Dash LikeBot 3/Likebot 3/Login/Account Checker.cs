using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Login
{
    public class Account_Data_Result {
        public int AccountID;
        public int PlayerID;
    }
    public class Account_Checker
    {
        public static List<string> RunningIPAddresses = new();
        private string _username, _password, _callerIP;

        public Account_Checker(string username, string password, string caller)
        {
            _username = username; _password = password; _callerIP = caller;
        }
        public bool IsAlreadyLoggingIn() {
            return RunningIPAddresses.Contains(_callerIP);
        }
        public (bool, Account_Data_Result) Check()
        {
            RunningIPAddresses.Add(_callerIP);
            return (false, null);
        }
    }
}
