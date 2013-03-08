using System;
using System.IO;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SharePirates.Common;

namespace SharePirates.Artefacts.Layouts.SharePirates.Artefacts
{
    public partial class DownloadTorrent : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void AddTorrentToList(Stream stream, string filename)
        {
            var contenttype = SPContext.Current.Web.ContentTypes["TorrentFile"];
            var lib = SPContext.Current.Web.Lists[SiteDefinition.GetTorrentLib()];

            var data = new byte[stream.Length];
            stream.Read(data, 0, (int) stream.Length);

            var item = lib.AddItem();
            item[SPBuiltInFieldId.ContentTypeId] = contenttype.Id;
            item[SPBuiltInFieldId.Title] = filename;
            item.Attachments.Add(filename, data);
            item.SystemUpdate();
            item.Update();
            
            Web.Update();
        }
    }
}
