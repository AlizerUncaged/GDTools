using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Database
{
    // yes, database is a JSON file, who cares anyways
    public class Data
    {
        public static List<Account> Accounts = new();

        public static bool IsExists(int accountid)
        {
            return Accounts.FindIndex(x => x.AccountID == accountid) != -1;
        }

        public static void ChangePassword(int accountid, string password, string gjp, string key)
        {
            var account = Accounts[GetIndexFromAccountID(accountid)];
            account.Password = password; account.GJP = gjp; account.SessionsKey = key;
        }

        public static void AddAccount(Likebot_3.Boomlings_Networking.Account_Data_Result serverResponse, string username, string password, string gjp, string key)
        {
            var account = new Account
            {

                AccountID = serverResponse.AccountID,
                PlayerID = serverResponse.PlayerID,
                UDID = serverResponse.UDID,
                UUID = serverResponse.UUID,
                Username = username,
                Password = password,
                GJP = gjp,
                SessionsKey = key,
                LoginDate = DateTime.Now
            };

            Accounts.Add(account);
        }

        public static void AddAccount(int accountid, int playerid, string username, string password, string gjp, string key)
        {
            var account = new Account
            {
                AccountID = accountid,
                PlayerID = playerid,
                Username = username,
                Password = password,
                GJP = gjp,
                SessionsKey = key,
                LoginDate = DateTime.Now
            };

            Accounts.Add(account);
        }

        public static int GetIndexFromAccountID(int accountid)
        {
            return Accounts.FindIndex(x => x.AccountID == accountid);
        }

        public static void RemoveAccount(Account account)
        {
            Accounts.Remove(account);
        }

        public static Account GetAccountFromSessionKey(string sessionkey)
        {
            if (string.IsNullOrWhiteSpace(sessionkey)) return null;
            return Accounts.FirstOrDefault(x => x.SessionsKey == sessionkey);
        }

        public static Account GetAccountFromCredentials(string username, string password = null)
        {
            if (password == null)
                return Accounts.FirstOrDefault(x =>
                x.Username.ToLower().Trim() == username.ToLower().Trim());
            else
                return Accounts.FirstOrDefault(x =>
                x.Username.ToLower().Trim() == username.ToLower().Trim() &&
                x.Password.Trim() == password.Trim());
        }

        public static Account GetAccountViaAccountID(int AccountID)
        {
            return Accounts.FirstOrDefault(x => x.AccountID == AccountID);
        }

        public static IEnumerable<Account> GetRandomAccounts(int howMany)
        {
            return Accounts.Take(howMany).OrderBy(x => Utilities.Random_Generator.Random.Next());
        }

        public static void Read()
        {
            if (!File.Exists(Constants.DatabaseFile))
            {
                File.Create(Constants.DatabaseFile).Close();
                File.WriteAllText(Constants.DatabaseFile, JsonConvert.SerializeObject(Accounts));
            }

            Accounts = JsonConvert.DeserializeObject<List<Account>>(File.ReadAllText(Constants.DatabaseFile));
        }

        public static void Save()
        {
            File.WriteAllText(Constants.DatabaseFile, JsonConvert.SerializeObject(Accounts, Formatting.Indented));
        }
    }
}
