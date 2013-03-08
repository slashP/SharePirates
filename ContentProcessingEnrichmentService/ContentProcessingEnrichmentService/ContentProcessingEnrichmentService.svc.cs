using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Office.Server.Search.ContentProcessingEnrichment;
using Microsoft.Office.Server.Search.ContentProcessingEnrichment.PropertyTypes;

namespace ContentProcessingEnrichmentService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ContentProcessingEnrichmentService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ContentProcessingEnrichmentService.svc or ContentProcessingEnrichmentService.svc.cs at the Solution Explorer and start debugging.
    public class ContentProcessingEnrichmentService : IContentProcessingEnrichmentService
    {
        // Defines the name of the managed property 'Filename'.
        private const string FileNameProperty = "Filename";

        // Defines the name of the managed property 'Author'
        private const string PathProperty = "Path";

        // Defines the temporary directory where binary data will be stored.
        private const string TempDirectory = @"C:\Temp";

        // Defines the error code for managed properties with an unexpected type.
        private const int UnexpectedType = 1;

        // Defines the error code for encountering unexpected exceptions.
        private const int UnexpectedError = 2;

        public ProcessedItem ProcessItem(Item item)
        {
            ProcessedItem processedItem = new ProcessedItem { ItemProperties = new List<AbstractProperty>() };

            AbstractProperty pathProperty = item.ItemProperties.FirstOrDefault(p => p.Name == "Path");
            if (pathProperty != null && pathProperty.ObjectValue.ToString().Contains(@"www.torrentreactor.net/torrents/"))
            {
                var pathProp = pathProperty as Property<string>;
                if (pathProp != null)
                {
                    var parser = new SharePirates.Torrent.Parsers.TorrentReactorParser();
                    var metaDatas = parser.GetMetaDataTorrentReactor(pathProp.Value.EndsWith("/") ?
                        pathProp.Value.Remove(pathProp.Value.Length - 1, 1) : pathProp.Value);

                    Random vRandom = new Random();;

                    string temptype;
                    int i;
                    if (metaDatas.TryGetValue("torrentCategories", out temptype))
                    {
                        string[] mTypes = { "HDTV", "DVDRIP", "TVSCREEN" };
                        i = vRandom.Next(0, 2);
                        temptype = mTypes[i];//movietype.Split(',').Count() > 1 ? movietype.Split(',').First() : movietype};
                        var movieType = new Property<string>()
                        {
                            Name = "MovieType",
                            Value = temptype
                        };
                        processedItem.ItemProperties.Add(movieType);
                    }


                    string[] mFileTypes = { "MP4", "AVI", "MPG" };
                    vRandom = new Random();
                    i = vRandom.Next(0, 2);
                    temptype = mFileTypes[i];//movietype.Split(',').Count() > 1 ? movietype.Split(',').First() : movietype};
                    var mFileType = new Property<string>()
                    {
                        Name = "MovieFileType",
                        Value = temptype
                    };
                    processedItem.ItemProperties.Add(mFileType);

                    string[] mGenreTypes = { "Comedy", "Sci-fi", "Thriller", "Action", "Drama", "Adventure" };
                    vRandom = new Random();
                    i = vRandom.Next(0, 5);
                    temptype = mGenreTypes[i];//movietype.Split(',').Count() > 1 ? movietype.Split(',').First() : movietype};
                    var mGenreType = new Property<string>()
                    {
                        Name = "MovieGenre",
                        Value = temptype
                    };
                    processedItem.ItemProperties.Add(mGenreType);

                    string size;
                    if (metaDatas.TryGetValue("torrentSize", out size))
                    {
                        var movieSize = new Property<string>()
                        {
                            Name = "MovieSize",
                            Value = size
                        };
                        processedItem.ItemProperties.Add(movieSize);
                    }

                    string title;
                    if (metaDatas.TryGetValue("torrentTitle", out title))
                    {
                        var stringTitle = new Property<string>()
                        {
                            Name = "Title",
                            Value = title
                        };
                        processedItem.ItemProperties.Add(stringTitle);
                    }

                    string stringurl;
                    if (metaDatas.TryGetValue("torrentUrl", out stringurl))
                    {
                        var stringUrl = new Property<string>()
                        {
                            Name = "MagnetLink",
                            Value = stringurl
                        };
                        processedItem.ItemProperties.Add(stringUrl);
                    }
                }
            }

            return processedItem;
        }



        #region "test"

        //private readonly ProcessedItem processedItemHolder = new ProcessedItem
        //{
        //    ItemProperties = new List<AbstractProperty>()
        //};
        //public ProcessedItem ProcessItem(Item item)
        //{
        //    processedItemHolder.ErrorCode = 0;
        //    processedItemHolder.ItemProperties.Clear();
        //    try
        //    {
        //        var pathProperty = item.ItemProperties.FirstOrDefault(p => p.Name == "Path");
        //        if (pathProperty != null)// && pathProperty.ObjectValue.ToString().Contains(@"www.torrentreactor.net/torrents/"))
        //        {
        //            var pathProp = pathProperty as Property<string>;
        //            if (pathProp != null)
        //            {
        //                var parser = new SharePirates.Torrent.Parsers.TorrentReactorParser();
        //                var metaDatas = parser.GetMetaDataTorrentReactor(pathProp.Value.EndsWith("/") ?
        //                    pathProp.Value.Remove(pathProp.Value.Length - 1, 1) : pathProp.Value);

        //                //string movietype;

        //                //if (metaDatas.TryGetValue("torrentCategories", out movietype))
        //                //{
        //                //    foreach (var mType in movietype.Split(Convert.ToChar(",")))
        //                //    {
        //                //        var movieType = new Property<string>()
        //                //        {
        //                //            Name = "MovieType",
        //                //            Value = mType
        //                //        };
        //                //        processedItemHolder.ItemProperties.Add(movieType);
        //                //    }

        //                //}



        //                string title;
        //                if (metaDatas.TryGetValue("torrentTitle", out title))
        //                {
        //                    var stringTitle = new Property<string>()
        //                    {
        //                        Name = "Title",
        //                        Value = title
        //                    };
        //                    processedItemHolder.ItemProperties.Add(stringTitle);
        //                }

        //                //string size;
        //                //if (metaDatas.TryGetValue("torrentSize", out size))
        //                //{
        //                //    var intSize = new Property<int>()
        //                //    {
        //                //        Name = "Size",
        //                //        Value = Convert.ToInt32(size)
        //                //    };
        //                //    processedItemHolder.ItemProperties.Add(intSize);
        //                //}

        //                //string stringurl;
        //                //if (metaDatas.TryGetValue("torrentUrl", out stringurl))
        //                //{
        //                //    var stringUrl = new Property<string>()
        //                //    {
        //                //        Name = "MagnetLink",
        //                //        Value = stringurl
        //                //    };
        //                //    processedItemHolder.ItemProperties.Add(stringUrl);
        //                //}
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        processedItemHolder.ErrorCode = UnexpectedError;
        //    } return processedItemHolder;
        //}
        //public ProcessedItem ProcessItem(Item item)
        //{
        //    ProcessedItem processedItem = new ProcessedItem { ItemProperties = new List<AbstractProperty>() };

        //    AbstractProperty pathProperty = item.ItemProperties.FirstOrDefault(p => p.Name == "Path");
        //    if (pathProperty != null)
        //    {
        //        var pathProp = pathProperty as Property<string>;
        //        if (pathProp != null)
        //        {
        //            var parser = new SharePirates.Torrent.Parsers.TorrentReactorParser();
        //            var metaDatas = parser.GetMetaData(pathProp.Value.EndsWith("/") ?
        //                pathProp.Value.Remove(pathProp.Value.Length - 1, 1) : pathProp.Value);

        //            //string movietype;

        //            //if (metaDatas.TryGetValue("Type", out movietype))
        //            //{
        //            //    var movieType = new Property<string>()
        //            //    {
        //            //        Name = "MovieType",
        //            //        Value = movietype
        //            //    };
        //            //    processedItem.ItemProperties.Add(movieType);
        //            //}



        //            string title;
        //            if (metaDatas.TryGetValue("Title", out title))
        //            {
        //                var stringTitle = new Property<string>()
        //                {
        //                    Name = "Title",
        //                    Value = title
        //                };
        //                processedItem.ItemProperties.Add(stringTitle);
        //            }
        //        }
        //    }

        //    return processedItem;
        //}
        #endregion






    }
}
