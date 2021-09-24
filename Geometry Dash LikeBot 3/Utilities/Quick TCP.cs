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
    }
}
