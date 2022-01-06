using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Scenes {
    public abstract class Scene {
        public char Key { get; set; }

        public string Name { get; set; }

        public abstract Task StartAsync();

        public Scene(char key, string name) {
            Key = key;
            Name = name;
        }
    }
}
