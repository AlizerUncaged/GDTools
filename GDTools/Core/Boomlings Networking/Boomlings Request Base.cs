using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using MihaZupan;

namespace GDTools.Core.Boomlings_Networking {
    public abstract class Boomlings_Request_Base {

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string Endpoint { get; set; }

        public string Query { get; set; }

        private static
                readonly MediaTypeHeaderValue ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        public async Task<string> SendAsync(ProxyType proxyToUse) {
            HttpClient proxiedClient = null;
            switch (proxyToUse) {
                case ProxyType.PaidProxy:
                    proxiedClient = Proxies.NextPaidProxy();
                    break;
                case ProxyType.ScrapedProxy:
                    proxiedClient = Proxies.NextScrapedProxy();
                    break;
            }

            var content = new StringContent(Query);

            content.Headers.ContentType = ContentType;

            try {
                string dataResult = null;
                using (var postResult = await proxiedClient.PostAsync(Endpoint, content)) {
                    dataResult = await postResult.Content.ReadAsStringAsync();
                }
                return dataResult;
            } catch (TaskCanceledException) {
                // was a timeout, ignore
            } catch (Exception ex) {
                // holy shit its a real exception
                Logger.Fatal(ex);
            }

            return null;
        }

        public Boomlings_Request_Base(string endpoint, string payload) {
            Endpoint = $"{Constants.BoomlingsHost}{endpoint}"; Query = payload;
        }
    }
}
