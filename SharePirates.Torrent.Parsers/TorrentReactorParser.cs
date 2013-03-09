using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

        public IDictionary<string, string> GetMetaDataTorrentReactor(string url)
        {
            LoadHtmlFromUrl(url);
            var torrentUrl = GetHrefAttributeValue("name_ansv1");
            _values.Add("torrentUrl", torrentUrl);
            var divHtmlNodes = _document.DocumentNode.Descendants("div");
            var sizehtmlnode = divHtmlNodes.First(x => x.InnerHtml == "Størrelse:");
            var size = sizehtmlnode.ParentNode.Descendants("div").Last().InnerText.Split(Convert.ToChar(" "))[0];
            _values.Add("torrentSize", size);
            var title = _document.DocumentNode.Descendants("title").First().InnerText;
            _values.Add("torrentTitle", title);
            var categoryNodes = _document.DocumentNode.Descendants("div").First(x => x.InnerHtml == "Kategori:").ParentNode.Descendants("a");
            var categories = string.Join(",", categoryNodes.Select(x => x.InnerText));
            _values.Add("torrentCategories", categories);
            return _values;
        }

        private string GetHrefAttributeValue(string divId)
        {
            var htmlNodes = _document.DocumentNode.Descendants("div");
            var torrentUrlHtmlNode = htmlNodes.First(x => x.GetAttributeValue("id", string.Empty) == divId);
            var torrentUrl = torrentUrlHtmlNode.Element("a").GetAttributeValue("href", string.Empty);
            return torrentUrl;
        }


        public Dictionary<string, string> GetMetaData(string url)
        {
            LoadHtmlFromUrl(url);
            // Parse nodes
            //var metaNodes = _document.DocumentNode.SelectNodes("//div");

            //foreach (var metaNode in metaNodes)
            //{                
            //    var key = string.IsNullOrEmpty(metaNode.GetAttributeValue("id", string.Empty))?metaNode.GetAttributeValue("class", string.Empty);
            //    var value = metaNode.GetAttributeValue("content", string.Empty);
            //}

            //title
            InsertValuePair(_document.DocumentNode.SelectSingleNode("//div[@id='title']"),"id", "innerHtml");
            var metadtNodes = _document.DocumentNode.SelectNodes("//dl[@class='col1']/dt");
            var metaddNodes = _document.DocumentNode.SelectNodes("//dl[@class='col1']/dd");
            var count = 0;
            foreach (var node in metaddNodes.Select(metaddNode => metaddNode.HasChildNodes ? metaddNode.FirstChild : metaddNode))
            {
                _values.Add(metadtNodes[count].InnerHtml.Replace(":", ""), node.InnerHtml.Replace("\r\n", "").Trim());
                count += 1;
            }

            metadtNodes = _document.DocumentNode.SelectNodes("//dl[@class='col2']/dt");
            metaddNodes = _document.DocumentNode.SelectNodes("//dl[@class='col2']/dd");
            count = 0;
            foreach (var metaddNode in metaddNodes)
            {
                var node = (metaddNode.HasChildNodes && 
                    string.IsNullOrEmpty(metaddNode.FirstChild.InnerHtml.Replace("\r\n","").Trim())) ? 
                    metaddNode.SelectSingleNode("a") : metaddNode;
                _values.Add(metadtNodes[count].InnerHtml, node.InnerHtml);
                count += 1;
            }            


            InsertValuePair(_document.DocumentNode.SelectSingleNode("//div[@class='download']/a"), "magnet", "href");
            return _values;
        }

        private void InsertValuePair(HtmlNode metaNode, string keyName, string valueName)
        {
            var key = string.IsNullOrEmpty(metaNode.GetAttributeValue(keyName, string.Empty)) ? 
                keyName : 
                metaNode.GetAttributeValue(keyName, string.Empty);
            var value = valueName.Equals("innerHtml", StringComparison.InvariantCultureIgnoreCase) ?
                metaNode.InnerHtml : 
                metaNode.GetAttributeValue(valueName, string.Empty);
            _values.Add(key, value.Replace("\r\n", "").Trim());
        }

        private void LoadHtmlFromUrl(string url)
        {
            var htmlDataBytes = _webClient.DownloadData(url);
            _document = new HtmlDocument();
            _document.LoadHtml(Encoding.UTF8.GetString(htmlDataBytes));
            _values = new Dictionary<string, string>();
        }
    }
}
