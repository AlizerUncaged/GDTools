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
        public const string IP = "127.0.0.1";
        public const int Port = 9000;

        public const string BoomlingsHost = 
            "http://www.boomlings.com/";

        public const string DatabaseFile = 
            "Database.json";

        // proxy list from the internet with the format of [host]:[port]:[username]:[password], should be socksv5 or v4
        // if you want to use http, modify the code yourself
        public const string ProxyList = 
            "https://proxy.webshare.io/proxy/list/download/ecxuorgiyfjngzvcrvnpsogurdaynmhvmlikyyia/-/socks/username/direct/";
    }
}
