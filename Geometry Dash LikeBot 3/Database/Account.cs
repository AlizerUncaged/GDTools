using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Database {
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

        // default tier is Free
        public Tier Tier = Tiers.Free;

        [JsonProperty]
        /// <summary>
        /// CSRF token, very important.
        /// </summary>
        private string SessionKey;

        /// <summary>
        /// Items the account liked, total contributions to other accounts.
        /// </summary>
        public List<Contribution> Contributions = new();

        /// <summary>
        /// Attempts to generate a session key if the account is still valid.
        /// </summary>
        public (bool IsSuccess, string Reason, string Key) TryGenerateSessionKey() {
            // 64 is the optimal length, not too big, not too small
            const int sessionKeyLength = 64;
            var isValid = IsValid();
            var sessionKey = Utilities.Random_Generator.RandomString(sessionKeyLength);
            SessionKey = sessionKey;

            if (isValid.IsValid)
                Logger.Debug($"{Username} - Generated new session key {sessionKey}");
            else
                Logger.Debug($"{Username} - Is invalid.");
            return (isValid.IsValid, isValid.Reason, SessionKey);
        }

        public bool CheckKey(string key) {
            return SessionKey == key;
        }

        /// <summary>
        ///  Checks if the account is valid.
        /// </summary>
        public (bool IsValid, string Reason) IsValid() {
            // check maxdate
            var currentDate = DateTime.Now;
            var dateDifference = currentDate - LoginDate;
            var totalDays = dateDifference.TotalDays;
            var maxAllowedDays = Tier.MaxAccountDays;
            if (totalDays > maxAllowedDays)
                return (false, "Account expired, you may register a new account at www.boomlings.com and log it in here. Or buy VIP.");

            return (true, "Account valid.");
        }
    }
}
