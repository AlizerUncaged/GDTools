using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Boomlings_Networking {
    public enum ItemType {
        Level = 1
    }
    public class Like_GJ_Item : Boomlings_Request_Base {
        public Like_GJ_Item(Database.Account account, int itemID, bool isLike, ItemType itemType, int specialID, string rs, string chk) :
            base("http://www.boomlings.com/database/accounts/loginGJAccount.php",
                $"gameVersion=20&binaryVersion=35&gdw=0&accountID={account.AccountID}&gjp={account.GJP}" +
                $"&udid={Utilities.Random_Generator.RandomUDID()}&uuid={Utilities.Random_Generator.RandomUUID()}" +
                $"&itemID={itemID}&like={(isLike ? 1 /* 1 = like */ : 0 /* 0 = dislike */)}&type={(int)itemType}" +
                $"&secret=Wmfd2893gb7&special={specialID}&rs={rs}&chk={chk}") {

        }
    }
}
