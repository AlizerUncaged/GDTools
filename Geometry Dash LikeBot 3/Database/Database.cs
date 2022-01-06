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

        private static Data Data = new();

        public static bool IsExists(int accountid) {
            return Data.Accounts.FindIndex(x => x.AccountID == accountid) != -1;
        }

        public static void ChangePassword(int accountid, string password, string gjp) {
            var account = Data.Accounts[GetIndexFromAccountID(accountid)];
            account.Password = password; account.GJP = gjp;
            Save();
        }
        public static List<Account> Accounts {
            get {
                if (Data != null)
                    return Data.Accounts;
                else return null;
            }
        }

        public static Account AddAccount(Likebot_3.Boomlings_Networking.Account_Data_Result serverResponse, string username, string password, string gjp) {
            var account = new Account {
                AccountID = serverResponse.AccountID,
                PlayerID = serverResponse.PlayerID,
                UDID = serverResponse.UDID,
                UUID = serverResponse.UUID,
                Username = username,
                Password = password,
                GJP = gjp
            };

            Data.Accounts.Add(account);
            Logger.Debug($"{account.Username} - Account added: {account.Username}");
            Save();

            return account;
        }

        public static void AddAccount(int accountid, int playerid, string username, string password, string gjp) {
            var account = new Account {
                AccountID = accountid,
                PlayerID = playerid,
                Username = username,
                Password = password,
                GJP = gjp
            };

            Data.Accounts.Add(account);
            Logger.Debug($"{account.Username} - Account added: {account.Username}");
            Save();
        }

        public static int GetIndexFromAccountID(int accountid) {
            return Data.Accounts.FindIndex(x => x.AccountID == accountid);
        }

        public static void RemoveAccount(Account account) {
            Data.Accounts.Remove(account);
            Logger.Debug($"{account.Username} - Removed from database.");
            Save();
        }

        /// <summary>
        /// Get a <b>valid</b> account from the database.
        /// </summary>
        public static Account GetAccountFromSessionKey(string sessionkey) {
            if (string.IsNullOrWhiteSpace(sessionkey)) return null;

            var account = Data.Accounts.FirstOrDefault(x => x.CheckKey(sessionkey) && x.IsValid().IsValid);
            if (account != null)
                Logger.Info($"{account.Username} - Account found with session key {sessionkey}");
            else
                Logger.Info($"Attempt to get non-existing account with key {sessionkey}");
            return account;
        }

        public static Account GetAccountFromCredentials(string username, string password = null) {
            if (password == null)
                return Data.Accounts.FirstOrDefault(x =>
                x.Username.ToLower().Trim() == username.ToLower().Trim());
            else
                return Data.Accounts.FirstOrDefault(x =>
                x.Username.ToLower().Trim() == username.ToLower().Trim() &&
                x.Password.Trim() == password.Trim());
        }

        public static Account GetAccountViaAccountID(int AccountID) {
            return Data.Accounts.FirstOrDefault(x => x.AccountID == AccountID);
        }

        public static IEnumerable<Account> GetRandomAccounts(int howMany) {
            return Data.Accounts.Take(howMany).OrderBy(x => Utilities.Random_Generator.Random.Next());
        }

        // file streams to db
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
                Data = JsonConvert.DeserializeObject<Data>(dbContents);
            Logger.Info($"Account list loaded {Data.Accounts.Count()} Accounts...");
        }

        /// <summary>
        /// Saves the current database.
        /// </summary>
        public static void Save() {
            Logger.Debug($"Saving database...");
            var defaultDb = JsonConvert.SerializeObject(Data, Formatting.Indented);
            // clear database contents
            _dbFileStream.SetLength(0);
            // rewrite database
            _dbWriteStream.Write(defaultDb);
            _dbWriteStream.Flush();
            Logger.Info($"Database saved...");
        }
    }
}
