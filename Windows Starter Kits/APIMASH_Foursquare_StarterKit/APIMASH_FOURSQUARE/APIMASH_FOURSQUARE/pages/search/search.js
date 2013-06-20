// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";
    var loc = null;
    var publicMembers = {
        itemList: new WinJS.Binding.List(),
        locations: []
    };
    WinJS.Namespace.define("FoursquareSearch", publicMembers);
    WinJS.UI.Pages.define("/pages/search/search.html", {

        el: '',

        ready: function (element, options) {
            // TODO: Initialize the page here.
            var self = this;
            this.el = $(element);
            Foursquare.init(FoursquareAPIGlobals.clientId, FoursquareAPIGlobals.clientSecret);


            $(this.el).find('.search button').click(function (evt) {
                evt.preventDefault();
                self.processSearch();
            })
            this.getCurrentLocation();
        },

        searchVenues: function (ll) {
            var self = this;

            Foursquare.searchVenues({ 'll': ll,'intent':'browse'})
                .done(function (data) {
                    self.processVenues(data);
                })
                .error(function (err) {
                    
                    self.showError(err);

                });
        },


        processSearch: function () {
            var value = $('input').val();
            this.searchVenues(value);
        },



        showError: function (err) {

            $('#msg').text(err.statusText);
        },

        processVenues: function (data) {

            if (!data.response.venues) {
                $('#msg').text('No Venues Found');
                return;
            }
            $('#msg').fadeOut();
            var len = data.response.venues.length;
         
            var d = data.response.venues;
            var myList = new WinJS.Binding.List();
            for (var i = 0; i < len; i++) {
                var item = d[i];
                myList.push(item);
            }
            FoursquareSearch.itemList = myList;
            document.getElementById('searchListView').winControl.itemDataSource = myList.dataSource;
            myList.notifyReload();
        },

        getCurrentLocation: function () {

            var self = this;
            if (loc == null) {
                loc = new Windows.Devices.Geolocation.Geolocator();
                
            }
            if (loc != null) {
                loc.getGeopositionAsync().then(function (pos) { self.getPositionHandler(pos) }, function () { self.errorHandler() });
            }
        },


        getPositionHandler: function (pos) {
            var str = "Current Latitude :" + pos.coordinate.latitude;
            str += "</br>";
            str += "Current Longitude " + pos.coordinate.longitude;
            str += "</br>";
            str += "Current accuracy :" + pos.coordinate.accuracy;
            document.getElementById('position').innerHTML = str;
            document.getElementById('status').innerHTML = this.getStatusString(loc.locationStatus);


            document.getElementById('search_fld').value = pos.coordinate.latitude + "," + pos.coordinate.longitude;


        },

        errorHandler: function (e) {

            document.getElementById('search_fld').innerHTML =
                this.getStatusString(loc.locationStatus);
        },

        getStatusString: function (locStatus) {
            switch (locStatus) {
                case Windows.Devices.Geolocation.PositionStatus.ready:
                    // Location data is available
                    return "Location is available.";
                    break;
                case Windows.Devices.Geolocation.PositionStatus.initializing:
                    // This status indicates that a GPS is still acquiring a fix
                    return "A GPS device is still initializing.";
                    break;
                case Windows.Devices.Geolocation.PositionStatus.noData:
                    // No location data is currently available 
                    return "Data from location services is currently unavailable.";
                    break;
                case Windows.Devices.Geolocation.PositionStatus.disabled:
                    // The app doesn't have permission to access location,
                    // either because location has been turned off.
                    return "Your location is currently turned off. " +
                        "Change your settings through the Settings charm " +
                        " to turn it back on.";
                    break;
                case Windows.Devices.Geolocation.PositionStatus.notInitialized:
                    // This status indicates that the app has not yet requested
                    // location data by calling GetGeolocationAsync() or 
                    // registering an event handler for the positionChanged event. 
                    return "Location status is not initialized because " +
                        "the app has not requested location data.";
                    break;
                case Windows.Devices.Geolocation.PositionStatus.notAvailable:
                    // Location is not available on this version of Windows
                    return "You do not have the required location services " +
                        "present on your system.";
                    break;
                default:
                    break;
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
