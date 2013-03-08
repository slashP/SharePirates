using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;

namespace SharePirates.Torrent.Parsers
{
    public class TorrentReactorParser : IHtmlParser
    {
        private readonly WebClient _webClient;
        private Dictionary<string, string> _values; 
        private HtmlDocument _document;

        public TorrentReactorParser()
        {
               _webClient = new WebClient();
        }


        public Dictionary<string, string> GetMetaData(string url)
        {
            LoadHtmlFromUrl(url);
            // Parse nodes
            var metaNodes = _document.DocumentNode.SelectNodes("//meta");

            foreach (var metaNode in metaNodes)
            {
                var key = metaNode.GetAttributeValue("name", string.Empty);
                var value = metaNode.GetAttributeValue("content", string.Empty);
            }

            return _values;
        }

        private void LoadHtmlFromUrl(string url)
        {
            var html = _webClient.DownloadString(url);
            _document = new HtmlDocument();
            _document.LoadHtml(html);
            _values = new Dictionary<string, string>();
        }
    }
}
