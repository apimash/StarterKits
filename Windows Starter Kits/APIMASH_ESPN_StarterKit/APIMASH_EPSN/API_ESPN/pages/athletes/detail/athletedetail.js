// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";

    WinJS.UI.Pages.define("/pages/athletes/detail/athletedetail.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var el = $(element);
            var content = options.sports[0].leagues[0].athletes[0];
            el.find('h3').text(options.sports[0].leagues[0].athletes[0].displayName);

            var str = '';
            for (var i in content) {
                str += ("<p>" + i + " : " + content[i] + "</p>");
            }

            el.find('.detail-news').append(str);


            // TODO: Initialize the page here.
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
