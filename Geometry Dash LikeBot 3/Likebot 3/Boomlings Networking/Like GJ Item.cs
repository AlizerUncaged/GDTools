using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Boomlings_Networking
{
    public class Like_GJ_Item : Boomlings_Request_Base
    {
        public Like_GJ_Item(Database.Account account, int itemID, int specialID, string rs, string chk): 
            base("http://www.boomlings.com/database/accounts/loginGJAccount.php",
                $"")
            {
            
            }
    }
}
