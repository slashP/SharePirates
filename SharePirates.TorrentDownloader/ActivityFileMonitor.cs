using System;
using System.IO;

namespace SharePirates.TorrentDownloader
{
    class ActivityFileMonitor
    {
        public ActivityFileMonitor(string path)
        {
            StartActivityMonitoring(path);
        }

        // System.IO
        readonly FileSystemWatcher _watchFolder = new FileSystemWatcher();
        public event EventHandler<FileSystemEventArgs> RaiseFileAddedEvent;
        public event EventHandler<FileSystemEventArgs> RaiseFileDeletedEvent;
        public event EventHandler<FileSystemEventArgs> RaiseFileChangedEvent;

        protected virtual void OnRaiseFileChangedEvent(FileSystemEventArgs e)
        {
            var handler = RaiseFileChangedEvent;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnRaiseFileDeletedEvent(FileSystemEventArgs e)
        {
            var handler = RaiseFileDeletedEvent;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnRaiseFileAddedEvent(FileSystemEventArgs eventArgs)
        {
            var handler = RaiseFileAddedEvent;
            if (handler != null) {
                handler(this, eventArgs);
            }
        }

        private void StartActivityMonitoring(string sPath)
        {
            _watchFolder.Path = sPath;
            _watchFolder.NotifyFilter = NotifyFilters.DirectoryName;
            _watchFolder.NotifyFilter =
            _watchFolder.NotifyFilter | NotifyFilters.FileName;
            _watchFolder.NotifyFilter =
            _watchFolder.NotifyFilter | NotifyFilters.Attributes;

            _watchFolder.Changed += EventRaised;
            _watchFolder.Created += EventRaised;
            _watchFolder.Deleted += EventRaised;

            try
            {
                _watchFolder.EnableRaisingEvents = true;
            }
            catch (ArgumentException)
            {
            }
        }

        /// <summary>
        /// Triggered when an event is raised from the folder activity monitoring.
        /// All types exists in System.IO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">containing all data send from the event that got executed.</param>
        private void EventRaised(object sender, System.IO.FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Changed:
                    OnRaiseFileChangedEvent(e);
                    break;
                case WatcherChangeTypes.Created:
                    OnRaiseFileAddedEvent(e);
                    break;
                case WatcherChangeTypes.Deleted:
                    OnRaiseFileDeletedEvent(e);
                    break;
            }
        } 
    }
}
