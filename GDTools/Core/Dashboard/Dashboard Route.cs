using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using WatsonWebserver;

namespace GDTools.Core.Dashboard {
    public class Dashboard_Route {

        public readonly string DashboardPage = File.ReadAllText("Core/Dashboard/Dashboard.html");

        [StaticRoute(HttpMethod.GET, "/dashboard")]
        public async Task EnterDashboard(HttpContext ctx) {
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

            await ctx.Response.Send(DashboardPage
                .Replace("{username}", account.Username)
                .Replace("{left}", account.Tier.LikesLeft.ToString())
                .Replace("{used_memory}", Utilities.Me_as_Process.FormatBytes(Utilities.Me_as_Process.MemoryUsage()))
                );
            return;
        }
    }
}
