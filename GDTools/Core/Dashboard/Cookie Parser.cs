using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace GDTools.Core.Dashboard {
    public class Cookie_Parser {
        private Dictionary<string, string> cookiesMap = new();
        private HttpContext httpContext;

        public Cookie_Parser(HttpContext ctx) {
            httpContext = ctx;
        }
        public bool Parse() {
            string cookies;
            var cookiesFound = httpContext.Request.Headers.TryGetValue("Cookie", out cookies);

            if (!cookiesFound) {
                // try lowercase
                cookiesFound = httpContext.Request.Headers.TryGetValue("cookie", out cookies);
                if (!cookiesFound)
                    // if still not found rip
                    return false;
            }


            cookiesMap = Utilities.Quick_TCP.ParseCookie(cookies);
            if (cookiesMap == null) {
                return false;
            }
            return true;
        }

        public async Task MoveToLogin(HttpContext ctx) {
            ctx.Response.StatusCode = 302;
            ctx.Response.Headers["Location"] = "/login";
            await ctx.Response.Send();
            return;
        }

        public string GetSessionKey() {
            string sessionKey;
            var sessionKeyFound = cookiesMap.TryGetValue("SessionsKey", out sessionKey);

            if (!sessionKeyFound) {
                return null;
            }
            // session keys are supposed to be fully lowercased
            if (sessionKey.Any(char.IsUpper)) {
                return null;
            }

            return sessionKey;
        }
    }
}
