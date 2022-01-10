using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace GDTools.Core.Watson_Server {
    public class Watson {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Server s;

        public Watson() {
            // When debugging (or building as Debug), GDL-3 will be at 127.0.0.1:8080 for testing reasons.
#if DEBUG
            Constants.IP = "127.0.0.1";
            Constants.Port = 8080;
#endif

            s = new Server(Constants.IP, Constants.Port, false, DefaultRoute);
            s.Settings.AccessControl.Mode = AccessControlMode.DefaultPermit;
            s.Settings.IO.MaxRequests = 4098;

            s.Events.Logger = LogReceived;
            s.Events.RequestReceived += RequestReceived;
            s.Events.ExceptionEncountered += ErrorOccured;
        }

        public async Task StartAsync() {
            try {
                Logger.Debug($"Running web server...");

                _ = s.StartAsync();

                Logger.Info($"Server running at {Constants.IP}:{Constants.Port}.");
            } catch (Exception ex) {
                Logger.Fatal(ex);
            }

        }
        public void Stop() {
            Logger.Debug($"Stopping webserver...");
            if (s != null) {
                s.Stop();
                s.Dispose();
            }
            Logger.Info($"Server stopped...");
        }

        private void RequestReceived(object sender, RequestEventArgs e) {

        }

        private void LogReceived(string message) {
            const string watermark = "[Watson] ";
            if (message.StartsWith(watermark)) {
                message = message.Remove(0, watermark.Length);
                message = message.First().ToString().ToUpper() + message.Substring(1);
            }
            Logger.Debug($"{message}");

        }

        private void ErrorOccured(object sender, ExceptionEventArgs e) {
            Logger.Info($"Error occured: {e.Exception}");
        }

        // 404
        static async Task DefaultRoute(HttpContext ctx) {
            ctx.Response.StatusCode = 302;
            ctx.Response.Headers["Location"] = "/";
            await ctx.Response.Send();
            return;
        }

    }
}
