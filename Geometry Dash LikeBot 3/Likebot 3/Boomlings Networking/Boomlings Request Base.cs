using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Boomlings_Networking
{
    public abstract class Boomlings_Request_Base
    {
        private static readonly HttpClient _client = new HttpClient();

        public string Endpoint { get; set; }

        public string Query { get; set; }

        public async Task<string> SendAsync()
        {
            var postResult = await _client.PostAsync(Endpoint, new StringContent(Query));
            string dataResult = await postResult.Content.ReadAsStringAsync();
            return dataResult;
        }

        public Boomlings_Request_Base(string endpoint, string payload)
        {
            Endpoint = endpoint; Query = payload;
        }
    }
}
