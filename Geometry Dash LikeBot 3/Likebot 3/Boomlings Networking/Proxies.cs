using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Boomlings_Networking
{
    // socks only duh
    public class Proxy
    {
        public string IP;
        public int Port;
        public string Username, Password;
    }

    public static class Proxies
    {
        private static int currentProxyIndex = -1; // initial value

        public static List<Proxy> ProxyList = new();
        public static async Task<bool> InitializeProxies()
        {
            string proxies = await Utilities.Quick_TCP.ReadURL(Constants.ProxyList);
            var lines = proxies.Split('\n').Where(x => x.Contains(":"));
            foreach (var line in lines)
            {
                var datas = line.Split(':');
                ProxyList.Add(new Proxy
                {
                    IP = datas[0],
                    Port = int.Parse(datas[1]),
                    Username = datas[2],
                    Password = datas[3]
                });
            }

            // shuffle
            ProxyList = ProxyList.OrderBy(x => Utilities.Random_Generator.Random.Next()).ToList();
            return true;
        }

        public static Proxy NextProxy()
        {
            currentProxyIndex++;

            if (currentProxyIndex > ProxyList.Count) currentProxyIndex = 0;
            return ProxyList[currentProxyIndex];
        }
    }
}
