using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Dashboard
{
    public class Dashboard_Route
    {
        public async Task MoveToLogin(HttpContext ctx)
        {

            ctx.Response.StatusCode = 302;
            ctx.Response.Headers["Location"] = "/login";
            await ctx.Response.Send();
            return;
        }
        // shouldve cookie already
        [StaticRoute(HttpMethod.GET, "/dashboard")]
        public async Task EnterDashboard(HttpContext ctx)
        {

            string cookies;
            var cookiesFound = ctx.Request.Headers.TryGetValue("Cookie", out cookies);

            if (!cookiesFound)
            {
                // we have to fucking return to save server resources because it doesnt
                // automatically return after doing a .Send();
                await MoveToLogin(ctx); return;
            }

            var keysAndCookies = Utilities.Quick_TCP.ParseCookie(cookies);
            string sessionKey;
            var sessionKeyFound = keysAndCookies.TryGetValue("SessionsKey", out sessionKey);
            if (!sessionKeyFound)
            {
                await MoveToLogin(ctx); return;
            }

            // session key found!
            var account = Database.Data.GetAccountFromSessionKey(sessionKey);
            if (account == null)
            {
                await MoveToLogin(ctx); return;
            }

            await ctx.Response.Send(
                (await File.ReadAllTextAsync("Likebot 3/Dashboard/Dashboard.html"))
                .Replace("{username}", account.Username)
                .Replace("{left}", account.LikesLeft.ToString())
                );
        }
    }
}
