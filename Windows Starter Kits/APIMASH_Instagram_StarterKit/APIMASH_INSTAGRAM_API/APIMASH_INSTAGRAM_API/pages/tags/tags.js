// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";
    var publicMembers =
   {
       itemList: new WinJS.Binding.List(),
       tags:[]
   };
    WinJS.Namespace.define("InstagramTags", publicMembers);


    WinJS.UI.Pages.define("/pages/tags/tags.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        el:'',
        ready: function (element, options) {
            // TODO: Initialize the page here.
            var self = this;
            this.el = $(element);
            Instagram.init(InstagramAPIGlobals.clientId);
            // In case we have tags
            if (InstagramTags.tags.length)
            {
                this.processTags(InstagramTags.tags);
            }
            // Add listener to the search button
            $(this.el).find('.search button').click(function () {
                self.searchTags($('.search_field').val());
            });
        },

        /*
        Search for tags based on a string to match
        */
        searchTags:function( value )
        {
            var self = this;
            Instagram.searchTagsByName(value).done(function (data) {
                self.processTagInfo(data);
            })
        },

        /*
        Process the tag data and put in a basic array
        */
        processTagInfo:function(data)
        {
            var arr = [];
            var d  = data.data;
            var len = d.length;
            for(var i=0;i<len;i++)
            {
                arr.push(d[i].name);
            }

            this.processTags(arr);
        },

        /*
        Get media by tag
        */
        getMediaByTag:function(tag)
        {
    
            var self = this;
            Instagram.getRecentMediaByTag(tag).done(function (data) {
                self.processData(data);
            });
        },

        /*
        process the data from the media search
        */
        processData:function(data)
        {
            var tagsList = [];
            var tagHash = {};
            var len = data.data.length;
            var myList = new WinJS.Binding.List();
            
            for (var i = 0; i < len; i++)
            {
              myList.push(data.data[i]);    
            }
            // force an update 
            InstagramTags.itemList = myList;
            document.getElementById('basicListView').winControl.itemDataSource = myList.dataSource;
            myList.notifyReload();
        },

        /*
        Add the tags as a link to a list
        */
        processTags: function (tags) {
            InstagramTags.tags = tags;
          
            var self = this;
            $(this.el).find('ul li').remove();
            if (!tags.length) {
                var div = "<li><a href='#' class='a-tag'>No tags found</a></li>"
                var item = $(this.el).find('ul').append(div);
                return;
            }
            for (var i = 0; i < tags.length; i++)
            {
                var div = "<li><a href='#' class='a-tag'>" + tags[i] + "</a></li>"
                $(this.el).find('ul').append(div);
            }

            $(this.el).find('a').click(function (evt) {
                evt.preventDefault();
                var tag = $(evt.target).text();
                self.getMediaByTag(tag);
            })
        },

        unload: function () {
         
            // TODO: Respond to navigations away from this page.
        },

        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />

            // TODO: Respond to changes in viewState.
        }
    });
})();
