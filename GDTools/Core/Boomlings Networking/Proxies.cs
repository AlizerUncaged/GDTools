using log4net;
using log4net.Config;
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
using System.Diagnostics;
using System.Threading;

namespace GDTools.Core.Boomlings_Networking {
    // socks only 
    public enum ProxyType {
        PaidProxy, ScrapedProxy
    }
    public class Proxy {
        public Proxy(ProxyType proxyType) {
            ProxyType = proxyType;
        }

        public ProxyType ProxyType;
        public string IP;
        public int Port;
        public string Username, Password;
        public override string ToString() {
            return $"{IP}:{Port}";
        }

        public static bool operator ==(Proxy left, Proxy right) {
            return (left.IP == right.IP) && (left.Port == right.Port);
        }

        public static bool operator !=(Proxy left, Proxy right) {
            return (left.IP != right.IP) && (left.Port != right.Port);
        }
    }

    public class CustomWebClient : WebClient {
        protected override WebRequest GetWebRequest(Uri uri) {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 15 * 1000; // 15 seconds
            return w;
        }
    }

    public static class Proxies {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string[] scrapedProxySources = new string[] {
            "https://raw.githubusercontent.com/ShiftyTR/Proxy-List/master/socks5.txt",
            "https://api.proxyscrape.com/v2/?request=getproxies&protocol=socks5&timeout=10000&country=all&simplified=true",
            "https://raw.githubusercontent.com/TheSpeedX/PROXY-List/master/socks5.txt",
            "https://raw.githubusercontent.com/jetkai/proxy-list/main/online-proxies/txt/proxies-socks5.txt",
            "https://raw.githubusercontent.com/monosans/proxy-list/main/proxies/socks5.txt",
            "https://raw.githubusercontent.com/hookzof/socks5_list/master/proxy.txt"
        };

        public static List<Proxy> PaidProxyList = new();
        public static List<Proxy> ScrapedProxies = new();

        // public static List<ProxiedHttpClient> PaidProxiedHttpClients = new();
        // public static List<ProxiedHttpClient> ScrapedProxiedHttpClients = new();
        public static async Task<bool> InitializeProxies() {
            Logger.Debug("Loading paid proxies...");
            string proxies = await Quick_TCP.ReadURL(Constants.ProxyList);
            var lines = proxies.Split('\n').Where(x => x.Contains(":"));
            foreach (var line in lines) {
                var datas = line.Split(':');
                PaidProxyList.Add(new Proxy(ProxyType.PaidProxy) {
                    IP = datas[0],
                    Port = int.Parse(datas[1]),
                    Username = datas[2],
                    Password = datas[3]
                });
            }

            // shuffle
            PaidProxyList.Shuffle();


            Logger.Info($"Loaded {PaidProxyList.Count()} paid proxies...");

            await gatherScrapedProxies();
            // start periodically checking of new proxies
            _ = Task.Factory.StartNew(async () => {
                var gatherFreeProxiesScheduler = new PeriodicTimer(TimeSpan.FromHours(2));
                while (await gatherFreeProxiesScheduler.WaitForNextTickAsync()) {
                    await gatherScrapedProxies();
                }
            });
            // generate scraped proxies
            return true;
        }

        private static async Task gatherScrapedProxies() {
            Logger.Debug($"Gathering scraped proxies from {scrapedProxySources.Count()} sources...");

            List<Proxy> refreshingProxies = new();
            foreach (var source in scrapedProxySources) {
                var list = await scrapedProxies(source);
                if (list != null) {
                    refreshingProxies.AddRange(list);
                }
            }

            ScrapedProxies.Clear();
            ScrapedProxies.AddRange(refreshingProxies);

            ScrapedProxies = ScrapedProxies.Distinct().ToList();
            ScrapedProxies.Shuffle();

            // generate http clients based on proxy
            // foreach (var proxy in ScrapedProxies) {
            //     var generatedClient = generateHttpClient(proxy);
            //     ScrapedProxiedHttpClients.Add(generatedClient);
            // }


            Logger.Debug($"Finished gathering free proxies {ScrapedProxies.Count} total...");


        }
        private static async Task<IEnumerable<Proxy>> scrapedProxies(string source) {
            var proxies = await Utilities.Quick_TCP.ReadURL(source);
            if (string.IsNullOrWhiteSpace(proxies)) return null;

            List<Proxy> proxiesObject = new();
            var splitted = proxies.Split(
                            new string[] { "\r\n", "\r", "\n" },
                            StringSplitOptions.None
                        );

            foreach (var str in splitted) {
                if (str.Contains(":")) {
                    var ipport = str.Split(':');
                    proxiesObject.Add(new Proxy(ProxyType.ScrapedProxy) { IP = ipport[0].Trim(), Port = int.Parse(ipport[1].Trim()) });
                }
            }
            return proxiesObject;
        }
        private static
                readonly MediaTypeWithQualityHeaderValue AcceptAll = new MediaTypeWithQualityHeaderValue("*/*");

        // generate an http client for each proxy to save hardware resources because each HttpClient generates
        // its own socket for some reason and i dont want to create a new instance of HttpClient each request
        // when we can generate for each proxies.
        private static CustomWebClient generateWebClient(Proxy proxy) {
            var wproxy = new WebProxy();
            wproxy.Address = new Uri($"socks5://{proxy.IP}:{proxy.Port}");
            wproxy.Credentials = new NetworkCredential(proxy.Username, proxy.Password); //Used to set Proxy logins. 

            var webClient = new CustomWebClient { Proxy = wproxy };
            webClient.Headers["User-Agent"] = string.Empty;

            webClient.Headers["Accept"] = "*/*";
            webClient.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            return webClient;

        }

        private static int currentPaidProxyIndex = -1; // initial value
        public static CustomWebClient NextPaidProxy() {
            currentPaidProxyIndex++;
            currentPaidProxyIndex = currentPaidProxyIndex >= PaidProxyList.Count() ? 0 : currentPaidProxyIndex;
            if (currentPaidProxyIndex <= 0) {
                PaidProxyList.Shuffle();
            }
            var proxiedHttpClient = PaidProxyList[currentPaidProxyIndex];
            var webclient = generateWebClient(proxiedHttpClient);
            return webclient;
        }

        private static int currentScrapedProxyIndex = -1; // initial value
        public static CustomWebClient NextScrapedProxy() {
            // generate HttpClient per proxy?
            currentScrapedProxyIndex++;
            currentScrapedProxyIndex = currentScrapedProxyIndex >= ScrapedProxies.Count() ? 0 : currentScrapedProxyIndex;
            if (currentScrapedProxyIndex <= 0) {
                ScrapedProxies.Shuffle();
            }
            var currentProxy = ScrapedProxies[currentScrapedProxyIndex];
            var generatedClient = generateWebClient(currentProxy);
            return generatedClient;
            // generate httpclient already
            ////currentScrapedProxyIndex++;
            ////currentScrapedProxyIndex = currentScrapedProxyIndex >= ScrapedProxiedHttpClients.Count() ? 0 : currentScrapedProxyIndex;
            ////if (currentScrapedProxyIndex <= 0) {
            ////    ScrapedProxiedHttpClients.Shuffle();
            ////}
            ////var proxiedHttpClient = ScrapedProxiedHttpClients[currentScrapedProxyIndex];
            ////Debug.WriteLine($"Took proxy: {proxiedHttpClient.Proxy}");
            ////return proxiedHttpClient;
        }
    }
}
