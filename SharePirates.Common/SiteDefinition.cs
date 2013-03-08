using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePirates.Common
{
    public static class SiteDefinition
    {
        public static string GetDefaultUrl()
        {
            return "http://fianbakken.com";
        }

        public static string GetTorrentLib()
        {
            return "TorrentFiles";
        }
    }
}
