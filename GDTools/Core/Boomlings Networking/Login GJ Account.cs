using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Core.Boomlings_Networking {
    public class Account_Data_Result {
        public bool Error;
        public string Message;
        public bool Success;
        public int AccountID;
        public int PlayerID;
        public string UDID;
        public string UUID;
    }
    public class Login_GJ_Account : Boomlings_Request_Base {
        private string _udid, _uuid;
        public Login_GJ_Account(string username, string password, string udid, string uuid)
            : base($"/database/accounts/loginGJAccount.php",
                  $"udid={udid}&userName={username}&" +
                  $"password={password}&secret=Wmfv3899gc9&" +
                  $"sID={uuid}") {
            _udid = udid; _uuid = uuid;
        }

        public async Task<Account_Data_Result> GetResult() {
            var result = new Account_Data_Result {
                UDID = _udid,
                UUID = _uuid
            };

            var response = await SendAsync();
            response = response.Trim();
            if (!string.IsNullOrWhiteSpace(response))
                foreach (string line in response.Split('\n')) {
                    if (line.Contains(',')) {
                        var IDs = line.Split(',');
                        // accountid, playerid
                        result.Success = int.TryParse(IDs[0], out result.AccountID);
                        if (result.Success)
                            result.Success = int.TryParse(IDs[1], out result.PlayerID);
                        result.Message = "Successfully logged in!";
                    }
                }


            if (response == null) {
                result.Error = true;
                result.Message = "An error occured with the request.";
            } else if (response.Contains("-1"))
                result.Message = "Account rejected :( please make sure it's a valid Geometry Dash account.";

            return result;
        }
    }
}
