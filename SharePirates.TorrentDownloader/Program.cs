using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.SharePoint;
using Newtonsoft.Json;
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
        private static readonly IDictionary<string, Torrent> Torrents = new Dictionary<string, Torrent>();
        private static SPWeb _web;

        static void Main(string[] args)
        {
            SetupSignalR();
            SetupActivityMonitors();

            InitializeWorkingtorrents();
            string url = args.Length < 1 ? "http://fianbakken.com" : args[0];
            var site = new SPSite(url);
            _web = site.OpenWeb();
            var list = _web.Lists[SiteDefinition.GetTorrentLib()];
            while (true)
            {
                foreach (var item in list.Items.Cast<SPListItem>().Where(x => (string)x["FileStatus"] == "Not started" && (string)x["Approved"] == "Approved"))
                {
                    const string downloadFolder = TorrentDeletedPath;
                    foreach (String attachment in item.Attachments)
                    {
                        var attachmentAbsoluteUrl = item.Attachments.UrlPrefix + attachment;
                        var file = _web.GetFile(attachmentAbsoluteUrl);
                        if (!Directory.Exists(downloadFolder))
                        {
                            Directory.CreateDirectory(downloadFolder);
                        }
                        var data = file.OpenBinary();
                        var pathTorrentFile = Path.Combine(downloadFolder, attachment + ".torrent");
                        File.WriteAllBytes(pathTorrentFile, data);
                        Console.WriteLine("--File = " + file.Name);
                    }
                }
                Thread.Sleep(60000);
            }
        }

        private static void InitializeWorkingtorrents()
        {
            var directory = new DirectoryInfo(TorrentDownloadPath);
            foreach (var directoryInfo in directory.GetDirectories())
            {
                var torrentName = directoryInfo.Name;
                if (!Torrents.ContainsKey(torrentName))
                {
                    Torrents.Add(torrentName,new Torrent(20000f));
                }
            }
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
            var torrentDeletedByUtorrentActivityMonitor = new ActivityFileMonitor(TorrentDeletedPath);
            torrentDeletedByUtorrentActivityMonitor.RaiseFileDeletedEvent += (sender, eventArgs) => FileDeletedByUtorrent(eventArgs);
            var torrentAddedByUtorrentActivityMonitor = new ActivityFileMonitor(TorrentAddedPath);
            torrentAddedByUtorrentActivityMonitor.RaiseFileAddedEvent += (sender, eventArgs) => FileAddedByUtorrent(eventArgs);
            var fileChangedActivityMonitor = new ActivityFileMonitor(TorrentDownloadPath);
            fileChangedActivityMonitor.RaiseFileChangedEvent += (sender, eventArgs) => FileChanged(eventArgs);
        }

        private static void FileChanged(FileSystemEventArgs eventArgs)
        {
            var folder = Path.GetDirectoryName(eventArgs.FullPath);
            var f = new FileInfo(eventArgs.FullPath);

            var directory = f.Directory;
            var torrentName = directory.Name; // Path.GetFileName(Path.GetDirectoryName(eventArgs.FullPath));
            // TODO: report progress
            var folderSize = CalculateFolderSize(folder);
            Torrent torrent;
            
            if (Torrents.TryGetValue(torrentName, out torrent))
            {
                torrent.CurrentSize = folderSize;
            }
            var torrentArray = Torrents.Select(x => new { Name = x.Key, PercentComplete = Math.Round(x.Value.CurrentSize/x.Value.OriginalSize*100, 2) }).ToList();
            var json = JsonConvert.SerializeObject(torrentArray);
            _torrentHub.Invoke("UpdateTorrents", json);
        }

        private static void FileAddedByUtorrent(FileSystemEventArgs eventArgs)
        {
            var torrentName = Path.GetFileNameWithoutExtension(eventArgs.FullPath);
            // Torrent is finished
            // GetListItemByFolderName - foldername == torrentName?
            Torrents.Remove(torrentName);
            _torrentHub.Invoke("TorrentDownloaded", torrentName);
            SetListItemStatus(torrentName, "Downloaded");
        }

        private static void FileDeletedByUtorrent(FileSystemEventArgs eventArgs)
        {
            // Torrent is starting downloading
            var torrentName = Path.GetFileNameWithoutExtension(eventArgs.FullPath);
            if (Torrents.ContainsKey(torrentName))
            {
    // ALLREADY THERe
            }
            else
            {

                Torrents.Add(torrentName, new Torrent(20000F));
            }
            _torrentHub.Invoke("TorrentDownloading", torrentName);
            try
            {
                SetListItemStatus(torrentName, "Downloading");
            }
            catch (Exception)
            {
            }
        }

        private static void SetListItemStatus(string torrentName, string fileStatus)
        {
            var list = _web.Lists[SiteDefinition.GetTorrentLib()];
            var finishedTorrentItem = list.Items.Cast<SPListItem>().FirstOrDefault(item => item.Title == torrentName);
            if (finishedTorrentItem == null) return;
            finishedTorrentItem["FileStatus"] = fileStatus;
            finishedTorrentItem.Update();
        }

        private static float CalculateFolderSize(string folder)
        {
            float folderSize = 0.0f;
            try
            {
                //Checks if the path is valid or not
                if (!Directory.Exists(folder))
                    return folderSize;
                try
                {
                    foreach (string file in Directory.GetFiles(folder))
                    {
                        if (File.Exists(file))
                        {
                            var finfo = new FileInfo(file);
                            folderSize += finfo.Length;
                        }
                    }

                    folderSize += Directory.GetDirectories(folder).Sum(dir => CalculateFolderSize(dir));
                }
                catch (NotSupportedException e)
                {
                    Console.WriteLine("Unable to calculate folder size: {0}", e.Message);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Unable to calculate folder size: {0}", e.Message);
            }
            return folderSize;
        }
    }

    internal class Torrent
    {
        private float _originalSize;

        public Torrent(float originalSize)
        {
            _originalSize = originalSize;
        }

        public float OriginalSize
        {
            get
            {
                if (Math.Abs(_originalSize - default(float)) < 0.01f)
                {
                    return float.MaxValue;
                }
                return _originalSize;
            }
            set { _originalSize = value; }
        }

        public float CurrentSize { get; set; }
    }
}
