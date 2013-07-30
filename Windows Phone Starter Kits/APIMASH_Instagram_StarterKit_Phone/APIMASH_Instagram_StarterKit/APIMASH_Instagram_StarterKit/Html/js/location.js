(function () {
    "use strict";
    var loc = null;
    var InstagramLocation = {

        el: '',

        ready: function (element, options) {
            // TODO: Initialize the page here.
            var self = this;
            this.el = $(element);
            Instagram.init(InstagramAPIGlobals.clientId);
            

            $('button').click(function (evt) {
                evt.preventDefault();
                self.processLocationSearch();
            })
            var pos = this.getCurrentLocation();
            $('input').val(pos.coordinates.lng + "," + pos.coordinates.lat);


        },

        getLocations: function (lon, lat) {
            var self = this;

            Instagram.getLocations({ 'lng': lon, 'lat': lat })
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

            this.locations = data;
            var self = this;

            var tags = data.data;
            // SM to revamp
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

            ul.find('a').click(function (evt) {
                evt.preventDefault();
                var id = $(evt.target).attr('href');
                $(this.el).find('input').val(id);
                self.getMediaByLocationId(id);

            });
        },

        /*
        Process results for the location search
        */
        processLocationSearch: function () {
            var value = $('input').val();
            var items = value.split(',');
            var lng = parseFloat(items[0]);
            var lat = parseFloat(items[1]);
            this.getMediaInLonLat(lng, lat);
        },

        getMediaInLonLat: function (lng, lat) {
            var self = this;

            Instagram.getMediaInLonLat({ 'lng': lng, 'lat': lat })
                .done(function (data) {
                    self.processData(data);
                })
                .error(function (err) {
                    self.showError(err);
                });
        },

        showError: function (err) {

            $('#msg').text(err.statusText);
        },

        processData: function (data) {
            $('#msg').fadeOut();
            var len = data.data.length;;
            var d = data.data;
            var arr = [];
            for (var i = 0; i < len; i++) {
                var item = d[i];
                arr.push(item);
            }
           this.itemList = myList;
           // document.getElementById('locationListView').winControl.itemDataSource = myList.dataSource;
           // myList.notifyReload();
        },

        getCurrentLocation: function () {
            var pos = {};
            pos.coordinates.latitude = "-44.7";
            pos.coordinates.longitude="71";
            return (pos);
        },


        getPositionHandler: function (pos) {
            var str = "Current Latitude :" + pos.coordinate.latitude;
            str += "</br>";
            str += "Current Longitude " + pos.coordinate.longitude;
            str += "</br>";
            str += "Current accuracy :" + pos.coordinate.accuracy;
            document.getElementById('position').innerHTML = str;
            document.getElementById('status').innerHTML = this.getStatusString(loc.locationStatus);


            document.getElementById('search_fld').value = pos.coordinate.longitude + "," + pos.coordinate.latitude;

            this.getLocations(pos.coordinate.longitude, pos.coordinate.latitude);
        },

        errorHandler: function (e) {

            document.getElementById('search_fld').innerHTML =
                this.getStatusString(loc.locationStatus);
        },

        getStatusString: function (locStatus) {
            return("finding")
        }

       
    };

    InstagramLocation.ready('body');
    return InstagramLocation;
   

})();