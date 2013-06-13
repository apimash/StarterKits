// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF 
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A 
// PARTICULAR PURPOSE. 
// 
// Copyright (c) Microsoft Corporation. All rights reserved 
/*******************************************************************************
* Author: Richard Brundritt
* Website: http://rbrundritt.wordpress.com
* Date: February 9th, 2011
* 
* Description: 
* This JavaScript file is meant to create a client side clustering alogrithm that 
* has high performance, is reusable, and easy to extend. This method takes in a map 
* reference, and a set of options. 
* 
* Example Usage:
*
* var clusteredLayer;
* var data = [];
*
* function GetMap()
* {
*	var map = new Microsoft.Maps.Map(document.getElementById("myMap"),{ credentials: "YOUR_BING_MAPS_KEY" });
*	
*	clusteredLayer = new ClusteredEntityCollection(map, {
*	            singlePinCallback: createPin,
*	            clusteredPinCallback: createClusteredPin
*	        });
* }
*
* //Have data be returned from data source and added to the cluster layer
* function ClusterLayer(results)
* {
* 	clusteredLayer.SetData(results);
* }
*
* function createPin(data)
* {
*	var pin = new Microsoft.Maps.Pushpin(data._LatLong);
*
*   //Add custom logic
*
*   return pin;
* }
*
* function createClusteredPin(data, latlong)
* {
*	var pin = new Microsoft.Maps.Pushpin(latLong, { text: '+' });
*
*	//Add custom logic
*
*	return pin;
* }
********************************************************************************/

/**
* An enumerator of the different ways to display a clustered pushpin.
*/
var ClusterPlacementTypes = {

    //Mean Average placement calculates the average position of a group of coordinates.
    //This method creates a more realistic representation of the data, however requires 
    //more processing power and increase the chances of pushpins overlapping	
    MeanAverage: 0,

    //This method is one of the simplest methods and works fast although it may not 
    //accurately represent the data.
    GridCenter: 1,

    //This method is the simplest way to represent a cluster. It places the cluster at 
    //the first location in the cluster. This method may not accurately represent the 
    //data but requires little processing power.
    FirstLocation: 2
};

/* ClusteredEntityCollection object */
var ClusteredEntityCollection = function (map, options) {
    /* Private Properties */
    var _map = map,
        _layer,
        _data = [],
        _numXCells, 
        _numYCells;

    var _options = {
        //The size of the grid cells for clustering in pixels
        gridSize: 45,

        //The cluster placement type
        clusterPlacementType: ClusterPlacementTypes.MeanAverage,

        //The offset of the layer. Only works when the clusterPlacementType is center
        layerOffset: new Microsoft.Maps.Point(0, 0),

        //flag that indicates if clustering is enabled or not.
        clusteringEnabled: true,

        //Default functionality to create a single pushpin
        singlePinCallback: function (data) {
            return new Microsoft.Maps.Pushpin(data._LatLong);
        },

        //Default function to generate clustered pushpin
        clusteredPinCallback: function (data) {
            var pin = new Microsoft.Maps.Pushpin(data._LatLong, { text: '+' });
            pin.description = cluster.length + " locations<br/>Zoom in for more details.";
            return pin;
        },
        
        //Callback function that gets fired after clustering has been completed. 
        callback: null
    };

    /* Private Methods */

    function _init() {
        //Create an instance of an EntityCollection
        _layer = new Microsoft.Maps.EntityCollection();
        _map.entities.push(_layer);

        setOptions(options);
        Microsoft.Maps.Events.addHandler(_map, 'viewchangeend', cluster);
    }

    //Calculates the LatLong position of a clustered pin using the specified Cluster Placement Type
    function calculateClusteredLatlong(cluster, key) {
        switch (_options.clusterPlacementType) {
            case ClusterPlacementTypes.MeanAverage:
                var lat = 0, lon = 0;

                var i = cluster.length - 1;
                if (i >= 0) {
                    do {
                        lat += cluster[i]._LatLong.latitude;
                        lon += cluster[i]._LatLong.longitude;
                    }
                    while (i--);
                }

                return new Microsoft.Maps.Location(lat / cluster.length, lon / cluster.length);
            case ClusterPlacementTypes.GridCenter:
                var x = ((key % _numXCells) + 0.5) * _options.gridSize + _options.layerOffset.x;
                var y = (Math.floor(key / _numXCells) + 0.5) * _options.gridSize + _options.layerOffset.y;

                return _map.tryPixelToLocation(new Microsoft.Maps.Point(x, y), Microsoft.Maps.PixelReference.control);
            default:
            case ClusterPlacementTypes.FirstLocation:
                return cluster[0]._LatLong;
        }
    }

    // Clusters the data and displays it on the map.
    function cluster() {
        //remove all pins from the layer
        _layer.clear();

        if (_data != null) {
            //Calculate the size of the map in pixels
            //This should always be done so that the map can be resizable
            var mapWidth = _map.getWidth();
            var mapHeight = _map.getHeight();

            //break the map into a grid
            _numXCells = parseInt(Math.ceil(mapWidth / _options.gridSize));
            _numYCells = parseInt(Math.ceil(mapHeight / _options.gridSize));

            var i = _data.length - 1,
                    pixel, k, j, key, pin;

            if (_options.clusteringEnabled) {

                //create an array to store all the grid data.
                var gridCells = new Array(_numXCells * _numYCells);

                //Itirate through the data
                if (i >= 0) {
                    do {
                        pixel = _map.tryLocationToPixel(_data[i]._LatLong, Microsoft.Maps.PixelReference.control);

                        //check to see if the pin is within the bounds of the viewable map
                        if (pixel != null && pixel.x <= mapWidth && pixel.y <= mapHeight && pixel.x >= 0 && pixel.y >= 0) {
                            //calculate the grid position on the map of where the location is located
                            k = Math.floor(pixel.x / _options.gridSize);
                            j = Math.floor(pixel.y / _options.gridSize);

                            //calculates the grid location in the array
                            key = k + j * _numXCells;

                            if (gridCells[key] == null) {
                                gridCells[key] = [];
                            }

                            gridCells[key].push(_data[i]);
                            _data[i].GridKey = key;
                        }
                        else {
                            _data[i].GridKey = -1;
                        }
                    }
                    while (i--);
                }

                //Iteriates through the data and pull out the clusters
                i = gridCells.length - 1;
                if (i >= 0) {
                    do {
                        if (gridCells[i] != null) {

                            //check to see if the a grid contains more than one point of data
                            //using switch statement as it is faster than an if statement
                            switch (gridCells[i].length) {
                                case 1:
                                    pin = _options.singlePinCallback(gridCells[i][0]);
                                    pin.isClustered = false;
                                    break;
                                default:
                                    var latlong = calculateClusteredLatlong(gridCells[i], i);
                                    pin = _options.clusteredPinCallback(gridCells[i], latlong);
                                    pin.isClustered = true;
                                    break;
                            }

                            pin.GridKey = i;
                            _layer.push(pin);
                        }
                    }
                    while (i--);
                }
            } else { //clustering is disabled. Display all locations that are in the current map view.
                //Itirate through the data
                if (i >= 0) {
                    do {
                        pixel = _map.tryLocationToPixel(_data[i]._LatLong, Microsoft.Maps.PixelReference.control);

                        //check to see if the pin is within the bounds of the viewable map
                        if (pixel != null && pixel.x <= mapWidth && pixel.y <= mapHeight && pixel.x >= 0 && pixel.y >= 0) {
                            //Give each pin a unique grid key
                            _data[i].GridKey = i;

                            pin = _options.singlePinCallback(_data[i]);
                            pin.GridKey = i;
                            pin.isClustered = false;
                            _layer.push(pin);
                        }
                        else {
                            _data[i].GridKey = -1;
                        }
                    }
                    while (i--);
                }
            }
        }

        //Call users callback
        if (_options.callback) {
            _options.callback();
        }
    }

    //Updates the default options with new options
    function setOptions(options) {
        for (attrname in options) {
            _options[attrname] = options[attrname];
        }

        cluster();
    }

    /* Public Methods */

    //Layer Methods

    /**
    * @returns A reference to the layer being used.
    */
    this.GetLayer = function () {
        return _layer;
    };

    /**
    * @returns The clustering options.
    */
    this.GetOptions = function () {
        return _options;
    };

    /**
    * Sets the clustering options.
    * Example: clusterLayer.SetOptions({ gridSize : 30});
    */
    this.SetOptions = function (options) {
        setOptions(options);
    };

    /**
    * Sets all layer's z-index to 0 and increases the z-index of the current layer to 1.
    */
    this.BringLayerToFront = function () {
        var i = _map.entities.getLength() - 1,
            entity;

        if (i >= 0) {
            do {
                entity = _map.entities.get(i);
                if (entity.clear != null) {//Only entity collections have the clear function
                    entity.setOptions({ zIndex: 0 });
                }
            }
            while (i--);

            i = _map.entities.indexOf(_layer);
            entity = _map.entities.get(i);
            entity.setOptions({ zIndex: 1 });
        }
    };

    //Data Handlers

    /**
    * Sets the data that is to be clustered and displayed on the map. All objects 
    * must at minimium have a Latitude and Longitude properties. 
    * The algorithm will convert them to a Location object when loading in data.
    * @param {[object]} data - An array of objects that are to be mapped. 
    */
    this.SetData = function (data) {
        if (data != null) {
            _data = data;

            var i = _data.length - 1;

            if (i > 0) {
                do {
                    //convert the data coordinate into a Location object and store it.
                    _data[i]._LatLong = new Microsoft.Maps.Location(_data[i].Latitude, _data[i].Longitude);
                } while (i--)
            }
        } else {
            _data = [];
        }

        cluster();
    };

    /**
    * @returns The data array and returns it to the user. 
    */
    this.GetData = function () {
        return _data;
    };

    /**
    * @returns All the data that is currently displayed.
    */
    this.GetDisplayedData = function () {
        var result = [];

        var i = _data.length - 1;
        if (i >= 0) {
            do {
                if (_data[i].GridKey != -1) {
                    result.push(_data[i]);
                }
            }
            while (i--);
        }

        return result.reverse();
    };

    /**
    * @param {int} key - An intger value that represents a valid grid cell key. 
    * @param {bool} isClustered - An boolean value that indicates if the data is clustered or not.(optional)  
    * @returns An array of data that is in a particular grid cell. A grid key is used to determine this.
    * Optionally a boolean can be passed to indicate if the data is clustered or not. This would be known 
    * if retrieving the data based on a reference from its associated shape. 
    */
    this.GetDataByGridKey = function (key, isClustered) {
        var result = [];

        var i = _data.length - 1;
        if (i >= 0) {
            if (isClustered) {
                do {
                    if (key == _data[i].GridKey) {
                        result.push(_data[i]);
                    }
                }
                while (i--);
            }
            else {
                do {
                    if (key == _data[i].GridKey) {
                        result.push(_data[i]);
                        break;
                    }
                }
                while (i--);
            }
        }

        return result.reverse();
    };

    /**
    * @param {int} key - An intger value that represents a valid grid cell key. 
    * @returns A Pushpin object that is in a grid cell. A grid key is used to determine this.
    */
    this.GetPinByGridKey = function (key) {
        var i = _layer.getLength() - 1,
            pin;

        if (i >= 0) {
            do {
                pin = _layer.get(i);

                if (pin.GridKey == key) {
                    return pin;
                }
            }
            while (i--);
        }

        return null;
    };

    //Initialize class
    _init();
};
Microsoft.Maps.moduleLoaded('clusterModule');