﻿using log4net;
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

    public class ProxiedHttpClient : HttpClient {
        public ProxiedHttpClient(HttpClientHandler handler, bool reuse) : base(handler, reuse) { }

        public Proxy Proxy;
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

        public static List<ProxiedHttpClient> PaidProxiedHttpClients = new();
        public static List<ProxiedHttpClient> ScrapedProxiedHttpClients = new();
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

            // generate http clients based on proxy
            foreach (var proxy in PaidProxyList) {
                var generatedClient = generateHttpClient(proxy);
                PaidProxiedHttpClients.Add(generatedClient);
            }

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

            foreach (var source in scrapedProxySources) {
                var list = await scrapedProxies(source);
                if (list != null) {
                    ScrapedProxies.AddRange(list);
                }
            }
            ScrapedProxies = ScrapedProxies.Distinct().ToList();
            ScrapedProxies.Shuffle();
            // generate http clients based on proxy
            foreach (var proxy in ScrapedProxies) {
                var generatedClient = generateHttpClient(proxy);
                ScrapedProxiedHttpClients.Add(generatedClient);
            }


            Logger.Debug($"Finished gathering free proxies {ScrapedProxiedHttpClients.Count} total...");


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
        private static ProxiedHttpClient generateHttpClient(Proxy proxy) {
            HttpToSocks5Proxy socks5 = null;

            if (proxy.Username == null && proxy.Password == null)
                socks5 = new HttpToSocks5Proxy(proxy.IP, proxy.Port);

            else
                socks5 = new HttpToSocks5Proxy(proxy.IP, proxy.Port, proxy.Username, proxy.Password);
            socks5.ResolveHostnamesLocally = true;

            var handler = new HttpClientHandler { Proxy = socks5 };
            var httpClient = new ProxiedHttpClient(handler, true);

            // timeout of clients in seconds
            const int timeout = 20;

            httpClient.Timeout = TimeSpan.FromSeconds(timeout);
            httpClient.DefaultRequestHeaders.Accept.Add(AcceptAll);
            httpClient.DefaultRequestVersion = HttpVersion.Version11;
            httpClient.Proxy = proxy;
            return httpClient;
        }

        private static int currentPaidProxyIndex = -1; // initial value
        public static HttpClient NextPaidProxy() {
            currentPaidProxyIndex++;
            currentPaidProxyIndex = currentPaidProxyIndex >= PaidProxiedHttpClients.Count() ? 0 : currentPaidProxyIndex;
            if (currentPaidProxyIndex <= 0) {
                PaidProxiedHttpClients.Shuffle();
            }
            var proxiedHttpClient = PaidProxiedHttpClients[currentPaidProxyIndex];
            Debug.WriteLine($"Took proxy: {proxiedHttpClient.Proxy}");
            return proxiedHttpClient;
        }

        private static int currentScrapedProxyIndex = -1; // initial value
        public static HttpClient NextScrapedProxy() {
            currentScrapedProxyIndex++;
            currentScrapedProxyIndex = currentScrapedProxyIndex >= ScrapedProxiedHttpClients.Count() ? 0 : currentScrapedProxyIndex;
            if (currentScrapedProxyIndex <= 0) {
                ScrapedProxiedHttpClients.Shuffle();
            }
            var proxiedHttpClient = ScrapedProxiedHttpClients[currentScrapedProxyIndex];
            Debug.WriteLine($"Took proxy: {proxiedHttpClient.Proxy}");
            return proxiedHttpClient;
        }
    }
}