using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Boomlings_Networking {
    public enum ItemType {
        Level = 1
    }
    public struct Like_Item {
        public int ItemID { get; set; }
        public int SpecialID { get; set; }
        public bool IsLike { get; set; }
        public ItemType ItemType { get; set; }
    }
}
