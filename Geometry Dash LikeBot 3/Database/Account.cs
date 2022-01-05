using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Database {
    public class Account {
        [JsonIgnore]
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public DateTime LoginDate;

        public int PlayerID, AccountID;

        public string Username;

        public string Password;

        // server generated
        public string UDID;

        // server generated
        public string UUID;

        public string GJP;

        // default tier is Free
        public Tier Tier = Tiers.Free;

        // the amount of likes the account **gave** to other people's levels/comments
        public int UseCount;

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
            var isValid = IsValid();
            var sessionKey = Utilities.Random_Generator.RandomString(sessionKeyLength);
            SessionKey = sessionKey;

            Logger.Debug($"{AccountID} - Generated new session key {sessionKey}");
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
                return (false, "Account expired, you may register a new account at www.boomlings.com and log it in here.");

            return (true, "Account valid.");
        }

        /// <summary>
        /// Items the account liked.
        /// </summary>
        public List<(DateTime LikeDate, 
                     bool WasSuccess, 
                     Likebot_3.Boomlings_Networking.Like_Item Item)> Contributions = new();
    }
}
