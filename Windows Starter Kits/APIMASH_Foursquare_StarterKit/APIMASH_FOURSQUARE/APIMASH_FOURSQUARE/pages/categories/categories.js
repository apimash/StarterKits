// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";
    var loc = null;
    var publicMembers = {
        itemList: new WinJS.Binding.List(),
        locations: []
    };
    WinJS.Namespace.define("FoursquareCategories", publicMembers);
    WinJS.UI.Pages.define("/pages/categories/categories.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        el: '',

        ready: function (element, options) {
            // TODO: Initialize the page here.
            var self = this;
            this.el = $(element);
            Foursquare.init(FoursquareAPIGlobals.clientId, FoursquareAPIGlobals.clientSecret);
            this.getCategories();
        },

        /* Please see the params for the popular venues via the foursquare api*/
        getCategories: function () {
            var self = this;

            Foursquare.getVenueCategories()
                .done(function (data) {
                   
                    self.processCategories(data);
                })
                .error(function (err) {
                    self.showError(err);

                });
        },

        showError: function (err) {

            $('#msg').text(err.statusText);
        },

        processCategories: function (data) {

            var categories = data.response.categories;
            var len = categories.length;
            var cats = [];
            for (var i = 0; i < len; i++)
            {
                var obj = {};
                var cat = categories[i];
                obj.name = cat.name;
                
                obj.categories = [];
                var sub = cat.categories;
                var sublen = sub.length;
                for (var j = 0; j < sublen; j++) {
                    var item = sub[j];
                    obj.categories.push(item.name);

                }
                
                cats.push(obj);
            }

            this.addElements(cats);
        },

        addElements:function(data)
        {
            var len = data.length;
            for(var i=0;i<len;i++)
            {
                var item = data[i];
              
                var div="<div class='cat-div'><h3>" + item.name + "</h3><ul>";
                var cats = item.categories;
                var clen = cats.length;
                for (var j = 0; j < clen; j++)
                {
                    var sub = cats[j];
                    div += "<li>" + sub + "</li>";
                }

                div += "</ul></div>";
                $(this.el).find('.holder').append(div);
            }

            
            
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