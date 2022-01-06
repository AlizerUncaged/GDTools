using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Database {
    // not struct because we'll have default values
    public class Contribution {
        public bool WasSuccess { get; set; }
        public Likebot_3.Boomlings_Networking.Like_Item Item { get; set; }

        public DateTime Date = DateTime.UtcNow;
    }
}
