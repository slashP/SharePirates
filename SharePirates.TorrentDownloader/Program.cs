using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

                        var downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Torrents"); 
                        foreach (String attachment in item.Attachments)
                        {
                            var attachmentAbsoluteUrl = item.Attachments.UrlPrefix + attachment;
                            var file = web.GetFile(attachmentAbsoluteUrl);

                            if (!Directory.Exists(downloadFolder))
                            {
                                Directory.CreateDirectory(downloadFolder);
                            }
                            var data = file.OpenBinary();
                            var torrentName = (string)item["TorrentName"];
                            var pathTorrentFile = Path.Combine(downloadFolder, torrentName + ".torrent");
                            File.WriteAllBytes(pathTorrentFile, data);
                            try
                            {
                                Process.Start(@"C:\Users\spdbadmin\AppData\Roaming\uTorrent\utorrent.exe", pathTorrentFile);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Ok, so that didn't work quite like we wanted.");
                            }
                            
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
