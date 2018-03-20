using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CullCat
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                if(CheckForInternetConnection() == false)
                {
                    foreach (var process in Process.GetProcessesByName("nc"))
                    {
                        process.Kill();
                    }
                }
                Thread.Sleep(5000);
            }
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.matek.org/nettest.html"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
