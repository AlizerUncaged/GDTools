using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Database {
    public class Crawler_User_Agents {
        [JsonProperty("pattern")]
        public string Pattern { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("instances")]
        public List<string> Instances { get; set; }

        [JsonProperty("addition_date")]
        public string AdditionDate { get; set; }

        [JsonProperty("depends_on")]
        public List<string> DependsOn { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
