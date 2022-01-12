using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Core.Boomlings_Networking {
    public struct LevelComment {
        public string Comment;
        public int CommenterAccID;
        public int ItemID; // between ~6~ and ~11~

        /// <summary>
        /// Or Level ID.
        /// </summary>
        public int SpecialID; // between ~1~ and ~3~
        public string Age;

    }
    public class Get_GJ_Comment_History : Boomlings_Request_Base {

        public Get_GJ_Comment_History(int fromAccountID, int count = 75) : base("/database/getGJCommentHistory.php",
            $"gameVersion=21&binaryVersion=35&gdw=0&page=0&total=0&secret=Wmfd2893gb7&mode=0&userID={fromAccountID}&count={count}"
            ) {

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
                SpecialID = int.Parse(keyAndVal["1"]),
                Age = keyAndVal["9"]
            };
        }
    }
}

/*
 
 
aba
2~bmljZSBsZXZlbA==~1~77232566~3~8157792~4~0~10~100~9~1 hour~6~35621472~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|
2~cXVlIGJvbml0byA6bw==~1~77150731~3~8157792~4~6~10~0~9~8 hours~6~35603285~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|
2~YnVlbm9zIGNvbG9yZXM=~1~77177716~3~8157792~4~27~10~0~9~2 days~6~35454795~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|
2~Z2c=~1~77182813~3~8157792~4~20~10~100~9~2 days~6~35454443~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|
2~c2kgbGUgc2UgOlY=~1~77163306~3~8157792~4~11~10~100~9~2 days~6~35453568~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|
2~Z2c=~1~77103457~3~8157792~4~7~10~100~9~2 days~6~35452470~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|2~Z2c=~1~76966516~3~8157792~4~5~10~100~9~2 days~6~35448900~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|2~Ym9uaXRvIG5pdmVs~1~77113581~3~8157792~4~19~10~0~9~3 days~6~35382164~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|2~bmljZSBsZXZlbCBtaSBhbWlnbw==~1~77103941~3~8157792~4~9~10~100~9~3 days~6~35381502~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|2~VkFNT09PTyBQSUJFRUVFRQ==~1~76962930~3~8157792~4~35~10~2~9~3 days~6~35371755~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|2~bWUgZW5jYW50bw==~1~76776320~3~8157792~4~22~10~0~9~4 days~6~35316889~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|2~OjMgYm9ubm5pdG8=~1~64392488~3~8157792~4~13~10~0~9~4 days~6~35316457~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|2~bmljZSBidWVuaXNpbWFhYQ==~1~77107544~3~8157792~4~8~10~0~9~4 days~6~35316090~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|2~bmljZQ==~1~74689789~3~8157792~4~15~10~100~9~5 days~6~35250659~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|2~YXdlc29tZQ==~1~77086488~3~8157792~4~21~10~100~9~6 days~6~35173449~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|2~ZXMgYnVlbmlzaW1vb28=~1~72311226~3~8157792~4~14~10~0~9~6 days~6~35162487~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|2~ZXN0YSBidWVubyBlbCBmbGFwcHkgdWZv~1~76880964~3~8157792~4~7~10~0~9~6 days~6~35161952~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|2~dmVyeSBuaWNl~1~76336526~3~8157792~4~8~10~69~9~6 days~6~35161813~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|2~Z28gMjAyMg==~1~76951594~3~8157792~4~9~10~100~9~6 days~6~35160006~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207|
2~dXd1~1~76507964~3~8157792~4~6~10~0~9~6 days~6~35155560~11~1~12~200,255,200:1~danolex~9~46~10~3~11~11~14~0~15~2~16~1449207#999:0:20
0


 */
