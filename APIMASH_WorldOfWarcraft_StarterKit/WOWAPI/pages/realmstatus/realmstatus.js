/// <reference path="/js/jquery-2.0.0.min.js" />

/*
File: realmstatus.js
Author: David Isbitski - @TheDaveDev, david.isbitski@microsoft.com, http://about.me/davedev
Last Mod: 5/1/2013
Description: Helper JavaScript library for binding WOWAPI Realm Status data to Listview

*/

(function () {
    "use strict";

    var app = WinJS.Application;

    WinJS.UI.Pages.define("/pages/realmstatus/realmstatus.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {


            //Get Realm Status Examples
            //wowapi.getRealmStatus('terenas', app.sessionState.locale);
            wowapi.getRealmStatusAll(app.sessionState.locale);

            //Set share contract info you want here.  Can be specific realms, etc.
            app.sessionState.shareTitle = "realm status ftw!";


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
