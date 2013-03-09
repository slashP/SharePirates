<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MoviePage.aspx.cs" Inherits="SharePirates.MoviePage.Layouts.SharePirates.MoviePage.MoviePage" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
<link href="theatre.css" rel="stylesheet" type="text/css" />
<link href="Pirate.css" rel="stylesheet" type="text/css" />
<script type="text/javascript" src="jquery-1.9.1.min.js"></script>
<script type="text/javascript" src="jquery.theatre.min.js"></script>
<style>
    #s4-bodyContainer {
        background: url('../images/MoviePage/jolly-roger-1.jpg') no-repeat center center fixed;
    }

     #contentRow {
         margin-left: -200px;
     }

    #s4-bodyContainer {
        -webkit-background-size: cover;
        -moz-background-size: cover;
        -o-background-size: cover;
        background-size: cover; 
        width: 100%; 
    }

    #globalNavBox, #suiteBar, #titleAreaBox, #suiteLinksBox, #sideNavBox, #suiteLinksBox, #DeltaSuiteBarRight {
        display: none;
    }
	
    #suiteBarLeft, #suiteBarRight {
        background: url('../images/MoviePage/planke.png') center center;
    }

    #s4-bodyContainer {
        background: url('../images/MoviePage/jolly-roger-1.jpg') no-repeat center center fixed;
        -webkit-background-size: cover;
        -moz-background-size: cover;
        -o-background-size: cover;
        background-size: cover; 
        width: 100%;
        min-height: 100%;
    }

    .moviePreview { 
        text-align:center;
        float: left;
    }

    .moviePreview h2 {
        color: black;
    }

    .hiddenMovie {
        display: none;
    }
    .movieTitle {
        margin-bottom: 30px;
        margin-left: 12px;
        width: 300px;
        background-color: peru;
    }
 
    .movieContainer{
        overflow: auto;
        width: 100%;
    }
    video {
        margin-top: -30px;
    }
</style>
<script type="text/javascript">
    $(document).ready(function() {
        $("#s4-titlerow").html("<a href='http://win-5cdumdj4n76/' title='Back to ship, matey!'><img src='../images/MoviePage/pirate-dexter.png' height='100'></a>");
        $("#s4-titlerow").css("height", "200px");
            

        $("#prevMovie").on('click', function () {
            alert("Prev movie clikced");
        });
        $("#nextMovie").on('click', function () {
            alert("Next movie clikced");
        });
    });

    $(window).load(function () {
        $("#videoGallery").theatre({
            /* other options here */
            selector: ".moviePreview",
            effect: "3d",
            autoplay: false,
            controls: "horizontal",
            onAfterMove: function (idx, actor, theatre) {
                var previews = $("#videoGallery").find(".moviePreview");
                $.each(previews, function (index, value) {
                    var smallVideo = $(value).find('video');
                    var smallTitle = $(value).find('.movieTitle');
                    $(value).css("width", "300");
                    smallTitle.css("width", "300");
                    smallTitle.css("margin-left", "12px");
                    smallVideo.attr("width", "300");
                });
                var video = $(actor).find('video');
                var title = $(actor).find('.movieTitle');
                $(actor).css("width", "700px");
                $(actor).css("left", "500px");
                $(actor).css("top", "50px");
                title.css("width", "700px");
                title.css("margin-left", "0px");
                video.attr("width", "700px");
            }
        });
    });
    
</script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="movieContainer" id="videoGallery" style="visibility: hidden; text-align: center; width: 100%; height: 500px;">
     <% foreach (SPListItem item in MovieList.Items){
            if (item.Url.EndsWith(".mp4")){%>
        <div class="moviePreview"> 
            <div class="movieTitle"><h2><% = item.Name %></h2></div>
            <video width="300" controls>
                <source src="<%= SPContext.Current.Web.Url + "/" + item.Url %>" type="video/mp4">
            Your browser does not support the video tag.
            </video> 
        </div>
    <% } } %>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Movies 
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Movies
</asp:Content>
