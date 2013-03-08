using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using SharePirates.Common;

namespace SharePirates.TorrentDownloader
{
    /// <summary>
    /// Program to initiate downloading of approved torrents
    /// </summary>
    class Program
    {

       
        static void Main(string[] args)
        {
            string url = args.Length < 1 ? SiteDefinition.GetDefaultUrl() : args[0];

            using (var site = new SPSite(url))
            {
                using (var web = site.OpenWeb())
                {
                    var list = web.Lists[SiteDefinition.GetTorrentLib()];

                    foreach (SPListItem item in list.Items)
                    {
                                            
                        foreach (String attachment in  item.Attachments)
                        {
                            var attachmentAbsoluteUrl = item.Attachments.UrlPrefix + attachment;
                            var file = web.GetFile(attachmentAbsoluteUrl);

                            
//                            file.SaveBinary();
                            Console.WriteLine("--File = "+file.Name);
                        }
                    }

                }
            }
            Console.WriteLine("Ready..");
            Console.ReadKey();
        }
    }

    
}
