Add-Pssnapin Microsoft.Sharepoint.Powershell

$surl = "http://fianbakken.com"
$wurl = $surl;

$site = get-spsite
$web = $site.RootWeb;


# Some neat startup methods
$DEVPATH = "C:\Dev\ASPC2013\SharePirates"
# ADD ALL PATHS where WSPs or binaries needed should be loaded from
# WSPS will be fetched automagically from here.
$DEVPATHS = @{
                "ContentTypes" = "$DEVPATH\SharePirates.ContentTypes\SharePirates.ContentTypes\bin\Debug\";  
                "SetupFramework" = "$DEVPATH\binaries\"; 
                "TorrentPath" = "$DEVPATH\torrents\" 
                "MovieImport" = "C:\Temp\video\";            
             };



Set-Location "$DEVPATH"


function Setup-Movies(){
    $path = "$($DEVPATHS['MovieImport'])";
    $files = ls -include *.mp4 -Recurse -Path $path;

    $list = $web.Lists["MediaFiles"];


    $files|%{
        $meta = $_.Fullname.Replace($_.Extension, ".txt");
        Write-Host -ForegroundColor Green "$meta"

        if(-not [System.IO.File]::Exists($meta)){            
            New-Item  -Path $meta -ItemType file;                
        }

        $csv = Import-Csv -Path $meta -Delimiter ";"

        
        $csv[0].Title;

        $stream = $_.OpenRead();
        $length = $stream.Length;
        
        $file= $list.RootFolder.Files.Add($_.Name, $stream, $true);


        $file["ContentTypeId"] = $web.ContentTypes["Media"].Id.ToString();
        

        $list.Update();

        $file;
    }

}

function Print-Values(){    
    begin {

    } 

    process {
        $item = $_;
        $_.Fields|%{ @{$_.InternalName = $item[$_.InternalName];} }

    }
}
function Setup-Data(){
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession  ;
    $cc = New-Object System.Net.CookieContainer  ;
    $a=("OAID","bec938cf83d6b556caddf46cffa60f5b","ads.fulldls.com/","2147484672","3814638208","30358202","1925486647","30284777","*");
    $a|%{
       
        $cookie = New-Object System.Net.Cookie($_, $_, "/","torrentreactor.net");
        $cc.Add($cookie);
    }
    $session.Cookies = $cc;


    $data = Invoke-WebRequest "http://www.torrentreactor.net/rss.php?sid=269"
    $xml = [xml]$data.Content;
    $inner = $xml.rss.channel.InnerXml.Replace("item", "feeditem");
    $fix = [xml]"<items>$inner</items>";
    $fix.items.feeditem|%{        
        $d = $_.description;
        $categories = $d.Substring(0, $d.IndexOf("Size:")).Replace("Category: ", "").Replace(".","").Split("-")|%{$_.Trim();}
        $size = $d.Substring($d.IndexOf("Size:"),  $d.IndexOf("Status:")-$d.IndexOf("Size:")).Replace("Size:","").Trim();
        $status = $d.Substring($d.IndexOf("Status:"),  $d.IndexOf("Hash:")-$d.IndexOf("Status:")).Replace("Status:","").Trim();
        $link = $_.link;
        $title= $_.title;

        Write-Host -ForegroundColor Green "Downloading torrent info from $link"

        $web = Invoke-WebRequest $link -WebSession $session -UseBasicParsing;
        $links = $web.Links;
        $directLink = $links|? title -eq "Download"
        $file="";
        if($directLink){
            $url = $directLink.href;
            $file = $directLink.innerHtml.Replace("torrent",".torrent").Replace("..",".");          
            $file = "$($DEVPATHS['SetupFramework'])$file"
            $fakefile = "$($DEVPATHS['SetupFramework'])Fake.Torrent"
            
            Write-Host "Downloading $url" -ForegroundColor Green;
            $file = Invoke-WebRequest $url -OutFile $file -WebSession $session -UseBasicParsing

            
            #cp $fakefile $file;
        }

           @{ 
                "categories"=$categories;
                "size"=$size;
                "status"=$status;
                "title"=$title;
                "link"=$link;
                "file"=$file;
            }
    }
}

function Create-Setup () {   
    # Add required DLL
    $SetupDll = "$($DEVPATHS['SetupFramework'])Fianbakken.Sharepoint.Setup.dll"; 
    Add-Type -Path $SetupDll;
    $setup = New-Object Fianbakken.Sharepoint.Setup.SetupContentTypesForSharepoint;   
    return $setup;
}


function Setup-Types() {

    $contentTypesDll = "$($DEVPATHS['ContentTypes'])SharePirates.ContentTypes.dll"; 
    $namespace = "SharePirates.ContentTypes";

    $setup = Create-Setup;

    return $setup.SetupContentTypesFromAssembly($web, $contentTypesDll, $namespace, $true);

}



function restart() {
	Start-Process powershell_ise
	exit
}

New-Alias rs restart;

# Add the framework for autogenerating types


#Local module path
$MODULEPATH = ($env:PSModulePath).Split(";")[0]

New-Alias -Name sudo "$MODULEPATH\sudo.ps1" -Force

# Just add the whole bunch from dev-tools
$env:path += "C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\CommonExten
sions\Microsoft\TestWindow;C:\Program Files (x86)\Microsoft SDKs\F#\3.0\Framewor
k\v4.0\;C:\Program Files (x86)\Microsoft Visual Studio 11.0\VSTSDB\Deploy;C:\Pro
gram Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\;C:\Program Files (x86
)\Microsoft Visual Studio 11.0\VC\BIN;C:\Program Files (x86)\Microsoft Visual St
udio 11.0\Common7\Tools;C:\Windows\Microsoft.NET\Framework\v4.0.30319;C:\Windows
\Microsoft.NET\Framework\v3.5;C:\Program Files (x86)\Microsoft Visual Studio 11.
0\VC\VCPackages;C:\Program Files (x86)\HTML Help Workshop;C:\Program Files (x86)
\Microsoft Visual Studio 11.0\Team Tools\Performance Tools;C:\Program Files (x86
)\Windows Kits\8.0\bin\x86;C:\Program Files (x86)\Microsoft SDKs\Windows\v8.0A\b
in\NETFX 4.0 Tools;C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\;C:\W
indows\system32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsP
owerShell\v1.0\;C:\Program Files\Microsoft SQL Server\110\DTS\Binn\;C:\Program F
iles (x86)\Microsoft SQL Server\110\Tools\Binn\;C:\Program Files\Microsoft SQL S
erver\110\Tools\Binn\;C:\Program Files (x86)\Microsoft SQL Server\110\Tools\Binn
\ManagementStudio\;C:\Program Files (x86)\Microsoft Visual Studio 10.0\Common7\I
DE\PrivateAssemblies\;C:\Program Files (x86)\Microsoft SQL Server\110\DTS\Binn\;
C:\Program Files\Microsoft Office Servers\15.0\Bin\;C:\Program Files (x86)\Micro
soft ASP.NET\ASP.NET Web Pages\v1.0\;C:\Program Files\TortoiseSVN\bin;C:\Program
 Files\Microsoft\Web Platform Installer\"


function Delete-Term($term){ $term.Terms|%{Delete-Term($_);};$term.Delete();  } 
function Delete-Group($group){ $group.TermSets|%{ $_.Terms|%{ Delete-Term($_) }; $_.Delete(); }; $group.Delete() ; }  


function Clean-TermStore(){
    $session = New-Object Microsoft.Sharepoint.Taxonomy.TaxonomySession($site); 
    $store = $session.TermStores[0]; 
    $theGroups=$store.Groups|?{$_.Name -Match "Site Collection"} 
    $theGroups|%{ Delete-Group($_);}  
    $store.CommitAll();
}

function Reload-Site(){
    $global:site = get-spsite
    $global:web = $site.RootWeb;
}

function Remove-Site(){
    Remove-SPSite $site
}

function Feature-Enable(){
    $feature=Get-SPFeature -Identity "Wms.Gims.Assets_IpLogFeature"
    Enable-SPFeature $feature -Url $surl    
}

function Setup-Site(){
    $t = Get-SPWebTemplate "BLANKINTERNET#0"
    $site = New-SPSite -Url $surl -SecondaryOwnerAlias "fianbakken\Harald Fianbakken" -OwnerAlias "fianbakken\spdbadmin" -Template $t -language 1033;    
}

function Clean-Site(){
    Remove-Site;    
    Clean-TermStore;

    Setup-Site;
    Reload-Site;

    Setup-Types
}

function Setup-Urls(){
    

    $editFormUrl =  "_layouts/IPLog/IPForm.aspx";

    $web.ContentTypes["ImprovementPotential"].EditFormUrl = $editFormUrl;
    $web.ContentTypes["ImprovementPotential"].NewFormUrl =  $editFormUrl;     
    $web.ContentTypes["ImprovementPotential"].Update($true);

    $web.Update();

}


function Get-Wsps(){
    $items = @();    
    $DEVPATHS.Keys|%{
        $items+=Get-ChildItem $DEVPATHS[$_] -Filter *.wsp
    }
    return $items;
}

function Remove-Gac(){
   (Get-Wsps)|%{
        $dll= $_.Name.Replace(".wsp","");
        Write-Debug "Removing $dll from GAC"
        Gacutil /u $dll
    }

}

function Add-SPSolutions(){
    (Get-Wsps)|%{
        Add-SPSolution -LiteralPath $_.Fullname
    }
}

function Install-SPSolutions(){
    (get-wsps)|%{       
        Write-Debug "Installing solution $($_.Name) "
        Install-SPSolution -Identity $_.Name -AllWebApplications -GACDeployment -Force
        while(-not (Get-SPSolution -Identity $_.Name).Deployed){
            Write-Debug "..";
            sleep(1);
        }
    }
}

function Uninstall-SPSolutions(){    
    (get-wsps)|%{        
        Write-Debug "Uninstalling solution $($_.Name) "
        Uninstall-SPSolution -Identity $_.Name -WebApplication AllWebApplications -ErrorAction SilentlyContinue
        while(-not(Get-SPSolution -Identity $_.Name).Deployed){
            Write-Debug "..";
            sleep(1);
        }
    }
}

function Remove-SPSolutions(){    
    (get-wsps)|%{        
        Write-Debug "Removing solution $($_.Name)"
        Remove-SPSolution -Identity $_.Name -Force 
    }
}

function Clean-SPSolutions(){    
    Uninstall-SPSolutions;
    Remove-SPSolutions;
}



