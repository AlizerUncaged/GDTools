using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Database {
    /// <summary>
    /// Cache.
    /// </summary>
    public class Cache {

        public Dictionary<string, (int playerID, int accountID)> UsernameAndIDs = new();
    }
}
