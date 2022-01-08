using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;

namespace GDTools.Resources {
    public class Resources_Route {
        [ParameterRoute(HttpMethod.GET, "/resources/{filename}")]
        public async Task GetResource(HttpContext ctx) {


            var filename = ctx.Request.Url.Parameters["filename"];
            // prevent path traversal
            if (filename.IndexOfAny(Path.GetInvalidPathChars()) == -1 && !filename.Contains(".."))
                await ctx.Response.Send(await File.ReadAllBytesAsync("Resources/" + filename));
            return;
        }
    }
}
