/*
 * LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
 */

(function () {
    "use strict";

    var msMaps = Microsoft.Maps;
    var map, item, searchResults;

    WinJS.UI.Pages.define("/pages/itemDetail/itemDetail.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            item = options && options.item ? MeetupData.resolveItemReference(options.item) : MeetupData.items.getAt(0);
            element.querySelector(".titlearea .pagetitle").textContent = "Coffee near your Meetup";
            element.querySelector("article .item-title").textContent = item.name;
            element.querySelector("article .item-subtitle").textContent = item.cityState;
            element.querySelector("article .item-content").innerHTML = item.description;
            element.querySelector(".content").focus();

            msMaps.loadModule('Microsoft.Maps.Map', { callback: GetMap });

        }
    });

    function GetMap() {
        var mapOptions =
        {
            credentials: Common.bingMapsKey
        }

        map = new msMaps.Map(document.getElementById("myMap"), mapOptions);

        var center = new msMaps.Location(item.venue.lat, item.venue.lon);

        map.entities.push(new msMaps.Pushpin(center));
        map.setView({center: center, zoom: 12});

        msMaps.loadModule('Microsoft.Maps.Search', { callback: searchLoaded });
    }

    function searchLoaded() {
        var searchManager = new msMaps.Search.SearchManager(map);

        var address = item.venue.address_1 + ", " + item.venue.city + ", " + item.venue.state;

        var searchObj = { what: "coffee", where: address, count: 20, callback: searchComplete }

        searchManager.search(searchObj);
    }

    function searchComplete(response, userData) {
        var a = 0;
        var list, lv;
        searchResults = new Array();
        response.searchResults.forEach(function (result) {
            a++;
            result.index = a.toString();
            result.dirlink = "bingmaps:?where=" + result.address + ", " + result.city + ", " + result.state;
            searchResults.push(result);
            var ppOpts =
            {
                icon: "../../images/pushpins/black_pin.png",
                text: a.toString()
            }
            var pp = new msMaps.Pushpin(result.location, ppOpts);

            map.entities.push(pp);
        });

        list = new WinJS.Binding.List(searchResults);
        lv = document.getElementById("searchResults").winControl;
        lv.itemTemplate = document.querySelector(".itemtemplate");
        lv.itemDataSource = list.dataSource;
    }

})();
