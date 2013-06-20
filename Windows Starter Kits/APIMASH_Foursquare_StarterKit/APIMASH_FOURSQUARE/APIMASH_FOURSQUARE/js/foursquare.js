
var Foursquare = function () {
    /*
    Please register a client/app with foursquare ( https://developer.foursquare.com) 
    and put the client id and secret in the apiglobals.js

    NOTE: Didn't want to use require for this, but it would be easy to port it
    and have its required dependancies be Jquery and apiglobals.js
    */
    var init = function (client_id,client_secret) {
        clientId = client_id;// Required
        clientSecret = client_secret;//Required
        version = getVersionString();
        defaultLngLat = '40.7,-74';
    }
    /*The Foursquare api changes. Passing a "v" parameter on the call
      means you are requesting the most recent from ths date. 
      This creates a version string based on todays date
    */
    var getVersionString=function()
    {
        // Foursquare wants the format YYYYMMDD
        var today = new Date()
        var day = today.getDate().toString();
        var month = today.getMonth() + 1;
        month = (month < 10) ? '0' + month.toString() : month.toString();
        var year = today.getFullYear().toString();
        var dateString = year + month + day;
        return(dateString);
    }

    var endPoints = {
        'getVenueDetailById': 'https://api.foursquare.com/v2/venues/',
        'getVenueCategories': 'https://api.foursquare.com/v2/venues/categories',
        'getPopularVenues': 'https://api.foursquare.com/v2/venues/explore',
        'searchVenues': 'https://api.foursquare.com/v2/venues/search',
        'getTrendingVenues': 'https://api.foursquare.com/v2/venues/trending',
    }

    
    var getVenueDetailById = function (venue_id) {
      
        var url = endPoints['getVenueDetailById'] + venue_id;
        return (makeJSONRequest(newUrl));
    }


    var getVenueCategories = function () {
        var url = endPoints['getVenueCategories'];
        return (makeJSONRequest(url));
    }

    /*
    Please refer to https://developer.foursquare.com/docs/venues/explore for the option parameters that can be sent
    */
    var getPopularVenues = function (options) {
    
        var url = endPoints['getPopularVenues'];
        if (!options) {
            options = { 'll': defaultLngLat };
        }
      
        return (makeJSONRequest(url, options));
    }
    //https://developer.foursquare.com/docs/venues/search
    var searchVenues = function (options) {
        // needs to be sorted
        var url = endPoints['searchVenues'];
        if (!options) {
            options = {'ll':defaultLngLat};
        }
        return (makeJSONRequest(url, options));
    }
    var getTrendingVenues = function (options) {
        if (!options) {
            options = { 'll': defaultLngLat };
        }
        var url = endPoints['getTrendingVenues']
        
        return (makeJSONRequest(url, options));
    }
   

    var makeJSONRequest = function (url, data) {
        var self = this;
        if (!data) { data = {} }
      
        data.client_id = clientId;
        data.client_secret = clientSecret;
        data.v = version;
       
        var jqxhr = $.getJSON(url, data);
        
        return (jqxhr);

    }

    return {
        // public properties
        init: init,
        getTrendingVenues: getTrendingVenues,
        searchVenues:searchVenues,
        getPopularVenues: getPopularVenues,
        getVenueCategories:getVenueCategories

    }
}();