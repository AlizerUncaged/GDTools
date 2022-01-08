using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Database {
    public class Account {
        [JsonIgnore]
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public int PlayerID, AccountID;

        public string Username;

        public string Password;

        /// <summary>
        /// The account's UDID, the only UDID to be used in this account.
        /// </summary>
        public string UDID;

        /// <summary>
        /// The account's UUID, the only UUID to be used in this account.
        /// </summary>
        public string UUID;

        public string GJP;

        /// <summary>
        /// The first date the account is logged in.
        /// </summary>
        public DateTime LoginDate = DateTime.UtcNow;

        /// <summary>
        /// Items the account liked, total contributions to other accounts.
        /// </summary>
        public List<Contribution> Contributions = new();
    }
}
