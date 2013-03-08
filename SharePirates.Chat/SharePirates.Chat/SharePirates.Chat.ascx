<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharePirates.Chat.ascx.cs" Inherits="SharePirates.Chat.VisualWebPart1.VisualWebPart1" %>

<input type="text" id="chatTextbox" />
<input type="button" id="chatButton" value="Chat"/>
<div id="chatArea"></div>
<div id="soundContainer"></div>
<script src="/_layouts/15/SharePirates.Chat/jquery-1.9.1.min.js" type="text/javascript"></script>
<script src="/_layouts/15/SharePirates.Chat/jquery.signalR-1.0.1.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="/_layouts/15/sp.debug.js"></script>


<script type="text/javascript" src="http://ciberpirates.apphb.com/signalr/hubs"></script>
<script type="text/javascript">
    var sendMessage;
    $(document).ready(function () {
        $(function () {
            var chatHub = $.connection.chatHub;
            $.connection.hub.url = 'http://ciberpirates.apphb.com:80/signalr';
            $.support.cors = true;
            $.extend(chatHub.client, {
                sendMessage: function (result) {
                    //$("#chatArea").html("");
                    var newHtml = "";
                    for (var i = 0; i < result.length; i++) {
                        var chatMessage = result[i];
                        var chatTag = "<div class='chatMessage'>" + chatMessage.User + "<br />" + chatMessage.Message + "</div>";
                        if (chatMessage.Message.toLowerCase().indexOf("arr") >= 0) { playSound("arr.mp3"); }
                        if (chatMessage.Message.toLowerCase().indexOf("mate") >= 0) { playSound("matey.mp3"); }
                        if (chatMessage.Message.toLowerCase().indexOf("plank") >= 0) { playSound("plank.mp3"); }
                        if (chatMessage.Message.toLowerCase().indexOf("timber") >= 0) { playSound("timbers.mp3"); }
                        newHtml += chatTag;
                    }
                    $("#chatArea").prepend(newHtml);
                    $("#chatArea").html($("#chatArea > div").slice(0, 10));
                }
            });
            //setTimeout(function () { // hack. couldn't get start().done(function(){ to work for some reason
            //    chatHub.server.getAll();
            //}, 2500);
            $.connection.hub.start({ transport: 'longPolling', xdomain: true }).done(function () {
                chatHub.server.getAll();
            });
            $("#chatButton").click(function () {
                getUserName2();
                //sendMessage();
            });

            $('#chatTextbox').keypress(function (e) {
                if (e.which == 13) {
                    getUserName2();
                    //sendMessage();
                }
            });
            sendMessage = function (user) {
                var message = $("#chatTextbox").val();
                chatHub.server.send(message, user);
                $("#chatTextbox").val('');
            };

        });
        function getUserName() {
            var thisUser = $().SPServices.SPGetCurrentUser({
                fieldName: "Name",
                debug: false
            });
            return thisUser;
        }
        var currentUser;
        function getUserName2() {
            this.clientContext = new SP.ClientContext.get_current();
            this.oWeb = clientContext.get_web();
            currentUser = this.oWeb.get_currentUser();
            currentUser.retrieve();
            this.clientContext.load(currentUser);
            this.clientContext.executeQueryAsync(Function.createDelegate(this, onQuerySucceeded), Function.createDelegate(this, onQueryFailed));
        }

        function onQuerySucceeded() {
            var loginName = currentUser.get_title();
            console.log(loginName);
            sendMessage(loginName);
            //alert(currentUser.get_loginName()); 
        }

        function onQueryFailed(sender, args) {
            alert('Query failed!');
        }
        
        function playSound(soundfile) {
            $("#soundContainer").html("<embed src='../_layouts/15/SharePirates.Chat/" + soundfile + "' hidden='true' autostart='true' loop='false' />");
            $("#soundContainer embed").load();
        }
    });

</script>