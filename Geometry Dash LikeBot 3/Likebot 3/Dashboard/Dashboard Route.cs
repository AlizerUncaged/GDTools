using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Geometry_Dash_LikeBot_3.Likebot_3.Dashboard
{
    public class Dashboard_Route
    {
        // shouldve cookie already
        [StaticRoute(HttpMethod.GET, "/dashboard")]
        public async Task EnterDashboard(HttpContext ctx)
        {


        }
    }
}
