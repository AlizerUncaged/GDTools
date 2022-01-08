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

namespace GDTools.Database {
    public class Database {

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Data Data = new();

        public static bool IsExists(int accountid) {
            return Accounts.Any(x => x.AccountID == accountid);
        }

        public static void ChangePassword(int accountid, string password, string gjp) {
            var account = Accounts.FirstOrDefault(x => x.AccountID == accountid);
            if (account == null) return;
            account.Password = password; account.GJP = gjp;
            Save();
        }

        public static IEnumerable<Account> Accounts {
            get {
                if (Data != null)
                    return Data.Owners.SelectMany(x => x.GDAccounts);
                else return null;
            }
        }
        public static List<User> Owners {
            get {
                if (Data != null)
                    return Data.Owners;
                else return null;
            }
        }
        public static User GetOwnerFromAccountID(int accID) {
            foreach (var owner in Data.Owners) {
                foreach (var account in owner.GDAccounts) {
                    if (account.AccountID == accID)
                        return owner;
                }
            }
            return null;
        }

        public static Account AddAccount(Core.Boomlings_Networking.Account_Data_Result serverResponse, string username, string password, string gjp) {
            var ownerID = Data.Owners.Count;
            var account = new Account {
                AccountID = serverResponse.AccountID,
                PlayerID = serverResponse.PlayerID,
                UDID = serverResponse.UDID,
                UUID = serverResponse.UUID,
                Username = username,
                Password = password,
                GJP = gjp,
            };

            var owner = GetOwnerViaID(ownerID);
            if (owner == null) {
                owner = GenerateNewOwner(username);
            }
            owner.GDAccounts.Add(account);

            Logger.Debug($"{account.Username} - Account added: {account.Username}");
            Save();

            return account;
        }
        public static User GenerateNewOwner(string username) {
            var newUser = new User(Data.Owners.Count);
            newUser.Username = username;
            Data.Owners.Add(newUser);
            return newUser;
        }

        public static User GetOwnerViaID(int ownerID) {
            return Data.Owners.FirstOrDefault(x => x.OwnerID == ownerID);
        }

        public static void RemoveAccount(Account account) {
            var accountOwner = Data.Owners.Where(x => x.GDAccounts.Any(y => y.AccountID == account.AccountID)).FirstOrDefault();
            if (accountOwner == null) return;
            accountOwner.GDAccounts.Remove(account);
            Logger.Debug($"{account.Username} - Removed from database.");
            Save();
        }

        /// <summary>
        /// Get a <b>valid</b> account from the database.
        /// </summary>
        public static User GetUserFromSessionKey(string sessionkey) {
            if (string.IsNullOrWhiteSpace(sessionkey)) return null;

            var account = Data.Owners.FirstOrDefault(x => x.CheckKey(sessionkey));
            // var account = Data.Owners.acc.FirstOrDefault(x => x.CheckKey(sessionkey) && x.IsValid().IsValid);
            if (account != null)
                Logger.Info($"{account.Username} - Account found with session key {sessionkey}");
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
            return Accounts.OrderBy(x => Utilities.Random_Generator.Random.Next()).Take(howMany);
        }

        public static bool IsUserAgentBanned(string ua) {
            return Data.BannedUserAgents.Contains(ua);
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
            Logger.Info($"Account list loaded {Accounts.Count()} Accounts...");

            Logger.Debug("Fetching banned User-Agents...");
            const string botUserAgentsSource = "https://raw.githubusercontent.com/monperrus/crawler-user-agents/master/crawler-user-agents.json";
            var gitBotUserAgents = Utilities.Quick_TCP.ReadURL(botUserAgentsSource).Result;
            var enumerated = JsonConvert.DeserializeObject<Crawler_User_Agents[]>(gitBotUserAgents);

            Data.BannedUserAgents = enumerated.SelectMany(x => x.Instances).ToList();
            Logger.Info($"Banned User-Agent list loaded {Data.BannedUserAgents.Count()} User-Agents banned...");
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
