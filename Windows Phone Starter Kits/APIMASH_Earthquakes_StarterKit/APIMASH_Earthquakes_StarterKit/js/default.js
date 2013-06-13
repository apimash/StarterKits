/* LICENSE: http://opensource.org/licenses/ms-pl) */

/*
bing maps account management:
https://www.bingmapsportal.com/
*/
var bingMapsKey = "[YOUR-DEV-KEY-HERE]";

/*
earthquake api documentation:
http://earthquake.usgs.gov/earthquakes/feed/
*/

(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var map;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll());
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    window.onload = function () {
        Microsoft.Maps.loadModule('Microsoft.Maps.Map', { callback: initMap, homeRegion: 'US', culture: 'en-us' });
    };

    function initMap() {
        try {
            var mapOptions =
            {
                credentials: bingMapsKey,
                center: new Microsoft.Maps.Location(35, -80.00),
                mapTypeId: Microsoft.Maps.MapTypeId.road,
                zoom: 4         
            };
            map = new Microsoft.Maps.Map(document.getElementById("mapdiv"), mapOptions);
            map.setView({ center: new Microsoft.Maps.Location(35, -80), zoom: 4 });
        }
        catch (e) {
            var md = new Windows.UI.Popups.MessageDialog(e.message);
            md.showAsync();
        }

        WinJS.xhr({ url: "http://earthquake.usgs.gov/earthquakes/feed/v0.1/summary/2.5_day.geojson" }).then(
          function (response) {
              var json = JSON.parse(response.responseText);
              processResults(json);
          },
          function (error) {
              console.log("Can't connect: " + error)
          }
     )
    }

    function processResults(json) {
        var featureList = json.features;
        featureList.forEach(function (item) {
            //to access a property in the JSON, you do something like:
            //var something = item.geometry.coordinates[0]; //longitude
            addPin(item);
        });
    }

    function addPin(item) {
        var loc = new Microsoft.Maps.Location(item.geometry.coordinates[1], item.geometry.coordinates[0]);
        var pushpin = new Microsoft.Maps.Pushpin(loc, null);
        map.entities.push(pushpin);
    }

    app.start();
})();
