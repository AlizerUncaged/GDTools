
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Core.Dashboard {
    public class LikeBot_Task {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private List<Task<bool>> tasks = new();
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
            this.owner.History.Add((likeItem, DateTime.UtcNow));

            const double expectancy = 0.75;

            int success = 0,
                failures = 0;

            int tries = 2;

            // if 75% of the likes are not achieved retry, do this 3 times
            while (tries > 0 && success < (maxLikes * expectancy)) {

                for (int i = 0; i < maxLikes; i++) {
                    tasks.Add(AddLike());
                }

                foreach (var task in tasks.ToList()) {
                    var result = await task;
                    if (result) success++;
                    else failures++;

                }

                tries--;
                if (success < maxLikes) {
                    Logger.Debug($"Max likes not achieved retrying ({tries}) requested by {owner.Username}");
                }
            }

            // add likes that were not sent
            // owner.Tier.LikesLeft += failures;
            // tasks done, remove it from tasks list

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
            toLike.Contributions.Add((likeItem, DateTime.UtcNow));

            var randS = Utilities.Random_Generator.RandomString(10);
            // add like
            var likeReq = new Boomlings_Networking.Like_GJ_Item(toLike, likeItem, randS);

            var doLike = await likeReq.SendAndGetValid();
            return doLike;
        }
    }
}
