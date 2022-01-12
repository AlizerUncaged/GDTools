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

        public readonly string PostsPage = File.ReadAllText("Core/Dashboard/Find Posts.html");

        [StaticRoute(HttpMethod.GET, "/actions/getComments")]
        public async Task GetComments(HttpContext ctx) {


            if (!ctx.Request.Query.Elements.ContainsKey("username"))
                return;

            string username, type;
            ctx.Request.Query.Elements.TryGetValue("username", out username);
            ctx.Request.Query.Elements.TryGetValue("type", out type);
            username = username.ToLower().Trim();
            type = type.ToLower().Trim();

            (int accountID, int playerID) ids;

            if (!Database.Database.DBCache.UsernameAndIDs.TryGetValue(username, out ids)) {
                // if dictionary doesnt contain value
                var usernameToIDsParser = new Boomlings_Networking.Get_GJ_Users_20(username.Trim());
                var IDs = await usernameToIDsParser.GetIDs();
                if (!IDs.success) {
                    await ctx.Response.Send(@$"<html><p style='color:white'>{IDs.reason}</p></html>");
                    return;
                }
                ids = (IDs.accountID, IDs.playerID);
                Database.Database.DBCache.UsernameAndIDs.Add(username, ids);
            }
            Logger.Debug($"Attempt to get comments from {username} with ID {ids.playerID} for type {type}");

            int likeType = 2;
            string commentToShow = "<html><p style='color:white'>An error occured finding comments.</p></html>";
            switch (type) {
                case "comments": {
                        likeType = 2;
                        List<string> forms = new();
                        var commentRequest = new Boomlings_Networking.Get_GJ_Comment_History(ids.playerID);
                        var foundComments = await commentRequest.GetCommentHistory();
                        if (foundComments == null) {
                            // {ownerID}
                            await ctx.Response.Send(@"<html><p style='color:white'>Can't find comments, author account may have been banned.</p></html>");
                            return;
                        }

                        foreach (var comment in foundComments) {
                            var form = GetForm(comment.Comment, comment.ItemID, comment.SpecialID, comment.Age);
                            forms.Add(form);
                        }
                        commentToShow = string.Join(Environment.NewLine, forms);
                    }
                    break;
                case "posts": {
                        likeType = 3;
                        List<string> forms = new();
                        var commentRequest = new Boomlings_Networking.Get_GJ_Account_Comments_20(ids.accountID);
                        var foundComments = await commentRequest.GetPostsAsync();
                        if (foundComments == null) {
                            // {ownerID}
                            await ctx.Response.Send(@"<html><p style='color:white'>Can't find posts, author account may have been banned.</p></html>");
                            return;
                        }
                        foreach (var comment in foundComments) {
                            var form = GetForm(comment.Post, comment.ItemID, ids.accountID, comment.Age);
                            forms.Add(form);
                        }
                        commentToShow = string.Join(Environment.NewLine, forms);
                    }
                    break;
            }

            // {ownerID}
            await ctx.Response.Send(PostsPage
                .Replace("{comments}", commentToShow.ToString())
                .Replace("{type}", $"{likeType}")
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