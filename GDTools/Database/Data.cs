using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Database {
    public class Data {

        public List<User> Owners = new();

        public List<string> BannedIPAddresses = new();

        public List<string> BannedUserAgents = new();
    }
}
