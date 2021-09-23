using Newtonsoft.Json;
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


        public class AccountCheckResponse
        {
            public bool IsSuccess = false;
            public string Message { get; set; }
            public string SessionsKey { get; set; }
        }

        [StaticRoute(HttpMethod.GET, "/check")]
        public async Task CheckAccount(HttpContext ctx)
        {
            Account_Checker checker = new(ctx.Request.Query.Elements["username"],
                ctx.Request.Query.Elements["password"],
                ctx.Request.Source.IpAddress);

            AccountCheckResponse response = new();

            if (checker.IsAlreadyLoggingIn())
            {
                response.IsSuccess = false;
                response.Message = "You're already attempting to log in, please wait for the attempt to finish.";
                await ctx.Response.Send(JsonConvert.SerializeObject(response));
                return;
            }

            // now check account fr
            var result = checker.Check();
            response.IsSuccess = result.Item1;
            if (response.IsSuccess) response.Message = "Logged in successfully!";
            else if (!response.IsSuccess) response.Message = "Failed logging in :(";

            // save account id and player id to database which is
            // just a fucking json file to be loaded every start

            await ctx.Response.Send(JsonConvert.SerializeObject(response));
        }
    }
}
