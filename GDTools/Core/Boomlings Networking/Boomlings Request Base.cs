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

        public async Task<string> SendAsync(ProxyType proxyToUse) {
            CustomWebClient proxiedClient = null;
            switch (proxyToUse) {
                case ProxyType.PaidProxy:
                    proxiedClient = Proxies.NextPaidProxy();
                    break;
                case ProxyType.ScrapedProxy:
                    proxiedClient = Proxies.NextScrapedProxy();
                    break;
            }


            try {
                var postResult = await proxiedClient.UploadStringTaskAsync(Endpoint, Query);
                proxiedClient.Dispose();
                return postResult;
            } catch { }
            return null;
        }

        public Boomlings_Request_Base(string endpoint, string payload) {
            Endpoint = $"{Constants.BoomlingsHost}{endpoint}"; Query = payload;
        }
    }
}
