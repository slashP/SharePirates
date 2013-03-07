using Fianbakken.Sharepoint.Annotations.SP15;
using Fianbakken.Sharepoint.Annotations.Sharepoint;

namespace SharePirates.ContentTypes
{
    [ContentTypeList("TorrentFiles")]
    [ContentType("TorrentFile", ContentTypeIds.Item, "SharePirates")]
    public class TorrentFile
    {
        [ContentTypeField(BuiltinType = "Title")]
        public string Title { get; set; }

        [Choices("N/A,Not approved,Approved")]
        [ContentTypeField(Type = FieldTypeDefinitions.Choice, DefaultValue = "N/A")]
        public string Approved { get; set; }

        [ContentTypeField]
        public string ApprovalText { get; set; }

        [ContentTypeField]
        public string TorrentName { get; set; }

        [ContentTypeField]
        public string TorrentUrl { get; set; }

        
        [ContentTypeField]
        public int PercentCompleted { get; set; }
        
        [ContentTypeField]
        public string AnotherText { get; set; }

        [Choices("Not started,Downloading,Downloaded,Error")]
        [ContentTypeField(Type = FieldTypeDefinitions.Choice,DefaultValue = "Not started")]
        public string FileStatus { get; set; }


    }
}
