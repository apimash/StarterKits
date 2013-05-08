/// <reference path="jquery-2.0.0.min.js" />

/*
File: WOWAPI.js
Author: David Isbitski - @TheDaveDev, david.isbitski@microsoft.com, http://about.me/davedev
Last Mod: 5/1/2013
Description: Helper JavaScript library for working with the World of Warcraft open APIs (http://blizzard.github.io/api-wow-docs/).
             Many thanks to Blizzard for creating the BEST MMO on the market and for caring about the community around
             it!

Latest Source: http://github.com/disbitski/win8wowapikit
*/

(function () {
    "use strict";

    var supported_locales = ["en_US", "es_MX", "pt_BR"]; //US Battle Realms 
    var _locale;

    WinJS.Namespace.define("wowapi", {
        realmStatusToColor: WinJS.Binding.converter(function (status) {
            return status == true ? "green" : "red";
        }),

        realmStatusToText: WinJS.Binding.converter(function (status) {
            return status == true ? "UP" : "DOWN";
        }),

        checkLocale: function (locale) {
            return $.inArray(locale, supported_locales) == -1 ? "en_US" : locale;
        },

        getRealmStatusAll: function (locale) {
            _locale = wowapi.checkLocale(locale);
            
            WinJS.xhr({ url: "http://us.battle.net/api/wow/realm/status?locale=" + _locale, responseType: "json" }).then(
            function (response) {
                var json = JSON.parse(response.responseText);
                var serverList = new WinJS.Binding.List(json.realms);
                var lv = document.querySelector("#servers").winControl;
                lv.itemDataSource = serverList.dataSource;
                lv.itemTemplate = document.querySelector("#servertemplate");

                WinJS.UI.processAll();
            },
             function (error) { WinJS.log(error); },
             function (progress) { }
        )},

        getRealmStatus: function (realm, locale) {
            _locale = wowapi.checkLocale(locale);

            WinJS.xhr({ url: "http://us.battle.net/api/wow/realm/status?realm=" + realm + "&locale=" + _locale, responseType: "json" }).then(
            function (response) {
                var json = JSON.parse(response.responseText);
                var serverList = new WinJS.Binding.List(json.realms);
                var lv = document.querySelector("#servers").winControl;
                lv.itemDataSource = serverList.dataSource;
                lv.itemTemplate = document.querySelector("#servertemplate");

                WinJS.UI.processAll();
            },
             function (error) { WinJS.log(error); },
             function (progress) { }
        )}
    })

})();