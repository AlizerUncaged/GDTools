using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Utilities
{
    public static class Quick_TCP
    {
        public static async Task<string> ReadURL(string url)
        {
            return await Task.Run(() =>
            {
                using (WebClient client = new WebClient())
                {
                    return client.DownloadString(url);
                }
            });
        }
        public static Dictionary<string, string> ParseCookie(string cookie) {
            try {
                Dictionary<string, string> cookies = new();

                if (string.IsNullOrWhiteSpace(cookie))
                    return cookies;

                if (cookie.Contains(';')) {
                    var keys = cookie.Split(';');
                    foreach (var key in keys) {
                        var keyandval = key.Split('=');
                        cookies.Add(keyandval[0], keyandval[1]);
                    }
                } else {
                    var keyandval = cookie.Split('=');
                    cookies.Add(keyandval[0], keyandval[1]);
                }
                return cookies;
            } catch { return null; }
        }
    }
}
