using Fianbakken.Sharepoint.Annotations.SP15;
using Fianbakken.Sharepoint.Annotations.Sharepoint;

namespace SharePirates.ContentTypes
{
    [ContentTypeList("MediaFiles", ListTemplate = "Asset Library")]
    [ContentType("Media", ContentTypeIds.Video, "SharePirates")]
    class Media
    {
        [Choices("Movie,TVShow")]
        [ContentTypeField(Type = FieldTypeDefinitions.Choice, DefaultValue = "Movie")]
        public string MediaType { get; set; }

    }
}
