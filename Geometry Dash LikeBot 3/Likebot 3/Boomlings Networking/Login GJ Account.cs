using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Boomlings_Networking
{
    public class Login_GJ_Account : Boomlings_Request_Base
    {
        public Login_GJ_Account(string username, string password, string udid, string uuid)
            : base("http://boomlings.com/database/accounts/loginGJAccount.php",
                  $"udid={udid}&userName={username}&" +
                  $"password={password}&secret=Wmfv3899gc9&" +
                  $"sID={uuid}")
        {

        }
    }
}
