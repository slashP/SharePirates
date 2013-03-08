<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MoviePage.aspx.cs" Inherits="SharePirates.MoviePage.Layouts.SharePirates.MoviePage.MoviePage" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <SharePoint:CSSRegistration name= "<%$SPUrl:Pirate.css%>" runat="server" After="corev4.css"/>
    <script type="text/javascript" src="jquery-1.9.1.min.js"></script>
    <style>
        #globalNavBox {
            display: none;
        }

           #suiteBar {
                 height:100px;   
            }

	
            #suiteBarLeft, #suiteBarRight {
                 background: url('../images/MoviePage/planke.png') center center;
            }
            #ctl00_onetidHeadbnnr2 {
                display: none;
            }
            #s4-titlerow {
                display: none
            }
           

             #s4-bodyContainer {
                 background: url('../images/MoviePage/jolly-roger-1.jpg') no-repeat center center fixed;
                    -webkit-background-size: cover;
                    -moz-background-size: cover;
                    -o-background-size: cover;
                    background-size: cover; 
                    width: 100%; 
             }

             .moviePreview { 
              text-align:center;
              width:350px;
              height:300px;              
              float: left;

             }
             .moviePreview h2 {
                 color: black;
             }
             .movieTitle {
                 margin-left: 15px;
                 width: 320px;
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
            $("#ctl00_onetidHeadbnnr2").attr("src", "../images/MoviePage/pirate-dexter.png");
        });
        
    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="movieContainer">
 <% foreach (SPListItem item in MovieList.Items)
    {
        if (item.Url.EndsWith(".mp4"))
        { %>
    <div class="moviePreview">
        <div class="movieTitle"><h2><% = item.Name %></h2></div>
        <video width="320" height="240" controls>
            <source src="<%= SPContext.Current.Web.Url + "/" + item.Url %>" type="video/mp4">
        Your browser does not support the video tag.
        </video> 
    </div>
<% }
    } %>
</div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Movies 
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Movies
</asp:Content>
