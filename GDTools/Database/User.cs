using GDTools.Core.Boomlings_Networking;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Database {
    /// <summary>
    /// An <b>Owner</b> of accounts, people can login via any GD Accounts associated with the Owner.
    /// </summary>
    public class User {
        [JsonIgnore]
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public User(string id) {
            OwnerID = id;
        }

        /// <summary>
        /// Geometry Dash accounts logged in by the owner.
        /// </summary>
        public List<Account> GDAccounts = new();

        public string OwnerID;

        public string Username = "Owner";

        // not used but probably in the future
        public string Password = Utilities.Random_Generator.RandomString(8);

        // default tier is Free
        public Tier Tier = new();

        public List<(Like_Item item, DateTime date)> History = new();

        [JsonProperty]
        /// <summary>
        /// CSRF token, very important.
        /// </summary>
        private string SessionKey;

        public void AppendAccount(Account gdAccount) {
            GDAccounts.Add(gdAccount);
            const int additionalLikes = 100;
            Tier.LikesLeft += additionalLikes;
        }

        /// <summary>
        /// Attempts to generate a session key if the account is still valid.
        /// </summary>
        public (bool IsSuccess, string Reason, string Key) TryGenerateSessionKey() {
            // 64 is the optimal length, not too big, not too small
            const int sessionKeyLength = 64;
            var sessionKey = Utilities.Random_Generator.RandomString(sessionKeyLength);
            SessionKey = sessionKey;

            Logger.Debug($"{OwnerID} - Generated new session key {sessionKey}");
            return (true, string.Empty, SessionKey);
        }

        public bool CheckKey(string key) {
            return SessionKey == key;
        }

    }
}
