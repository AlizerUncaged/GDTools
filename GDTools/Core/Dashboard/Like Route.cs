using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WatsonWebserver;

namespace GDTools.Core.Dashboard {
    public class Like_Route {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // returns 
        [StaticRoute(HttpMethod.POST, "/actions/like")]
        public async Task LikeItem(HttpContext ctx) {
            var cookies = new Cookie_Parser(ctx);

            if (!cookies.Parse()) {
                await cookies.MoveToLogin(ctx);
                return;
            }

            var sessionKey = cookies.GetSessionKey();
            // session key found!
            // get account from db which is Valid and has the key
            var account = Database.Database.GetUserFromSessionKey(sessionKey);
            // else return to login screen
            if (account == null) {
                await cookies.MoveToLogin(ctx);
                return;
            }

            // parse like request
            var requestPayload = ctx.Request.DataAsString;
            var fieldsAndValues = HttpUtility.ParseQueryString(requestPayload);

            // we cant pass struct properties to out keyword for some reason
            int itemID, specialID = 0, itemType = 1, maxLikes;
            bool isLike = true;

            var itemIDString = fieldsAndValues.Get("itemid");
            var specialIDString = fieldsAndValues.Get("specialid");
            var isLikeString = fieldsAndValues.Get("isLike");
            var itemTypeString = fieldsAndValues.Get("itemType");
            var maxLikesString = fieldsAndValues.Get("maxLikes");

            // make sure everything is valid
            if (!int.TryParse(itemIDString, out itemID)) {
                // itemid can never be null
                await cookies.MoveToLogin(ctx);
                return;
            }

            // specialID can be null
            int.TryParse(specialIDString, out specialID);

            bool.TryParse(isLikeString, out isLike);

            int.TryParse(itemTypeString, out itemType);

            int.TryParse(maxLikesString, out maxLikes);
            maxLikes = Math.Abs(maxLikes);
            // parse to likeItem
            var likeItem = new Boomlings_Networking.Like_Item();
            likeItem.IsLike = isLike;
            likeItem.ItemID = itemID;
            likeItem.SpecialID = specialID;
            likeItem.ItemType = (Boomlings_Networking.ItemType)itemType;

            Logger.Debug($"Like request, Type: {likeItem.ItemType} Max: {maxLikes} ItemID: {itemID} SpecialID: {specialID} IsLike: {isLike}");

            LikeBot_Task lTask = new(account, likeItem, maxLikes);
            await lTask.LikeBotAll();
            Logger.Debug("Done!");
        }
    }
}
