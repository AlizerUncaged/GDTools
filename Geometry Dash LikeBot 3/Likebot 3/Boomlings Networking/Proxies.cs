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

    // proxies from webshare.io
    public static class Proxies
    {
        private static int currentProxyIndex = 0;

        public static List<Proxy> ProxyList = new();
        public static async Task<bool> InitializeProxies()
        {

        }
    }
}
