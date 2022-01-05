using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Database {
    public class Database {

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static List<Account> Accounts = new();

        public static bool IsExists(int accountid) {
            return Accounts.FindIndex(x => x.AccountID == accountid) != -1;
        }

        public static void ChangePassword(int accountid, string password, string gjp) {
            var account = Accounts[GetIndexFromAccountID(accountid)];
            account.Password = password; account.GJP = gjp;
        }

        public static Account AddAccount(Likebot_3.Boomlings_Networking.Account_Data_Result serverResponse, string username, string password, string gjp) {
            var account = new Account {
                AccountID = serverResponse.AccountID,
                PlayerID = serverResponse.PlayerID,
                UDID = serverResponse.UDID,
                UUID = serverResponse.UUID,
                Username = username,
                Password = password,
                GJP = gjp,
                LoginDate = DateTime.Now
            };

            Accounts.Add(account);
            Logger.Debug($"{account.AccountID} - Account added: {account.Username}");

            return account;
        }

        public static void AddAccount(int accountid, int playerid, string username, string password, string gjp) {
            var account = new Account {
                AccountID = accountid,
                PlayerID = playerid,
                Username = username,
                Password = password,
                GJP = gjp,
                LoginDate = DateTime.Now
            };

            Accounts.Add(account);
            Logger.Debug($"{account.AccountID} - Account added: {account.Username}");
        }

        public static int GetIndexFromAccountID(int accountid) {
            return Accounts.FindIndex(x => x.AccountID == accountid);
        }

        public static void RemoveAccount(Account account) {
            Accounts.Remove(account);
        }

        /// <summary>
        /// Get a <b>valid</b> account from the database.
        /// </summary>
        public static Account GetAccountFromSessionKey(string sessionkey) {
            if (string.IsNullOrWhiteSpace(sessionkey)) return null;

            var account = Accounts.FirstOrDefault(x => x.CheckKey(sessionkey) && x.IsValid().IsValid);
            if (account != null)
                Logger.Info($"{account.AccountID} - Account found with session key {sessionkey}");
            else
                Logger.Info($"Attempt to get non-existing account with key {sessionkey}");
            return account;
        }

        public static Account GetAccountFromCredentials(string username, string password = null) {
            if (password == null)
                return Accounts.FirstOrDefault(x =>
                x.Username.ToLower().Trim() == username.ToLower().Trim());
            else
                return Accounts.FirstOrDefault(x =>
                x.Username.ToLower().Trim() == username.ToLower().Trim() &&
                x.Password.Trim() == password.Trim());
        }

        public static Account GetAccountViaAccountID(int AccountID) {
            return Accounts.FirstOrDefault(x => x.AccountID == AccountID);
        }

        public static IEnumerable<Account> GetRandomAccounts(int howMany) {
            return Accounts.Take(howMany).OrderBy(x => Utilities.Random_Generator.Random.Next());
        }

        private static FileStream _dbFileStream =
            File.Open(Constants.DatabaseFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        private static StreamReader _dbReadStream =
            new(_dbFileStream);
        private static StreamWriter _dbWriteStream =
            new(_dbFileStream);

        /// <summary>
        /// Refresh the database, reads the JSON file.
        /// </summary>
        public static async Task Read() {
            Logger.Debug("Reading database file...");
            var dbContents = await _dbReadStream.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(dbContents))
                Save();
            else
                Accounts = JsonConvert.DeserializeObject<List<Account>>(dbContents);
            Logger.Info($"Account list loaded {Accounts.Count()} accounts...");
        }

        /// <summary>
        /// Saves the current database.
        /// </summary>
        public static void Save() {
            var defaultDb = JsonConvert.SerializeObject(Accounts, Formatting.Indented);
            _dbFileStream.SetLength(0);
            _dbWriteStream.Write(defaultDb);
            _dbWriteStream.Flush();
        }
    }
}
