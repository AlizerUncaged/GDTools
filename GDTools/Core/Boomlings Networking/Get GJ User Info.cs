using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Core.Boomlings_Networking {

    public struct GetGJUserInfoResponse {

        public string Username; // 1
        public int PlayerID; // 2
        public int AccountID; // 16
    }
    public class Get_GJ_User_Info : Boomlings_Request_Base {
        public Get_GJ_User_Info(int accountID, string udid, string uuid) : base($"/database/getGJUserInfo20.php",
            $"gameVersion=21&binaryVersion=35&gdw=0&udid={udid}&uuid={uuid}&targetAccountID={accountID}&secret=Wmfd2893gb7") {
        }

        public async Task<GetGJUserInfoResponse?> GetUserInfoAsync() {
            // paid proxy since this wont really be used much and its not really that sus of an endpoint
            var serverResponse = await SendAsync(ProxyType.PaidProxy);

            if (serverResponse == null) return null;

            const char separator = ':';
            if (!serverResponse.Contains(separator)) return null;

            Dictionary<string, string> keyAndVal = new();
            var splitted = serverResponse.Split(separator);

            for (int i = 0; i < splitted.Length; i++) {
                if (i % 2 == 0) {
                    // if even then its key else val
                } else {
                    var key = splitted[i - 1];
                    var val = splitted[i];
                    // it is val now
                    keyAndVal.Add(key, val);
                } 
            }
            return new GetGJUserInfoResponse {
                Username = keyAndVal["1"],
                PlayerID = int.Parse(keyAndVal["2"]),
                AccountID = int.Parse(keyAndVal["16"])
            };
        }
    }
}
