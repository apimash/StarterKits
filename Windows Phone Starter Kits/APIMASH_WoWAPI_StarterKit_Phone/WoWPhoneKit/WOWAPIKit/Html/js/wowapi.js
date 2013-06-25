/*

File: wowapi.js
Author: David Isbitski - @TheDaveDev, david.isbitski@microsoft.com, http://davedev.net
Last Mod: 6/25/2013
Description: Helper JavaScript library for working with the World of Warcraft open APIs (http://blizzard.github.io/api-wow-docs/).
             Many thanks to Blizzard for creating the BEST MMO on the market and for caring about the community around
             it!

Latest Source: http://github.com/disbitski/win8wowapikit or http://github.com/apimash/StarterKits

*/

"use strict";

function getRealmStatusAll() {

    $.getJSON("http://us.battle.net/api/wow/realm/status?jsonp=?", function (data) {
        $.each(data.realms, function (i, item) {

            //Add some padding between the server name cells
            var bgPadding = 'padding-top: 30px;padding-bottom: 30px;';

            //Check status of realm and convert background to green for online and red for offline
            var bgStyle = item.status === true ? 'background-color: green;' : 'background-color: red;';

            //Set up div style and show server text
            var divText = "<div style='" + bgPadding + bgStyle + "'><p>" + item.name + "<p></div><p>";

            //Add new div to main divStatus div
            $(divText).appendTo("#divStatus");
        });
    });
}

function onLoad() {
    //tmp until buttons included
    getRealmStatusAll();
}
