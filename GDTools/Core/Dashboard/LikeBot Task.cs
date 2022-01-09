
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Core.Dashboard {
    public class LikeBot_Task {
        public static Dictionary<Database.User, List<Task<bool>>> Tasks = new();

        public static bool HasTask(Database.User acc) {
            return Tasks.ContainsKey(acc);
        }

        private int maxLikes;
        private Database.User owner;
        private Database.Account[] likers;
        private Boomlings_Networking.Like_Item likeItem;
        public LikeBot_Task(Database.User owner, Boomlings_Networking.Like_Item item, int max) {
            this.owner = owner;
            var ownerLikesLeft = this.owner.Tier.LikesLeft;
            this.maxLikes = max > ownerLikesLeft ? ownerLikesLeft : max;
            likeItem = item;

            likers = Database.Database.GetRandomAccounts(ownerLikesLeft > 512 ? 512 : ownerLikesLeft).ToArray();
            int likersCount = likers.Count();
            // if the maximum likes are too many nerf it
            maxLikes = likersCount < maxLikes ? likersCount : maxLikes;
        }

        // comence likebot
        public async Task<int> LikeBotAll() {
            // negate maximum likes that can be given on the account.
            this.owner.Tier.LikesLeft -= maxLikes;

            if (!Tasks.TryGetValue(owner, out _))
                Tasks.Add(owner, new List<Task<bool>>());

            int success = 0, 
                failures = 0;

            for (int i = 0; i < maxLikes; i++) {
                Tasks[owner].Add(AddLike());
            }

            foreach (var task in Tasks[owner].ToList()) {
                var result = await task;
                if (result) success++;
                else failures++;
            }

            // add likes that were not sent
            owner.Tier.LikesLeft += failures;
            // tasks done, remove it from tasks list
            Tasks.Remove(owner);

            GC.Collect();

            return success;
        }

        private int activeAccount = -1;

        private Database.Account GetNextAccount() {
            activeAccount++;
            activeAccount = activeAccount >= likers.Count() ? 0 : activeAccount;
            var currentAccount = likers[activeAccount];
            return currentAccount;
        }
        // adds one like
        private async Task<bool> AddLike() {
            // get random account
            var toLike = GetNextAccount();
            var randS = Utilities.Random_Generator.RandomString(10);
            // add like
            var likeReq = new Boomlings_Networking.Like_GJ_Item(toLike, likeItem, randS);

            var doLike = await likeReq.SendAndGetValid();
            return doLike;
        }
    }
}
