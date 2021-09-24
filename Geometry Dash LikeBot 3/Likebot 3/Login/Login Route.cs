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
            string username = ctx.Request.Query.Elements["username"].Trim();
            string password = ctx.Request.Query.Elements["password"].Trim();
            Console.WriteLine($"Username: {username} Password: {password}");

            Account_Checker checker = new(username, password,
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
            var result = await checker.Check();
            response.IsSuccess = result.Item1;
            var serverResponses = result.Item2;
            response.Message = serverResponses.Message;

            if (response.IsSuccess)
            {
                string sessionsKey = Utilities.Random_Generator.RandomString(64);
                string gjp = Utilities.Robcryptions.PasswordToGJP(password);

                response.SessionsKey = sessionsKey;

                if (Database.Data.IsExists(serverResponses.AccountID))
                {
                    // always change sessions key every log in to prevent multi device login
                    response.Message = "Account already exists! Updating...";
                    Database.Data.ChangePassword(serverResponses.AccountID, password, gjp, sessionsKey);
                }
                else
                    Database.Data.AddAccount(serverResponses, username, password, gjp, sessionsKey);

            }
            else if (!response.IsSuccess)
            {
                // if account with password exist, remove it
                var account = Database.Data.GetAccountFromCredentials(username, password);
                if (account != null)
                    Database.Data.RemoveAccount(account);

            }


            await ctx.Response.Send(JsonConvert.SerializeObject(response));
        }
    }
}
