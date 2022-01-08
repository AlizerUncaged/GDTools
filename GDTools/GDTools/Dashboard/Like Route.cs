using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace GDTools.Likebot_3.Dashboard {
    public class Like_Route {

        [StaticRoute(HttpMethod.GET, "/actions/like")]
        public async Task LikeItem() { 

        }
    }
}
