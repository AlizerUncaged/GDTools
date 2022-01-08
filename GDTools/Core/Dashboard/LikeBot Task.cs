
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Core.Dashboard {
    public class LikeBot_Task {
        public static Dictionary<Database.Account, List<Task<bool>>> Tasks = new();

        public static bool HasTask(Database.Account acc) {
            return Tasks.ContainsKey(acc);
        }

        private int maxLikes;
        private Database.Account owner;
        private Database.Account[] likers;
        private Boomlings_Networking.Like_Item likeItem;
        public LikeBot_Task(Database.Account owner, Boomlings_Networking.Like_Item item, int max) {
            this.owner = owner;
            var ownerLikesLeft = this.owner.Tier.LikesLeft;
            this.maxLikes = max > ownerLikesLeft ? ownerLikesLeft : max;
            likeItem = item;

            likers = Database.Database.GetRandomAccounts(ownerLikesLeft > 512 ? 512 : ownerLikesLeft).ToArray();

            // negate maximum likes that can be given on the account.
            this.owner.Tier.LikesLeft -= max;
        }

        // comence likebot
        public async Task<int> LikeBotAll() {
            if (!Tasks.TryGetValue(owner, out _))
                Tasks.Add(owner, new List<Task<bool>>());

            int success = 0;
            Console.WriteLine($"Max likes {maxLikes}.");

            for (int i = 0; i < maxLikes; i++) {
                Tasks[owner].Add(AddLike());
            }

            foreach (var task in Tasks[owner]) {
                var result = await task;
                if (result) success++;

            }
            var failures = maxLikes - success;

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

            // Debug.WriteLine($"Account {toLike.Username} was valid {doLike}");

            return doLike;
        }
    }
}
