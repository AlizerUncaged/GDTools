﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using MihaZupan;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Boomlings_Networking
{
    public abstract class Boomlings_Request_Base {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly HttpClient _client = new HttpClient();

        public string Endpoint { get; set; }

        public string Query { get; set; }

        // declare this shit so it wont have to be reinitializwed
        private static
                readonly MediaTypeWithQualityHeaderValue AcceptAll = new MediaTypeWithQualityHeaderValue("*/*");
        private static
                readonly MediaTypeHeaderValue ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        public async Task<string> SendAsync()
        {
            try
            {
                var proxy = Proxies.NextProxy();

                var socks5 = new HttpToSocks5Proxy(proxy.IP, proxy.Port, proxy.Username, proxy.Password);
                socks5.ResolveHostnamesLocally = true;

                var handler = new HttpClientHandler { Proxy = socks5 };
                HttpClient httpClient = new HttpClient(handler, true);
                httpClient.Timeout = TimeSpan.FromSeconds(5);

                httpClient.DefaultRequestHeaders.Accept.Add(AcceptAll);
                httpClient.DefaultRequestVersion = HttpVersion.Version11;

                var content = new StringContent(Query);
                content.Headers.ContentType = ContentType;

                var postResult = await httpClient.PostAsync(Endpoint, content);
                string dataResult = await postResult.Content.ReadAsStringAsync();

                return dataResult;
            }
            catch (Exception ex)
            {
                Logger.Info($"Error Sending Request: {ex}");
            }
            return null;
        }

        public Boomlings_Request_Base(string endpoint, string payload)
        {
            Endpoint = endpoint; Query = payload;
        }
    }
}
