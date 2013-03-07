using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;

namespace SharePirates.TorrentNotifier
{
    class Program
    {

        private static string GetDefaultUrl()
        {
            return "http://fianbakken.com";
        }
        private static string GetTorrentLib()
        {
            return "TorrentFiles";
        }

        static void Main(string[] args)
        {
            //string url = args.Length < 1 ? GetDefaultUrl() : args[0];

            //using (var site = new SPSite(url))
            //{
            //    using (var web = site.OpenWeb())
            //    {
            //        var list = web.Lists[GetTorrentLib()];

            //        foreach (SPListItem item in list.Items)
            //        {

                        
            //        }

            //    }
            //}

            var buddy = ibuddylib.BuddyManager.Global;

            Console.Read();
        }
    }
}
