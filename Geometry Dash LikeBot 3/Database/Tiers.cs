using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Database {
    public static class Tiers {
        public static Tier Free { get { return new Tier(512, 1); } }
        public static Tier VIP { get { return new Tier(1024, 7); } }
    }
    public class Tier {
        public int MaxLikes { get; set; }
        public int MaxAccountDays { get; set; }

        public Tier(int maxLikes, int maxDays) {
            MaxLikes = maxLikes;
            MaxAccountDays = maxDays;
        }
    }
}
