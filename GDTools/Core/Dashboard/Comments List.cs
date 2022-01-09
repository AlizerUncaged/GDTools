using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace GDTools.Core.Dashboard {
    public class Comments_List {

        public readonly string PostsPage = File.ReadAllText("Core/Dashboard/Find Posts.html");

        [StaticRoute(HttpMethod.GET, "/actions/getComments")]
        public async Task GetComments(HttpContext ctx) {


            if (!ctx.Request.Query.Elements.ContainsKey("username"))
                return;

            string targetAccountIDString;
            ctx.Request.Query.Elements.TryGetValue("username", out targetAccountIDString);

            var usernameToIDsParser = new Boomlings_Networking.Username_To_IDs(targetAccountIDString.Trim());
            var IDs = await usernameToIDsParser.GetIDs();

            int targetPlayerID = IDs.playerID;

            List<string> forms = new();
            var commentRequest = new Boomlings_Networking.Get_GJ_Comment_History(targetPlayerID);
            var foundComments = await commentRequest.GetCommentHistory();
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