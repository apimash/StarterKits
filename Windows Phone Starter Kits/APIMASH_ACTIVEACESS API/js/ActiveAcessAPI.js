
/*
File: Active AccessAPI.js 
Author:  Maria Naggaga - @LadyNaggaga  code for key authentication  was written by dev.windows.com  working with the ListViews 
http://code.msdn.microsoft.com/windowsapps/ListView-custom-data-4dcfb128/sourcecode?fileId=50893&pathId=1185249808
Last updated : May 2013
 
 Active acces Starter Kit. Get New York Moving 
 This implements a datasource that will fetch Activities from the Active Access search API. 
 The Active Access API r requires the developer to sigup for a developers key at http://developer.active.com/
 From details on how to Active Acess API use please visit the below 
 http://developer.active.com/apis
*/
/* LICENSE: http://opensource.org/licenses/ms-pl) */

(function ()
{

    // Definition of the data adapter
    var activeAcessSearchDataAdapter = WinJS.Class.define(
        function (devkey, query)
        {

            // Constructor
            this._minPageSize = 50;
            this._maxPageSize = 50;
            this._maxCount = 1000;
            this._devkey = devkey;
            this._query = query;
        },

        // Data Adapter interface methods
        // These define the contract between the virtualized datasource and the data adapter.
        // These methods will be called by virtualized datasource to fetch items, count etc.
        {
            // This example only implements the itemsFromIndex and count methods

            // Called to get a count of the items
            // The value of the count can be updated later in the response to itemsFromIndex
            getCount: function ()
            {
                var that = this;

                // As the Active Access API does not return a count at this time, and the queries invariably return
                // large datasets, we'll check the results and then assume its maxcount if the result set is full

                // Build up a URL to request 50 items so we can get the count if less than that
                var requestStr = "http://api.amp.active.com/search/"
                    + "?v=json"
                    + "&k='" + that._query + "'"
                    + "&l=New York City'" // this can also be a zip code , latitude or longitude
                    + "&r=50" // radius number consider using this search content in the future 
                    + "&m=''"// metadata the ultimate search.  Split up and make into drop boxes natural language applies, date time 
                    + "&f='actvities'"// other options are result, trainining, and articles. Consider making an app focused on each one
                    + "&s='relevance'" //other options include date_asc, dat_desc switch this option to a drop down box
                    + "&num=25"
                    + "&page=50"
                    //+ "&api_key=";
                    +"&api_key="+that._devkey+"";// change key
                  

                //Return the promise from making an XMLHttpRequest to the server
                // The Active Access API authenticates using any username and the developer key as the password.
                return WinJS.xhr({ url: requestStr}).then(

                    //Callback for success
                    function (request)
                    {
                        var obj = JSON.parse(request.responseText);

                        // Verify if the service has returned activities
                        if (obj.numberOfResults > 0)
                        {
                            var count = obj.numberOfResults > that._maxCount ? that._maxCount:obj.numberOfResults;
                            if (count == 0) { WinJS.log && WinJS.log("The search returned 0 results.", "sample", "error"); }
                            return count;
                        } else
                        {
                            WinJS.log && WinJS.log("Error fetching results from the Active Acces", "sample", "error");
                            return 0;
                        }
                    },
                    // Called if the XHR fails
                     function (request)
                     {
                         if (request.status === 400) // make sure to check API documentation to figure out what error handling is needed
                         {
                             WinJS.log && WinJS.log(request.statusText, "sample", "error");
                         } else
                         {
                             WinJS.log && WinJS.log("Error fetching data from the service. " + request.responseText, "sample", "error");
                         }
                         return 0;
                     });
            },

            // Called by the virtualized datasource to fetch items
            // It will request a specific item and optionally ask for a number of items on either side of the requested item. 
            // The implementation should return the specific item and, in addition, can choose to return a range of items on either 
            // side of the requested index. The number of extra items returned by the implementation can be more or less than the number requested.
            //
            // Must return back an object containing fields:
            //   items: The array of items of the form items=[{ key: key1, data : { field1: value, field2: value, ... }}, { key: key2, data : {...}}, ...];
            //   offset: The offset into the array for the requested item
            //   totalCount: (optional) update the value of the count
            itemsFromIndex: function (requestIndex, countBefore, countAfter)
            {
                var that = this;
                if (requestIndex >= that._maxCount)
                {
                    return WinJS.Promise.wrapError(new WinJS.ErrorFromName(WinJS.UI.FetchError.doesNotExist));
                }

                var fetchSize, fetchIndex;
                // See which side of the requestIndex is the overlap
                if (countBefore > countAfter)
                {
                    //Limit the overlap
                    countAfter = Math.min(countAfter, 10);
                    //Bound the request size based on the minimum and maximum sizes
                    var fetchBefore = Math.max(Math.min(countBefore, that._maxPageSize - (countAfter + 1)), that._minPageSize - (countAfter + 1));
                    fetchSize = fetchBefore + countAfter + 1;
                    fetchIndex = requestIndex - fetchBefore;
                } else
                {
                    countBefore = Math.min(countBefore, 10);
                    var fetchAfter = Math.max(Math.min(countAfter, that._maxPageSize - (countBefore + 1)), that._minPageSize - (countBefore + 1));
                    fetchSize = countBefore + fetchAfter + 1;
                    fetchIndex = requestIndex - countBefore;
                }

                // Build up a URL for the request and begin to fetch the index
                var requestStr = "http://api.amp.active.com/search/"

                    // Common request fields (required)
                    + "?v=json"
                    + "&k='" + that._query + "'"
                    + "&l=New York City'" // this can also be a zip code , latitude or longitude
                    + "&r=50" // radius number consider using this search content in the future 
                    + "&m=''"// metadata the ultimate search.  Split up and make into drop boxes natural language applies, date time 
                    + "&f='actvities'"// other options are result, trainining, and articles. Consider making an app focused on each one
                    + "&s='date_asc'" //other options include date_asc, dat_desc switch this option to a drop down box
                    + "&num=" + fetchSize
                    + "&page=" + fetchIndex
                    +"&api_key=" + that._devkey + "";


                // Return the promise from making an XMLHttpRequest to the server
                // The ActiveAccess API authenticates using any username and the developer key as the password.
                return WinJS.xhr({ url: requestStr}).then(

                    //Callback for success
                    function (request)
                    {
                        var results = [], count;

                        // Use the JSON parser on the results, safer than eval
                        var obj = JSON.parse(request.responseText);

                        // Verify if the service has returned activities
                        if (obj.numberOfResults)
                        {
                            var items = obj._results;

                            // Data adapter results needs an array of items of the shape:
                            // items =[{ key: key1, data : { field1: value, field2: value, ... }}, { key: key2, data : {...}}, ...];
                            // Form the array of results objects
                            for (var i = 0, itemsLength = items.length; i < itemsLength; i++)
                            {
                                var dataItem = items[i];
                                results.push({
                                    key: (fetchIndex + i).toString(),
                                    data: {
                                        title: dataItem.title,
                                        thumbnail: dataItem.meta.image1,
                                        width: dataItem.Width,
                                        height: dataItem.Height,
                                        linkurl: dataItem.SourceUrl,
                                        url: dataItem.MediaUrl,
                                        description : dataItem.meta.description
                                    }
                                });
                            }

                            WinJS.log && WinJS.log("", "sample", "status");
                            return {
                                items: results, // The array of items
                                offset: requestIndex - fetchIndex, // The offset into the array for the requested item
                            };

                        } else
                        {
                            WinJS.log && WinJS.log(request.statusText, "sample", "error");
                            return WinJS.Promise.wrapError(new WinJS.ErrorFromName(WinJS.UI.FetchError.doesNotExist));
                        }
                    },

                    //Called on an error from the XHR Object
                    function (request)
                    {
                        if (request.status === 400)
                        {
                            WinJS.log && WinJS.log(request.statusText, "sample", "error");
                        } else
                        {
                            WinJS.log && WinJS.log("Error fetching data from the service. " + request.responseText, "sample", "error");
                        }
                        return WinJS.Promise.wrapError(new WinJS.ErrorFromName(WinJS.UI.FetchError.noResponse));
                    });
            }

            // setNotificationHandler: not implemented
            // itemsFromStart: not implemented
            // itemsFromEnd: not implemented
            // itemsFromKey: not implemented
            // itemsFromDescription: not implemented
            // MapitemsonMap: not implemented
        });

    WinJS.Namespace.define("activeAcessSSearchDataSource", {
        datasource: WinJS.Class.derive(WinJS.UI.VirtualizedDataSource, function (devkey, query)
        {
            this._baseDataSourceConstructor(new activeAcessSearchDataAdapter(devkey, query));
        })
    });
})();
