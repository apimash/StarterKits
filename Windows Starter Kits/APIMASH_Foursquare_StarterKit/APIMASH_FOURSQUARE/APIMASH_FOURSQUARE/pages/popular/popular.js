// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";
    var loc = null;
    var publicMembers = {
        itemList: new WinJS.Binding.List(),
        locations: []
    };
    WinJS.Namespace.define("FoursquarePopular", publicMembers);
    WinJS.UI.Pages.define("/pages/popular/popular.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        el:'',
      
        ready: function (element, options) {
            // TODO: Initialize the page here.
            var self = this;
            this.el = $(element);
            Foursquare.init(FoursquareAPIGlobals.clientId, FoursquareAPIGlobals.clientSecret);
            this.getPopularVenues();
        },

        /* Please see the params for the popular venues via the foursquare api*/
        getPopularVenues: function () {
            var self = this;

            Foursquare.getPopularVenues()
                .done(function (data) {
                   
                    self.processVenues(data);
                })
                .error(function (err) {
                    self.showError(err);

                });
        },

        showError: function (err) {

            $('#msg').text(err.statusText);
        },

        processVenues: function (data) {
         
            if (!data.response.groups) {
                $('#msg').text('No Recommendations Found');
                return;
            }
            $('#msg').fadeOut();
            var len = data.response.groups.length;
          
            var d = data.response.groups;
            var myList = new WinJS.Binding.List();
            for (var i = 0; i < len; i++) {
                var name = d[i].name;
                var items = d[i].items;
                var vlen = items.length;
                for (var j = 0; j < vlen; j++)
                {
                    var item = items[j].venue;
                    item['cat_name'] = name;
                    myList.push(item);
                }
                
            }
            FoursquarePopular.itemList = myList;
            document.getElementById('popularListView').winControl.itemDataSource = myList.dataSource;
            myList.notifyReload();
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