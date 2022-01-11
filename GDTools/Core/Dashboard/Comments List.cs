using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace GDTools.Core.Dashboard {
    public class Comments_List {

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<string, int> cachedUsernameAndPlayerID = new();
        public readonly string PostsPage = File.ReadAllText("Core/Dashboard/Find Posts.html");

        [StaticRoute(HttpMethod.GET, "/actions/getComments")]
        public async Task GetComments(HttpContext ctx) {


            if (!ctx.Request.Query.Elements.ContainsKey("username"))
                return;

            string username;
            ctx.Request.Query.Elements.TryGetValue("username", out username);
            username = username.ToLower().Trim();

            int targetPlayerID = 0;

            if (!cachedUsernameAndPlayerID.TryGetValue(username, out targetPlayerID)) {
                // if dictionary doesnt contain value
                var usernameToIDsParser = new Boomlings_Networking.Username_To_IDs(username.Trim());
                var IDs = await usernameToIDsParser.GetIDs();
                targetPlayerID = IDs.playerID;
                cachedUsernameAndPlayerID.Add(username, targetPlayerID);
            }
            Logger.Debug($"Attempt to get comments from {username} with ID {targetPlayerID}.");

            List<string> forms = new();
            var commentRequest = new Boomlings_Networking.Get_GJ_Comment_History(targetPlayerID);
            var foundComments = await commentRequest.GetCommentHistory();
            if (foundComments == null) {
                // {ownerID}
                await ctx.Response.Send(@"<html><p style='color:white'>Can't find comments, author account may have been banned.</p></html>");
            }

            foreach (var comment in foundComments) {
                var form = GetForm(comment.Comment, comment.ItemID, comment.SpecialID, comment.Age);
                forms.Add(form);
            }


            // {ownerID}
            await ctx.Response.Send(PostsPage
                .Replace("{comments}", string.Join(Environment.NewLine, forms))
                );
            return;

        }

        public string GetForm(string comment, int commentID, int levelID, string age) {
            string form =
             @$"
<div class='p-2'>
            <label class='form-check-label'>
                <input class='form-check-input' type='radio' name='itemAndSpecial' value='{commentID}:{levelID}'>
                        <strong>Level: {levelID}, Posted: {age}</strong>
                        <div>{comment}</div>
            </label>
</div>";
            return form;
        }
    }
}

/*

        
 */