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
        public async Task MoveToLogin(HttpContext ctx) {

            ctx.Response.StatusCode = 302;
            ctx.Response.Headers["Location"] = "/login";
            await ctx.Response.Send();
            return;
        }
        // shouldve cookie already
        [StaticRoute(HttpMethod.GET, "/dashboard")]
        public async Task EnterDashboard(HttpContext ctx)
        {
            // TODO
            string cookies;
            var cookiesFound = ctx.Request.Headers.TryGetValue("Cookie", out cookies);

            if (!cookiesFound)
                await MoveToLogin(ctx);
            

            var keysAndCookies = Utilities.Quick_TCP.ParseCookie(cookies);
            string sessionKey;
            var sessionKeyFound = keysAndCookies.TryGetValue("SessionsKey", out sessionKey);

            if (!sessionKeyFound)
                await MoveToLogin(ctx);

            // session key found!
            var account = Database.Data.GetAccountFromSessionKey(sessionKey);

            await ctx.Response.Send(File.ReadAllBytes("Likebot 3/Dashboard/Dashboard.html"));

        }
    }
}
