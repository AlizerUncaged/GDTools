using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Core.Boomlings_Networking {
    public struct AccountPost {
        // special id is account id too
        public int ItemID; // 6
        public string Post; // 2
        public string Age; // 9
    }
    public class Get_GJ_Account_Comments_20 : Boomlings_Request_Base {
        public Get_GJ_Account_Comments_20(int accountID) : base("/database/getGJAccountComments20.php",
            $"gameVersion=21&binaryVersion=35&gdw=0&accountID={accountID}&page=0&total=0&secret=Wmfd2893gb7") {
        }

        public async Task<IEnumerable<AccountPost>> GetPostsAsync() {
            var serverResponse = await SendAsync(ProxyType.PaidProxy);

            if (serverResponse == null) return null;
            if (!serverResponse.Contains("~")) return null;

            List<AccountPost> comments = new();

            var commentStructStrings = serverResponse.Split('|');
            foreach (var structString in commentStructStrings) {
                var structStringButNotInTheForLoop = structString;
                if (structStringButNotInTheForLoop.Contains('#'))
                    structStringButNotInTheForLoop = structStringButNotInTheForLoop.Split('#').FirstOrDefault();
                var levelCommentStruct = ParseStruct(structStringButNotInTheForLoop);
                comments.Add(levelCommentStruct);
            }

            return comments;

        }
        public AccountPost ParseStruct(string structString) {
            Dictionary<string, string> keyAndVal = new();
            var splitted = structString.Split('~');

            for (int i = 0; i < splitted.Length; i++) {
                if (i % 2 == 0) {
                    // if even then its key else val
                } else {
                    var key = splitted[i - 1];
                    var val = splitted[i];
                    // it is val now
                    keyAndVal.Add(key, val);
                }
            }

            var commentBase64String = keyAndVal["2"];
            var commentString = "[Error occured decrypting comment.]";

            try {
                commentString = Utilities.Encryptions.Base64Decode(commentBase64String);
            } catch { }

            return new AccountPost() {
                ItemID = int.Parse(keyAndVal["6"]),
                Post = commentString,
                Age = keyAndVal["9"]
            };
        }
    }
}
