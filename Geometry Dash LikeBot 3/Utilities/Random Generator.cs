using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        public static long RandomLong(long min, long max)
        {
            long result = Random.Next((Int32)(min >> 32), (Int32)(max >> 32));
            result = (result << 32);
            result = result | (long)Random.Next((Int32)min, (Int32)max);
            return result;
        }
        public static string RandomUUID()
        {
            return $"{RandomLong(60000000, 99999999)}";
        }
        public static string RandomUDID()
        {
            return $"S" +
                $"{RandomLong(1000000000, 9999999999)}" +
                $"{RandomLong(1000000000, 9999999999)}" +
                $"{RandomLong(1000000000, 9999999999)}" +
                $"{RandomLong(1000000000, 9999999999)}";
        }
        public static int Between(int a, int b) { return Random.Next(a, b); }
    }
}
