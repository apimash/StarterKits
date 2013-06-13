/*
File: download.js
Author: Stacey Mulcahy- @bitchwhocodes, stacey.mulcahy@microsoft.com
Last Mod: 5/1/2013
Description: Helper class for downloading a remote file to your photo directory

*/

function DownloadOp() {
    var download = null;
    var promise = null;
    var imageStream = null;
 

    this.start = function (uriString, fileName) {
        try {
            // Asynchronously create the file in the pictures folder.
           Windows.Storage.KnownFolders.picturesLibrary.createFileAsync(fileName, Windows.Storage.CreationCollisionOption.generateUniqueName).done(function (newFile) {
                var uri = Windows.Foundation.Uri(uriString);
                var downloader = new Windows.Networking.BackgroundTransfer.BackgroundDownloader();

                // Create a new download operation.
                download = downloader.createDownload(uri, newFile);
           
                // Start the download and persist the promise to be able to cancel the download.
               download.startAsync().then(complete,error,progress);
           }, error);
        } catch (err) {
            //console.log(err.message);
        }
        
    };
    // On application activation, reassign callbacks for a download
    // operation persisted from previous application state.
    this.load = function (loadedDownload) {
        try {
            download = loadedDownload;
            //printLog("Found download: " + download.guid + " from previous application run.<br\>");
            promise = download.attachAsync().then(complete, error, progress);
        } catch (err) {
            //displayException(err);
        }
    };

   
    function complete(obj) {
        $('.meter').hide();
        $('.download-message').fadeIn(300).delay(300).fadeOut(300);
    }

    function progress() {
      
        $('.meter').show();
        var currentProgress = download.progress;
        var bytesReceived = currentProgress['bytesReceived'];
        var totalBytes = currentProgress['totalBytesToReceive'];
        var percent = Math.max(Math.floor((bytesReceived / totalBytes)*100),100);
        $('.meter > span').css('width', percent + '%');
    }

    function error(err) {
        //console.log("error" + err);
    }
    
}