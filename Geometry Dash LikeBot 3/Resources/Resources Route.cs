using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Geometry_Dash_LikeBot_3.Resources
{
    public class Resources_Route
    {
        [ParameterRoute(HttpMethod.GET, "/Resources/{filename}")]
        public static async Task GetResource(HttpContext ctx)
        {
            await ctx.Response.Send(await File.ReadAllBytesAsync("Resources/"+ctx.Request.Url.Parameters["filename"]));
        }
    }
}
