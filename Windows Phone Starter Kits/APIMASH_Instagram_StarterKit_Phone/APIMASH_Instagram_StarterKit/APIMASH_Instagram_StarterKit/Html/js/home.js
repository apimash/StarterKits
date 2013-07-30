// JavaScript source code
var InstagramHome=(function () {
    "use strict";
    var loc = null;
    var mode = null;
    var InstagramMain= {

        el: '',
        ready: function (element, options) {
            // TODO: Initialize the page here.
            var self = this;
            this.el = $(element);

            $('.main-page-nav a').click(function (evt) {
               
                evt.preventDefault();
                $('.label-results').hide();
                var txt = $(this).text();
                switch (txt)
                {
                    case "Locations":
                        self.mode = "locations";
                        self.showLocations();
                        break;
                    case "Tags":
                        self.mode = "tags";
                        self.showTags();
                        break;
                    case "Popular":
                        self.mode ="popular";
                        self.showPopular();
                        break;
                }
            })

            $('button').click(function (evt) {
                evt.preventDefault();
                $('#loader').fadeIn();
                $('.label-results').fadeIn();
                switch (self.mode) {
                    case "locations":
                        self.processLocationSearch();
                        break;
                    case "tags":
                        self.processTagSearch();
                }

            });


            $('select').change(function (evt) {
                evt.preventDefault();
                var data = $(evt.target).val();
                switch (self.mode) {
                    case "locations":
                        self.getMediaByLocationId(data);
                        break;
                    case "tags":
                        self.getMediaByTag(data);
                }
                
                
            })

            Instagram.init(InstagramAPIGlobals.clientId);

        },
        setDirectionsLabel: function (str)
        {
            $('.label-directions').text(str);
        },

        setResultsLabel: function (str) {
            $('.label-results').text(str);
        },

        showTags: function () {
            var self = this;
            this.showForm();
            this.emptyResults();
            this.setDirectionsLabel("Search for tags");
            this.setResultsLabel("Retrieving tags...");

        },
        showLocations: function () {
            var self = this;
            this.emptyResults();
            this.showForm();
            this.setDirectionsLabel("Search for Locations by lat & lng");
            this.setResultsLabel("Retrieving locations...");
            var pos = this.getCurrentLocation();
            $('input').val(pos.coordinates.longitude + "," + pos.coordinates.latitude);
           

        },
        showPopular: function () {

            this.emptyResults();
            this.hideForm();
            this.getPopularMedia();
        },

        emptyResults: function () {
            $('#results').empty();
        },

        hideForm: function () {
            $('.form-items').hide();
           
        },
        showForm: function () {
            $('.form-items').fadeIn();
            $('select').hide();
            $('input').val('');
         
        },

        /*API CALLS*/
        getPopularMedia: function () {

            var self = this;
            Instagram.getPopularMedia().done(function (data) {
                self.processData(data);
            })
        },
        /*API CALLS*/
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
        /*API CALLS*/
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
            $('select').empty();
            $('#loader').fadeOut();

            this.setResultsLabel("Locations found:");

            var tags = data.data;
         
            var options = $('select');
            
            for (var i = 0; i < tags.length; i++) {
                options.append($("<option />").val(tags[i].id).text(tags[i].name));
            }
            $('select').fadeIn();
        },
        /*API CALLS*/
        getMediaByTag:function(tag)
        {
            var self = this;
            Instagram.getRecentMediaByTag(tag).done(function (data) {
                self.processData(data);
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
            this.getLocations(lng,lat);
       
        },

        processTagSearch:function(){
            var value = $('input').val();
            var self = this;
          
            Instagram.searchTagsByName(value).done(function (data) {
                self.processTagInfo(data);
            })
        },


        processTagInfo:function(data)
        {

            $('#loader').fadeOut();
            this.setResultsLabel("Tags found:");

            var arr = [];
            var d  = data.data;
            var len = d.length;
            for(var i=0;i<len;i++)
            {
                arr.push(d[i].name);
            }

            this.processTags(arr);
        },

        processTags: function (tags) {
         
          
            var self = this;
            $('select').empty();
            $('select').fadeIn();
            if (!tags.length) {
                var div = "<li><a href='#' class='a-tag'>No tags found</a></li>"
                var item = $('#results').append(div);
                return;
            }
            for (var i = 0; i < tags.length; i++)
            {
                var div = "<li><a href='#' class='a-tag'>" + tags[i] + "</a></li>"
                $('select').append($("<option />").val(tags[i]).text(tags[i]));
                
            }
            $('select').fadeIn();
        },


       
        /* API CALLS*/
        getMediaInLonLat: function (lng, lat) {
  
            var self = this;
            Instagram.getMediaInLonLat({ 'lng': lng, 'lat': lat })
                .done(function (data) {
                    
                    self.processLocations(data);
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
            $('#results').empty();
            $('#results').hide();

            if (!d || !len) {
                var div = "<li>No Items found</li>"
                var item = $('#results').append(div);
                return;
            }
           
            for (var i = 0; i < len; i++) {
             
                var html = "<li><img src='" + d[i].images.thumbnail.url + "'><p>"+d[i].user.username+"</p></li>";
                $('#results').append(html);
            }

            $('#results').fadeIn();
     
        },

        getCurrentLocation: function () {
            var pos = {};
            pos.coordinates = {};
           pos.coordinates.latitude = " 40.7482";
           pos.coordinates.longitude = "-73.9068";
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
            return ("finding")
        }


    };

    return InstagramMain;


})();