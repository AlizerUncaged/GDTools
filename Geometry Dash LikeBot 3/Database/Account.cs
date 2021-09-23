using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Database
{
    public class Account
    {
        public int PlayerID, AccountID;

        public string Username;

        public string Password;

        public int UseCount;

        public string GJP;

        public Tiers Tier = Tiers.Free;

        public bool Valid;

        /// <summary>
        /// 512 character key.
        /// </summary>
        public string SessionID;
    }
}
