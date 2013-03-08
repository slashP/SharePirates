using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.SharePoint;
using SharePirates.Common;

namespace SharePirates.TorrentDownloader
{
    /// <summary>
    /// Program to initiate downloading of approved torrents
    /// </summary>
    class Program
    {
        private static IHubProxy _torrentHub;
        private const string TorrentDownloadPath = @"C:\Users\spdbadmin\Documents\Torrents\TorrentDataDownloading";
        private const string TorrentAddedPath = @"C:\Users\spdbadmin\Documents\Torrents\TorrentsFinished";
        private const string TorrentDeletedPath = @"C:\Users\spdbadmin\Documents\Torrents\TorrentsDownloading";

        static void Main(string[] args)
        {
            SetupSignalR();
            SetupActivityMonitors();
            string url = args.Length < 1 ? SiteDefinition.GetDefaultUrl() : args[0];
            using (var site = new SPSite(url))
            {
                using (var web = site.OpenWeb())
                {
                    var list = web.Lists[SiteDefinition.GetTorrentLib()];
                    while (true)
                    {
                        foreach (var item in list.Items.Cast<SPListItem>().Where(x => (string)x["FileStatus"] == "Not started" && (string)x["Approved"] == "Approved"))
                        {
                            const string downloadFolder = TorrentDeletedPath;
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
                                Console.WriteLine("--File = " + file.Name);
                            }
                        }
                        Thread.Sleep(60000);
                    }
                }
            }
            Console.WriteLine("Ready..");
            Console.ReadKey();
        }

        private static void SetupSignalR()
        {
            var hubConnection = new HubConnection("http://ciberpirates.apphb.com/");
            // Create a proxy to the chat service
            _torrentHub = hubConnection.CreateHubProxy("torrentHub");
            // Start the connection
            hubConnection.Start().Wait();
        }

        private static void SetupActivityMonitors()
        {
            var torrentDeletedByUtorrentActivityMonitor = new ActivityFileMonitor();
            torrentDeletedByUtorrentActivityMonitor.StartActivityMonitoring(TorrentDeletedPath);
            torrentDeletedByUtorrentActivityMonitor.RaiseFileDeletedEvent += (sender, eventArgs) => FileDeletedByUtorrent(eventArgs);
            var torrentAddedByUtorrentActivityMonitor = new ActivityFileMonitor();
            torrentAddedByUtorrentActivityMonitor.StartActivityMonitoring(TorrentAddedPath);
            torrentAddedByUtorrentActivityMonitor.RaiseFileAddedEvent += (sender, eventArgs) => FileAddedByUtorrent(eventArgs);
            var fileChangedActivityMonitor = new ActivityFileMonitor();
            fileChangedActivityMonitor.StartActivityMonitoring(TorrentDownloadPath);
            fileChangedActivityMonitor.RaiseFileChangedEvent += (sender, eventArgs) => FileChanged(eventArgs);
        }

        private static void FileChanged(FileSystemEventArgs eventArgs)
        {
            var file = Path.GetDirectoryName(eventArgs.FullPath);
            // TODO: report progress
        }

        private static void FileAddedByUtorrent(FileSystemEventArgs eventArgs)
        {
            var folderName = Path.GetDirectoryName(eventArgs.FullPath);
            // Torrent is finished
            // GetListItemByFolderName - foldername == torrentName?
            _torrentHub.Invoke("TorrentDownloaded", folderName);
            SetListItemStatus(folderName, "Downloaded");
        }

        private static void FileDeletedByUtorrent(FileSystemEventArgs eventArgs)
        {
            // Torrent is starting downloading
            var folderName = Path.GetDirectoryName(eventArgs.FullPath);
            _torrentHub.Invoke("TorrentDownloading", folderName);
            SetListItemStatus(folderName, "Downloading");
        }

        private static void SetListItemStatus(string folderName, string fileStatus)
        {
            string url = SiteDefinition.GetDefaultUrl();
            using (var site = new SPSite(url))
            {
                using (var web = site.OpenWeb())
                {
                    var list = web.Lists[SiteDefinition.GetTorrentLib()];
                    var finishedTorrentItem = list.Items.Cast<SPListItem>().FirstOrDefault(item => (string) (item["TorrentName"]) == folderName);
                    if (finishedTorrentItem == null) return;
                    finishedTorrentItem["FileStatus"] = fileStatus;
                    finishedTorrentItem.Update();
                }
            }
        }
    }

    
}
