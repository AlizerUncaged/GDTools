using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Boomlings_Networking
{
    public class Account_Data_Result
    {
        public bool Success;
        public int AccountID;
        public int PlayerID;
        public string UDID;
        public string UUID;
    }
    public class Login_GJ_Account : Boomlings_Request_Base
    {
        private string _udid, _uuid;
        public Login_GJ_Account(string username, string password, string udid, string uuid)
            : base("http://www.boomlings.com/database/accounts/loginGJAccount.php",
                  $"udid={udid}&userName={username}&" +
                  $"password={password}&secret=Wmfv3899gc9&" +
                  $"sID={uuid}")
        {
            _udid = udid; _uuid = uuid;
        }

        public async Task<Account_Data_Result> GetResult()
        {
            var result = new Account_Data_Result
            {
                UDID = _udid,
                UUID = _uuid
            };

            var response = await SendAsync();


            return result;
        }
    }
}
