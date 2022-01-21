using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Core.Boomlings_Networking {
    public class Get_GJ_Comments_21 : Boomlings_Request_Base {
        private int levelID;
        public Get_GJ_Comments_21(int levelId) : base($"/database/getGJComments21.php", 
            $"gameVersion=21&binaryVersion=35&gdw=0&page=0&total=0&secret=Wmfd2893gb7&mode=0&levelID={levelId}&count=20") {
            levelID = levelId;
        }

        public async Task<List<LevelComment>> GetCommentHistory() {
            var serverResponse = await SendAsync(ProxyType.PaidProxy);

            if (serverResponse == null) return null;
            if (!serverResponse.Contains("~")) return null;

            List<LevelComment> comments = new();

            var commentStructStrings = serverResponse.Split('|');
            foreach (var structString in commentStructStrings) {
                var structStringButNotInTheForLoop = structString;
                if (structStringButNotInTheForLoop.Contains(':'))
                    structStringButNotInTheForLoop = structStringButNotInTheForLoop.Split(':').FirstOrDefault();
                var levelCommentStruct = ParseStruct(structStringButNotInTheForLoop);
                comments.Add(levelCommentStruct);
            }

            return comments;
        }
        public LevelComment ParseStruct(string structString) {

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

            return new LevelComment() {
                Comment = commentString,
                CommenterAccID = int.Parse(keyAndVal["3"]),
                ItemID = int.Parse(keyAndVal["6"]),
                SpecialID = levelID,
                Age = keyAndVal["9"]
            };
        }
    }
}
