using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Utilities {
    public static class Robcryptions {
        public static string PasswordToGJP(string pass) {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(Encryptions.XOR(pass, "37526")));
        }

        private static
            SHA1 sha = new SHA1CryptoServiceProvider();

        // cock penis
        public static string GetChk(string special, string levelid, string like, string type, string randomstring, string accountid, string udid, string uuid) //thnks cos80
        {
            string salted = special + levelid + like + type + randomstring + accountid + udid + uuid + "ysg6pUrtjn0J";
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(salted));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash) sb.Append(b.ToString("x2"));
            string shaed = sb.ToString();
            string xored = Encryptions.XOR(shaed, "58281");
            string based = Convert.ToBase64String(Encoding.UTF8.GetBytes(xored));
            return based;
        }
    }
}
