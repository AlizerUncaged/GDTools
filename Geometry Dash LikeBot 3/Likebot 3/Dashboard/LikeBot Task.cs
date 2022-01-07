
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Dashboard {
    public class LikeBot_Task {
        public static Dictionary<Database.Account, List<Task<bool>>>
            Tasks = new();

        public static bool HasTask(Database.Account acc) {
            return Tasks.ContainsKey(acc);
        }

        private int maxLikes;
        private Database.Account owner;
        private Database.Account[] likers;
        private Boomlings_Networking.Like_Item likeItem;
        public LikeBot_Task(Database.Account owner, Boomlings_Networking.Like_Item item, int max) {
            this.owner = owner; this.maxLikes = max;
            var ownerLikesLeft = this.owner.Tier.LikesLeft;
            likeItem = item;
            likers = Database.Database.GetRandomAccounts(
                // gd servers accept likes more than once lmao
                ownerLikesLeft > 512 ? 512 : ownerLikesLeft).ToArray();
        }

        // comence likebot
        public async Task<int> LikeBotAll() {
            if (!Tasks.TryGetValue(owner, out _))
                Tasks.Add(owner, new List<Task<bool>>());

            int success = 0;

            for (int i = 0; i < maxLikes; i++) {
                Tasks[owner].Add(AddLike());
            }

            foreach (var task in Tasks[owner]) {
                if (await task) success++;
            }

            // tasks done, remove it from tasks list
            Tasks.Remove(owner);

            return success;
        }
        private int activeAccount = 0;
        private Database.Account GetNextAccount() {
            var currentAccount = likers[activeAccount];
            activeAccount = activeAccount > likers.Count() - 1 ? 0 : activeAccount + 1;
            return currentAccount;
        }
        // adds one like
        public async Task<bool> AddLike() {
            // get random account
            var toLike = GetNextAccount();
            var randS = Utilities.Random_Generator.RandomString(10);
            // add like
            var likeReq = new Boomlings_Networking.Like_GJ_Item(toLike, likeItem, randS);
            return await likeReq.SendAndGetValid();

        }
    }
}
