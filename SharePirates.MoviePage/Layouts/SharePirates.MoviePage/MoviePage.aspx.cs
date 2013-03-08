using System;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SharePirates.MoviePage.Layouts.SharePirates.MoviePage
{
    public partial class MoviePage : LayoutsPageBase
    {
        public SPList MovieList;

        protected void Page_Load(object sender, EventArgs e)
        {

            MovieList = SPContext.Current.Web.Lists["MediaFiles"];

        }
    }
}
