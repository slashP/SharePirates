using System;
using System.IO;
using System.Net;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SharePirates.Artefacts.Layouts.SharePirates.Artefacts
{
    public partial class DownloadTorrent : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var torrentUrl = Request.Params["torrentUrl"];
            var title = Request.Params["title"];
            using (var client = new WebClient())
            {
                if (string.IsNullOrEmpty(torrentUrl))
                {
                    return;
                }
                var data = client.DownloadData(torrentUrl);
                var stream = new MemoryStream(data);
                AddTorrentToList(stream, title + ".torrent");
            }
        }

        protected void AddTorrentToList(Stream stream, string filename)
        {
            SPContext.Current.Web.AllowUnsafeUpdates = true;
            var contenttype = SPContext.Current.Web.ContentTypes["TorrentFile"];
            var lib = SPContext.Current.Web.Lists["TorrentFiles"];

            var data = new byte[stream.Length];
            stream.Read(data, 0, (int) stream.Length);

            var item = lib.AddItem();
            item[SPBuiltInFieldId.ContentTypeId] = contenttype.Id;
            item[SPBuiltInFieldId.Title] = filename;
            item.Attachments.Add(filename, data);
            item.SystemUpdate();
            item.Update();
            
            Web.Update();

            SPContext.Current.Web.AllowUnsafeUpdates = false;
        }
    }
}
