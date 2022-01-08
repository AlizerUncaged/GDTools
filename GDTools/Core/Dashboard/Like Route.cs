using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace GDTools.Core.Dashboard {
    public class Like_Route {

        // returns 
        [StaticRoute(HttpMethod.POST, "/actions/like")]
        public async Task LikeItem(HttpContext ctx) {
            var cookies = new Cookie_Parser(ctx);

            if (!cookies.Parse()) {
                await cookies.MoveToLogin(ctx);
                return;
            }

            var sessionKey = cookies.GetSessionKey();
            Console.WriteLine($"Session key {sessionKey}");
            // session key found!
            // get account from db which is Valid and has the key
            var account = Database.Database.GetUserFromSessionKey(sessionKey);
            // else return to login screen
            if (account == null) {
                await cookies.MoveToLogin(ctx);
                return;
            }

            // parse like request
        }
    }
}
