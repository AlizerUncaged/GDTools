using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Database {
    // not struct because we'll have default values
    public class Contribution {
        public bool WasSuccess { get; set; }
        public Core.Boomlings_Networking.Like_Item Item { get; set; }

        public DateTime Date = DateTime.UtcNow;
    }
}
