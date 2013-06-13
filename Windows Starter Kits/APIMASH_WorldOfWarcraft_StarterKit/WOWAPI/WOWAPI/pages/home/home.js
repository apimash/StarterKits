/*
File: home.js
Author: David Isbitski - @TheDaveDev, david.isbitski@microsoft.com, http://about.me/davedev
Last Mod: 5/1/2013
Description: Main Menu for App to select API function

*/

(function () {
    "use strict";

    var app = WinJS.Application;

    WinJS.UI.Pages.define("/pages/home/home.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {

            //Events
            document.getElementById("realmstatus").addEventListener("click", function () {
                WinJS.Navigation.navigate("/pages/realmstatus/realmstatus.html");
            }, false);

        }
    });
})();
