using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Geometry_Dash_LikeBot_3.Resources {
    public class Resources_Route {
        [ParameterRoute(HttpMethod.GET, "/Resources/{filename}")]
        public static async Task GetResource(HttpContext ctx) {
            var filename = ctx.Request.Url.Parameters["filename"];
            // prevent path traversal
            if (filename.IndexOfAny(Path.GetInvalidPathChars()) == -1 && !filename.Contains(".."))
                await ctx.Response.Send(await File.ReadAllBytesAsync("Resources/" + filename));
            return;
        }
    }
}
