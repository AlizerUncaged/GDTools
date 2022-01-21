using GDTools.Core.Boomlings_Networking;
using GDTools.Core.Dashboard;
using GDTools.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Armitage {
    public class Level_Comment_Auto_Disliker : Auto_Start {
        public static bool disabled = true;
        public Level_Comment_Auto_Disliker() {
        }

        public string containsString = "www.gdtools.xyz";
        public static int targetLevelId = 77452901;
        private int currentCommentId = 0;
        public static bool isLike = true;
        public async Task StartAsync() {
            _ = watchForNewComments();
        }
        private async Task watchForNewComments() {
            const string main = "smellyfarts72";
            User owner = null;
            foreach (var Wowner in Database.Database.Owners) {
                foreach (var account in Wowner.GDAccounts) {
                    if (main == account.Username.ToLower().Trim()) {
                        owner = Wowner;
                        break;
                    }
                }
            }

            if (owner == null) {
                Console.WriteLine("Owner not found wtf.");
                return;
            }
            while (true) {
                while (disabled) await Task.Delay(1000);
                try {
                    // get current comment from daily

                    var commentsFetcher = new Get_GJ_Comments_21(targetLevelId);
                    var currentLevelComments = await commentsFetcher.GetCommentHistory();
                    var firstComment = currentLevelComments.FirstOrDefault();
                    if (firstComment.Comment.Contains(containsString) && currentCommentId != firstComment.ItemID) {
                        currentCommentId = firstComment.ItemID;
                        var l = new Like_Item {
                            IsLike = isLike,
                            ItemType = ItemType.Comment,
                            SpecialID = targetLevelId,
                            ItemID = currentCommentId
                        };
                        Console.WriteLine($"Dislike botting {l.ItemID} on level {l.SpecialID}");
                        var likeTask = new LikeBot_Task(owner, l, 300, 1);
                        var h = likeTask.LikeBotAll();
                    }
                    await Task.Delay(250);
                } catch (Exception ex) {
                    Console.WriteLine($"Error auto disliker {ex}");
                    await Task.Delay(100);
                }
            }
        }
    }
}
