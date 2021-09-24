using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Database
{
    public class Account
    {
        public DateTime LoginDate;

        public int PlayerID, AccountID;

        public string Username;

        public string Password;

        // generated
        public string UDID;
        // generated
        public string UUID;

        public int UseCount;

        public string GJP;

        public Tiers Tier = Tiers.Free;

        public bool Valid;

        /// <summary>
        /// 512 character key.
        /// </summary>
        public string SessionsKey;

        public List<(bool /* is login success */, string /* ip address */)> Logins = new();
    }
}
