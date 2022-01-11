using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace GDTools.Core.Login {
    public class Login_Route {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [StaticRoute(HttpMethod.GET, "/")]
        public async Task LoginFormRoot(HttpContext ctx) {
            await LoginForm(ctx);
        }

        // just read it once
        public readonly byte[] LoginPage = File.ReadAllBytes("Core/Login/Login.html");
        [StaticRoute(HttpMethod.GET, "/login")]
        public async Task LoginForm(HttpContext ctx) {

            var cookies = new Dashboard.Cookie_Parser(ctx);

            if (!cookies.Parse()) {
                // if cookies dont exist send login page
                await ctx.Response.Send(LoginPage);
                return;
            }

            var sessionKey = cookies.GetSessionKey();

            var accountFromSessionKey = Database.Database.GetUserFromSessionKey(sessionKey);
            if (accountFromSessionKey != null) {
                // theres an account tied with the session key, go to dashboard
                ctx.Response.StatusCode = 302;
                ctx.Response.Headers["Location"] = "/dashboard";
                await ctx.Response.Send();
                return;
            }

            await ctx.Response.Send(LoginPage);
            return;
        }


        public class AccountCheckResponse {
            public bool IsSuccess = false;
            public bool WasError = false;
            public string Message { get; set; }
            public string SessionsKey { get; set; }
            public string Redirect { get; set; }
        }

        [StaticRoute(HttpMethod.GET, "/check")]
        public async Task CheckAccount(HttpContext ctx) {
            // should be protected

            if (!ctx.Request.Query.Elements.ContainsKey("username") ||
                !ctx.Request.Query.Elements.ContainsKey("password"))
                return;

            string ownerIDString = null;
            ctx.Request.Query.Elements.TryGetValue("ownerID", out ownerIDString);

            string username = ctx.Request.Query.Elements["username"].Trim();
            string password = ctx.Request.Query.Elements["password"].Trim();

            Account_Checker checker = new(username, password,
                ctx.Request.Source.IpAddress);

            AccountCheckResponse response = new();

            var userAgent = ctx.Request.Useragent.Trim();
            var isUserAgentBanned = Database.Database.IsUserAgentBanned(userAgent);
            if (isUserAgentBanned) {
                response.IsSuccess = false;
                response.Message = "Bot detected.";
                await ctx.Response.Send(JsonConvert.SerializeObject(response));
                return;
            }

            if (checker.IsAlreadyLoggingIn()) {
                response.IsSuccess = false;
                response.Message = "You're already attempting to log in, please wait for the attempt to finish.";
                await ctx.Response.Send(JsonConvert.SerializeObject(response));
                return;
            }

            // now check account boomlings login
            var result = await checker.Check();
            var wasSuccessAndwasRouge = result.Result;
            var wasSkid = wasSuccessAndwasRouge.wasRouge;
            var serverResponses = result.ServerResult;
            var loggedInAccountID = serverResponses.AccountID;

            if (wasSkid) {
                response.IsSuccess = false;
                response.Message = "Skid detected.";
                response.Redirect = "https://www.youtube.com/watch?v=xjS6Z8kztq8";
                await ctx.Response.Send(JsonConvert.SerializeObject(response));
                return;
            }

            response.IsSuccess = wasSuccessAndwasRouge.isSuccess;
            response.Message = serverResponses.Message;

            if (response.IsSuccess) {
                Logger.Debug($"Successful login - Username: {username} with Account ID: {result.ServerResult.AccountID}");
                // boomlings login success
                string gjp = Utilities.Robcryptions.PasswordToGJP(password);

                response.Redirect = "/dashboard";
                // response.SessionsKey = sessionsKey;

                if (Database.Database.IsExists(loggedInAccountID)) {
                    var owner = Database.Database.GetOwnerFromAccountID(loggedInAccountID);

                    Database.Database.ChangePassword(serverResponses.AccountID, password, gjp);
                    // generate new session key
                    var sessionKeyGenerationResult = owner.TryGenerateSessionKey();
                    response.IsSuccess = sessionKeyGenerationResult.IsSuccess;
                    response.Message = sessionKeyGenerationResult.IsSuccess ? "Account already exists! Updating..." : sessionKeyGenerationResult.Reason;
                    // generated key
                    response.SessionsKey = sessionKeyGenerationResult.Key;

                } else {
                    var accountInDB = Database.Database.AddAccount(serverResponses, username, password, gjp, ownerIDString);
                    var owner = Database.Database.GetOwnerFromAccountID(accountInDB.AccountID);
                    var sessionKeyGenerationResult = owner.TryGenerateSessionKey();
                    response.IsSuccess = sessionKeyGenerationResult.IsSuccess;
                    // generated key
                    response.SessionsKey = sessionKeyGenerationResult.Key;

                }

            } else if (!response.IsSuccess) {
                Logger.Debug($"Failed login - Username: {username} Password: {password}");
                // if an account with the same password exist, remove it
                if (!response.WasError) { // if the login was an actual -1 remove it
                    var account = Database.Database.GetAccountFromCredentials(username, password);
                    if (account != null) Database.Database.RemoveAccount(account);
                }
            }

            await ctx.Response.Send(JsonConvert.SerializeObject(response));
        }
    }
}
