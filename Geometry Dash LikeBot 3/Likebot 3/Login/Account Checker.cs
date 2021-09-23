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
        private string _username, _password;

        public Account_Checker(string username, string password)
        {
            _username = username; _password = password;
        }

        public (bool, Account_Data_Result) Check() { 
            // todo
        }
    }
}
