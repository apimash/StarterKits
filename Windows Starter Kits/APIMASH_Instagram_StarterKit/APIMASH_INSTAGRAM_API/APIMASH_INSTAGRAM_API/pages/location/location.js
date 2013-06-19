// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";
    var loc = null;
    var publicMembers ={
        itemList: new WinJS.Binding.List(),
        locations :[]
    };
    WinJS.Namespace.define("InstagramLocation", publicMembers);
    WinJS.UI.Pages.define("/pages/location/location.html", {
        /*
        Location calls are flaky at best. 
        */
        el: '',
        
        ready: function (element, options) {
            // TODO: Initialize the page here.
            var self = this;
            this.el = $(element);
            Instagram.init(InstagramAPIGlobals.clientId);
            if (InstagramLocation.locations.length)
            {
                this.processLocations(InstagramLocation.locations);
            }

            $('button').click(function (evt) {
                evt.preventDefault();
                self.processLocationSearch();
            })
            this.getCurrentLocation();
        },

        getLocations: function (lon,lat) {
            var self = this;

            Instagram.getLocations({'lng':lon, 'lat':lat})
                .done(function (data) {
                    self.processLocations(data);
                })
                .error(function (err) {
                    self.showError(err);
                   
                });
        },

        getMediaByLocationId: function (id) {
            var self = this;
            Instagram.getMediaByLocationId(id)
                .done(function (data) {
                    self.processData(data);
                })
                .error(function (err) {
                    self.showError(err);
                });
        },
        processLocations: function (data) {
            
            InstagramLocation.locations = data;
            var self = this;
         
            var tags = data.data;
            
            $(this.el).find('ul li').remove();
            if (!tags.length) {
                var div = "<li><a href='#' class='a-tag'>No Locations found</a></li>"
                var item = $(this.el).find('ul').append(div);
                return;
            }

            var ul = $(this.el).find('ul');
            for (var i = 0; i < tags.length; i++) {
               
                
                var div = "<li><a href='" + tags[i].id + "' class='a-tag'>" + tags[i].name + " " + tags[i].id + "</a></li>"

                ul.append(div);

               
            }

            ul.find('a').click(function (evt) 
            {
                evt.preventDefault();
                var id = $(evt.target).attr('href');
                $(this.el).find('input').val(id);
                self.getMediaByLocationId(id);

            });
        },

        /*
        Process results for the location search
        */
        processLocationSearch:function()
        {
            var value = $('input').val();
            var items = value.split(',');
            var lng = parseFloat(items[0]);
            var lat = parseFloat(items[1]);
            this.getMediaInLonLat(lng, lat);
        },

        getMediaInLonLat: function (lng, lat) {
            var self = this;
           
            Instagram.getMediaInLonLat({'lng':lng,'lat':lat})
                .done(function (data) {
                    self.processData(data);
                })
                .error(function (err) {
                    self.showError(err);
                });
            },

        showError:function(err)
        {
           
            $('#msg').text(err.statusText);
        },

        processData: function (data) {
            $('#msg').fadeOut();
            var len = data.data.length;;
            var d = data.data;
            var myList = new WinJS.Binding.List();
            for (var i = 0; i < len; i++) {
                var item = d[i];
                myList.push(item);
            }
            InstagramLocation.itemList = myList;
            document.getElementById('locationListView').winControl.itemDataSource = myList.dataSource;
            myList.notifyReload();
        },

        getCurrentLocation:function()
        {

            var self = this;
            if (loc == null) {
                loc = new Windows.Devices.Geolocation.Geolocator();
                console.log(loc);
            }
            if (loc != null) {
                loc.getGeopositionAsync().then(function (pos) { self.getPositionHandler(pos) }, function () { self.errorHandler()});
            }
        },

       
        getPositionHandler: function (pos) {
            var str = "Current Latitude :" + pos.coordinate.latitude;
            str+= "</br>";
            str+= "Current Longitude " + pos.coordinate.longitude;
            str += "</br>";
            str += "Current accuracy :" + pos.coordinate.accuracy;
            document.getElementById('position').innerHTML = str;
            document.getElementById('status').innerHTML = this.getStatusString(loc.locationStatus);
        
     
            document.getElementById('search_fld').value = pos.coordinate.longitude + "," + pos.coordinate.latitude;

            this.getLocations(pos.coordinate.longitude,pos.coordinate.latitude);
    },

   errorHandler:function(e) {        

        document.getElementById('search_fld').innerHTML =
            this.getStatusString(loc.locationStatus);    
    },

  getStatusString:function(locStatus) {
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
