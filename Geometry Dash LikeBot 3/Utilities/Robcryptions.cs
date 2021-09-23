using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Utilities
{
    public static class Robcryptions
    {
        public static string PasswordToGJP(string pass)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(Encryptions.XOR(pass, "37526")));
        }
    }
}
