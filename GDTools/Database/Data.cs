using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Database {
    public class Data {

        public List<Account> Accounts = new();

        public List<string> BannedIPAddresses = new();

        public List<string> BannedUserAgents = new();
    }
}
