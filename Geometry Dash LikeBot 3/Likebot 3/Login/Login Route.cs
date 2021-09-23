using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Login
{
    public class Login_Route
    {
        [StaticRoute(HttpMethod.GET, "/")]
        public async Task LoginFormRoot(HttpContext ctx)
        {
            await LoginForm(ctx);
        }

        [StaticRoute(HttpMethod.GET, "/login")]
        public async Task LoginForm(HttpContext ctx)
        {
            await ctx.Response.Send(File.ReadAllBytes("Likebot 3/Login/Login.html"));
        }

        [StaticRoute(HttpMethod.GET, "/check")]
        public async Task CheckAccount(HttpContext ctx)
        {

            await ctx.Response.Send(Utilities.Random_Generator.RandomString(16));
        }
    }
}
