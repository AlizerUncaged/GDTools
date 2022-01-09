using log4net;
using log4net.Config;
using MihaZupan;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using GDTools.Utilities;
namespace GDTools.Core.Boomlings_Networking {
    // socks only 
    public class Proxy {
        public string IP;
        public int Port;
        public string Username, Password;
    }

    public static class Proxies {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static int currentProxyIndex = -1; // initial value

        public static List<Proxy> ProxyList = new();

        public static List<HttpClient> ProxiedHttpClients = new();
        public static async Task<bool> InitializeProxies() {
            Logger.Debug("Loading proxies...");
            string proxies = await Utilities.Quick_TCP.ReadURL(Constants.ProxyList);
            var lines = proxies.Split('\n').Where(x => x.Contains(":"));
            foreach (var line in lines) {
                var datas = line.Split(':');
                ProxyList.Add(new Proxy {
                    IP = datas[0],
                    Port = int.Parse(datas[1]),
                    Username = datas[2],
                    Password = datas[3]
                });
            }

            // shuffle
            ProxyList.Shuffle();
            // generate http clients based on proxy
            foreach (var proxy in ProxyList) {
                var generatedClient = generateHttpClient(proxy);
                ProxiedHttpClients.Add(generatedClient);
            }

            Logger.Info($"Loaded {ProxyList.Count()} proxies...");
            return true;
        }

        private static
                readonly MediaTypeWithQualityHeaderValue AcceptAll = new MediaTypeWithQualityHeaderValue("*/*");

        // generate an http client for each proxy to save hardware resources because each HttpClient generates
        // its own socket for some reason and i dont want to create a new instance of HttpClient each request
        // when we can generate for each proxies.
        private static HttpClient generateHttpClient(Proxy proxy) {
            var socks5 = new HttpToSocks5Proxy(proxy.IP, proxy.Port, proxy.Username, proxy.Password);
            socks5.ResolveHostnamesLocally = true;

            var handler = new HttpClientHandler { Proxy = socks5 };
            var httpClient = new HttpClient(handler, true);

            // timeout of clients in seconds
            const int timeout = 40;

            httpClient.Timeout = TimeSpan.FromSeconds(timeout);
            httpClient.DefaultRequestHeaders.Accept.Add(AcceptAll);
            httpClient.DefaultRequestVersion = HttpVersion.Version11;

            return httpClient;
        }

        public static HttpClient NextProxy() {
            currentProxyIndex++;
            currentProxyIndex = currentProxyIndex >= ProxiedHttpClients.Count() ? 0 : currentProxyIndex;
            return ProxiedHttpClients[currentProxyIndex];
        }
    }
}
