// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";
    var publicMembers =
     {
         itemList: new WinJS.Binding.List(),
         tags:[]
     };
    WinJS.Namespace.define("InstagramPopular", publicMembers);
    
    WinJS.UI.Pages.define("/pages/popular/popular.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        el:'',
        ready: function (element, options) {
            // TODO: Initialize the page here.
            Instagram.init(InstagramAPIGlobals.clientId);
            if (InstagramPopular.tags.length)
            {
                this.processTags(InstagramPopular.tags);
            }
            this.el = $(element);
            this.getPopularMedia();
        },

        getPopularMedia: function () {
            
            var self = this;
            Instagram.getPopularMedia().done(function (data) {
                self.processData(data);
            })
        },

        processData: function (data)
        {

        
            var tagsList = [];
            var tagHash = {};
            // loop through the items
            var len = data.data.length;
            var myList= new WinJS.Binding.List();
            for (var i = 0; i < len; i++)
            {
                // Need to loop through tags
               var item = data.data[i];
               var tags = item.tags;
               var tlen = tags.length;
               for (var j = 0; j < tlen; j++)
               {
                   var label = tags[i];
                   if (!tagHash[label])
                   {
                      tagHash[label] = true;
                      if (label)
                      {
                         tagsList.push(label);
                      }
                   }
               }
                myList.push(item);
              }
             // create the tag list 
            this.processTags(tagsList);
      
      
            // force an update 
            InstagramPopular.itemList = myList;
            var list = document.getElementById('popularListView').winControl;
            list.itemDataSource = myList.dataSource;
            myList.notifyReload();
            
          
    
            
          
        },

        getMediaByTag:function(tag)
        {
           
            var self = this;
            Instagram.getRecentMediaByTag(tag).done(function (data) {
                self.processData(data);
            });
            
        },

        processTags: function (tags) {
            InstagramPopular.tags = tags;
            var self = this;
            $(this.el).find('ul li').remove();
            if (!tags.length) {
                var div = "<li><a href='#' class='a-tag'>No tags found</a></li>"
                $(this.el).find('ul').append(div);
                return;
            }
            for (var i = 0; i < tags.length; i++)
            {
               
                var div = "<li><a href='#' class='a-tag'>" + tags[i] + "</a></li>"
                $(this.el).find('ul').append(div);
              
            }

            $(this.el).find('ul li a').click(function (evt) {
                evt.preventDefault();
             
                var tag = $(evt.target).text();
                self.getMediaByTag(tag);

            });

        },

        unload: function () {
            // TODO: Respond to navigations away from this page.
            InstagramPopular.itemList = new WinJS.Binding.List();
        },

        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />

            // TODO: Respond to changes in viewState.
        }
    });
})();
