using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Utilities
{
    public static class Random_Generator
    {
        private static Random Random = new();
        public static string RandomString(int length)
        {
            const string chars = "123456789abcdefghjklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
