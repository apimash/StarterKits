

/*
File: tumblrapi.js
Author: Stacey Mulcahy- @bitchwhocodes, stacey.mulcahy@microsoft.com
Last Mod: 5/1/2013
This is the basic UI class - handles all User interface actions
Description: Helper file for tumblr api - basic search by tags
*/

(function () {
    "use strict";

    WinJS.Namespace.define("tumblrapi", {
        // stacey use underscore and clean up your collection bs. 
        _currentItem: null,
        _currentIndex: -1,
        list: null,
        /*
        Replace this Developer key with your own.
        */
        api: new API.TumblrApi('QGnCeExI62suDlMk67ECyjGyswIZZ9mehwb9VkYKqnatayldaX'),
        /*
        Initialize the UI
        */
        
      
        initialize: function () {
            $('#previous').click($.proxy(this.goPrevious, this));
            $('#next').click($.proxy(this.goNext, this));
            $('#download').click($.proxy(this.download, this));
            $('img').load($.proxy(this.onImageLoaded, this));

            this.layout();
            this.getData();

        },
        
        layout: function () {
            var width = $('img').width();
            var height = $('img').height();
            
            var xpos = ($(window).width() - width) / 2;
            var ypos = (400 - height) / 2;
            var top = (height) / 2;
            var left = (width - 200) / 2;
            $('.bar').css({ 'margin-top': top + 'px', 'margin-left': left + 'px' });
            $('.controls').css({ 'margin-left': ($(window).width() - width) / 2 });
            $('.controls-container').css({ width: width+20 + 'px' });
            $('.meter').css({ width: width + 'px' });
            $('.img-holder').css({ 'margin-left': xpos + 'px', 'margin-top': ypos + 'px' });
        },
            /*
        Download the current image
        See DownloadOperation Class
        */
        download: function () {
            var url = $('img').attr('src');
            var name = url.substring(url.lastIndexOf('/') + 1, url.length);
            var newDownload = new DownloadOp(tumblrapi);
           newDownload.start(url, name);
         
        },

       
            /*
          Get data from the api 
          Call onListUpdate when complete 
        */
        getData: function() {
            var that = this;
            this.api.getTags().done(function (response) {
                that.onListUpdate(response);
            });
        },
        
        /*
        Callback for when new data from the api has been received
        Put the results in a WinJs List
        Update the ui. 
        */
        onListUpdate: function (response) {
            if (!this.list) {
                this.list = new WinJS.Binding.List();
            }
            var len = response.length;
            for (var i = 0; i < len; i++) {
                this.list.push(response[i]);
            }
            this.update();
        },

        /*
        Callback for when the image as loaded. 
        Fade the loader bar out and then call the _showImage method when complete
        */
        onImageLoaded: function() {
            $('.bar').delay(500).fadeOut(500, $.proxy(this._showImage, this));
        },

        /*
        Show Image
        Hide the loader bar, set the size of the image to reflect the image that will be loaded in,
        and fade the image up
        */
        _showImage: function() {
            var item = this.list.getAt(this._currentIndex);
            var height = item.getHeight();
            var width = item.getWidth();
            $('.bar').hide();
            $('img').css({ width: width + 'px', height: height + 'px', display: 'block' });
            $('img').animate({ 'opacity': 1 }, 300);

        },
        /*
        Increment the index pointer for the item we are looking at
        Show the item
        */
        update: function() {
            this._currentIndex++;
            this.showItem();
        },
        /*
        Get the next item in the list 
        If there are no more items, lets call the api to get more items
        */
        goNext: function () {
            if (this._currentIndex < this.list.length - 2) {
                this._currentIndex++;
                this.showItem();
            } else {
                {
                    this.getData();
                }
            }
        },
        /*
        Get the previous item in the list
        If we are at the first item, we can't move the cursor
        */
        goPrevious: function() {
            if (this._currentIndex > 0) {
                this._currentIndex--;
            }
            this.showItem();
        },
        /*
        Show the current item
        We need to load the item, so lets hide the image, show the loading bar
        and animate the size of the img container to the new current image size and load it in
        */
        showItem: function () {
      
            var item = this.list.getAt(this._currentIndex);
            var height = item.getHeight();
            var width = item.getWidth();

            var w = $('.bar-holder').width();
            var image = item.getImage();
            $('img').hide();
            $('.bar').show();
            var xpos = ($(window).width() - width) / 2;
            var ypos = (400 - height) / 2;
            
            $('.img-holder').animate({ 'margin-left':xpos,'margin-top':ypos,width: width + 'px', height: height + 'px' },
                {
                    duration: 300,
                    progress: function() {
                        var top = ($(this).height()) / 2;
                        var left = ($(this).width() - 200) / 2;
                        $('.bar').css({ 'margin-top': top + 'px', 'margin-left': left + 'px' });
                        $('.controls').css({ 'margin-left': ($(window).width() - width) / 2 });
                        $('.controls-container').css({ width: width+20 + 'px' });
                        $('.meter').css({ width: width + 'px' });
                        $('.progress-loader').css({ width: width + 'px' });
                    },
                    complete: function() {
                        $('img').attr('src', image);
                    }
                });
        },
    });
    

    


})();