using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Database {
    public class User {
        [JsonIgnore]
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public User(int id) {
            OwnerID = id;
        }

        public List<Account> GDAccounts = new();

        public int OwnerID;

        public string Username { get; set; }

        // default tier is Free
        public Tier Tier = Tiers.Free;

        [JsonProperty]
        /// <summary>
        /// CSRF token, very important.
        /// </summary>
        private string SessionKey;

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
