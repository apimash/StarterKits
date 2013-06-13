/*
File: apimashtumblr.js
Author: Stacey Mulcahy- @bitchwhocodes, stacey.mulcahy@microsoft.com
Last Mod: 5/1/2013
Description: Basic tumblr api access - does a search against tags
*/
(function () {
    
    WinJS.Namespace.define("API", {
        TumblrApi : WinJS.Class.define(function (apiKey) {
            this._apiKey    = apiKey;
            this._limit     = 20;
            this._endPoint  = 'http://api.tumblr.com/v2/tagged?';
            this._startDate = new Date();
            this._tags      = ['lol', 'gif'];
        }, {
            getCacheData: function () {

                var app = WinJS.Application;
                var that = this;
                return app.local.exists('tumblr-cache.json').then(function (exists) {
                    if (exists)
                        return app.local.readText('tumblr-cache.json').then(function (data) {
                            return that.handleResponse(data);
                        });
                    else return null;
                });

            },

            cacheData: function (data) {
                WinJS.Application.local.writeText('tumblr-cache.json', data);
            },
            handleResponse: function (response) {

                var json        = JSON.parse(response);
                var items = json['response'];
                this.cacheData(response);
                this._startDate = new Date( parseInt(items[items.length-1]['timestamp']));
                var arr = this.formatResults(items);
                return (arr);
            },
            handleError:function(error) {
                return this.getCacheData();
            },
            getTags: function (tags,resultLimit,beginDate) {
                var that = this;
                var tagsList    = (tags) ? tags.join("+") : this._tags.join("+");
                var limit       = (resultLimit) ? resultLimit : this._limit;
                
                this._startDate = (beginDate) ? beginDate : this._startDate;
                
                var date        = this._startDate.getTime() - 60 * 60000;
                var uri         = this._endPoint + 'tag=' + tagsList + "&limit=" + limit + "&api_key=" + this._apiKey + "&beforeDate=" + date;
             
              

                return (WinJS.xhr({ url: uri, responseType: "json" }).then(function (response) {
                    var arr = that.handleResponse(response.responseText);
                    return (arr);
                   // sm not sure the best way to do this
                }, function(error) {
                    return (that.handleError(error));
                }));
            },
            
            resizeImage: function(item,maxWidth,maxHeight) {

                var srcWidth = item.getWidth();
                var srcHeight = item.getHeight();
                var resizeWidth = srcWidth;
                var resizeHeight = srcHeight;
                var aspect = resizeWidth / resizeHeight;
            
                if (resizeWidth < 250) {
            
                    resizeWidth = 300;
               
                    resizeHeight = resizeWidth / aspect;
                }

                if (resizeWidth > maxWidth)
                    {
                        resizeWidth = maxWidth;
                        resizeHeight = resizeWidth / aspect;
                    }
                    if (resizeHeight > maxHeight)
                    {
                        aspect = resizeWidth / resizeHeight;
                        resizeHeight = maxHeight;
                        resizeWidth = resizeHeight * aspect;
                    }

                    item.setWidth(resizeWidth);
                    item.setHeight(resizeHeight);

                return item;

            },
            
            
            formatResults: function (results) {
              
                var len         = results.length;
                var that        = this;
                var resultArray = [];
                
                for (var i = 0; i < len; i++) {
                    if (results[i]['photos']) {
                        var photolist   = results[i]['photos'];
                        var singleItem  = results[i];
             
                        var plen = photolist.length;
                        // Short cut methods for the Original size images
                        for (var j = 0; j < plen; j++) {
                            var item = photolist[j];
                            // copy over all the stuff here in case you want it. 
                            for (var x in singleItem) {
                                    item[x] = singleItem[x];
                            }
                            
                            // Shortcut methods
                            item.getImage = function() {
                                return (this['original_size']['url']);
                            };
                            item.getHeight = function() {

                                return (this['original_size']['height']);
                            };
                            item.getWidth = function() {
                                return (this['original_size']['width']);
                            };
    
                            item.setHeight = function(val) {

                                this['original_size']['height']=val;
                            };
                            item.setWidth = function(val) {
                                this['original_size']['width']=val;
                            };
                            
                            item = this.resizeImage(item, 600, 400);
                            
                            resultArray.push(item);
                           
                        }
                    }
                   
                }
                return (resultArray);
            },
           
        }) 
    });
})();