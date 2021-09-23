using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Boomlings_Networking
{
    public abstract class Boomlings_Request_Base
    {
        public string Endpoint { get; set; }

        public string Query { get; set; }

        public async Task<string> SendAsync() { 

        }

        public Boomlings_Request_Base(string endpoint, string payload)
        {
            Endpoint = endpoint; Query = payload;
        }
    }
}
