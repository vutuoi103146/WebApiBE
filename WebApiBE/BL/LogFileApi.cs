using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiBE.BL
{
    public static class LogFileApi
    {
        public static void LogFile(string str)
        {
            System.IO.StreamWriter StreamWriter1 = new System.IO.StreamWriter(@"C:\fileapi.txt", true);
            StreamWriter1.WriteLine(str);
            StreamWriter1.Close();
        }
    }
}