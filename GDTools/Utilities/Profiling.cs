using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GDTools.Utilities {
    public static class Profiling {
        public static long GetObjectSize(object o) {
            using (Stream s = new MemoryStream()) {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, o);
                long size = s.Length;
                return size;
            }
        }
    }
}
