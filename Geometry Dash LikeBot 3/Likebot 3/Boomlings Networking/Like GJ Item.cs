using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Boomlings_Networking {
  
    public class Like_GJ_Item : Boomlings_Request_Base {
        public Like_GJ_Item(Database.Account account, Like_Item item, string randomString) :
            base($"/database/accounts/loginGJAccount.php",
                $"gameVersion=20&binaryVersion=35&gdw=0&accountID={account.AccountID}&gjp={account.GJP}" +
                $"&udid={account.UDID}&uuid={account.UUID}" +
                $"&itemID={item.ItemID}&like={(item.IsLike ? 1 : 0)}&type={(int)item.ItemType}" +
                $"&secret=Wmfd2893gb7&special={item.SpecialID}&rs={randomString}" +
                $"&chk={Utilities.Robcryptions.GetChk($"{item.SpecialID}", $"{item.ItemID}", $"{(item.IsLike ? 1 : 0)}", $"{(int)item.ItemType}", randomString, $"{account.AccountID}", $"{account.UDID}", $"{account.UUID}")}") {

        }
    }
}

/// Sample Request
//
//  POST / database / likeGJItem211.php HTTP / 1.1
//  Host: www.boomlings.com
//  Accept: */*
//  Content-Length: 270
//  Content-Type: application/x-www-form-urlencoded
//
//  gameVersion=20&binaryVersion=35&gdw=0&accountID=4726301&gjp=BA8CCg8LYUNbWFBe&udid=S15212997662534280161630320781155051002&uuid=170779410&itemID=77030648&like=0&type=1&secret=Wmfd2893gb7&special=0&rs=6ge86PKTSA&chk=AloADAVXXQIOVAILAwgHVFwBXAUMCVNdBAJeVgwEBAAHCgcMXlQKCA==HTTP/1.1 200 OK
//
/// Boomlings Response
//
//  Date: Mon, 03 Jan 2022 11:10:01 GMT
//  Content-Type: text/html
//  Transfer-Encoding: chunked
//  Connection: keep-alive
//  X-Powered-By: PHP/5.3.10-1ubuntu3.48
//  Vary: Accept-Encoding
//  CF-Cache-Status: DYNAMIC
//  Report-To: {"endpoints":[{"url":"https:\/\/a.nel.cloudflare.com\/report\/v3?s=QpGrxd531eLZMUAaegfvP4AhPOJvHuANqmlSUoXEsSqC%2B1HrAKIsp%2BTkr1STqdzbk%2FzOruamZLwEkSW7mGNefCdoi%2FmW%2Bg8SPQE905HKcudJHxJZd3oidqmKESDbYcriT9E9"}],"group":"cf-nel","max_age":604800}
//  NEL: {"success_fraction":0,"report_to":"cf-nel","max_age":604800}
//  Server: cloudflare
//  CF-RAY: 6c7bc7399edf880e-SIN
//
//  1
//  1
//  0

