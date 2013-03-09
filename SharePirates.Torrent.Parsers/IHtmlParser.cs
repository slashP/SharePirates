using System.Collections.Generic;

namespace SharePirates.Torrent.Parsers
{
    interface IHtmlParser
    {
        Dictionary<string, string> GetMetaData(string url);
        IDictionary<string, string> GetMetaDataTorrentReactor(string url);
    }
}
