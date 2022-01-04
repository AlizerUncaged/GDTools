using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3
{
    public static class Constants
    {
        // ip and port to run the web application
        public static string IP = 
            "localhost";
        public static int Port = 
            80;

        public const string BoomlingsHost = 
            "http://172.104.26.87/";

        public const string DatabaseFile = 
            "Database.json";

        // proxy list from the internet with the format of [host]:[port]:[username]:[password], should be socksv5 or v4
        // if you want to use http, modify the code yourself
        public const string ProxyList =
            "https://proxy.webshare.io/proxy/list/download/vqdlkdoslzscpxmkiibspcnjbjtfqqpjhowjekto/-/socks/username/direct/";
    }
}
