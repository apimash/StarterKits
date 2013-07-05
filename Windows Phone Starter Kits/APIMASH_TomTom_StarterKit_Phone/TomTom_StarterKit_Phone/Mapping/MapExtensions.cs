using APIMASH.Mapping;
using Microsoft.Phone.Maps.Controls;
using System;
using System.Device.Location;
using System.Linq;
using System.Windows;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace TomTom_StarterKit_Phone.Mapping
{
    /// <summary>
    /// Map extension methods
    /// </summary>
    public static partial class MapExtensions
    {
        /// <summary>
        /// Gets the layer of map containing point-of-interest pins
        /// </summary>
        /// <returns>Map layer containing point-of-interest pins</returns>
        public static MapLayer PoiLayer(this Map m)
        {
           if (m == null) return null;

            // find first map layer with a mapoverlay that contains a PointOfInterestPin object
            return m.Layers.Where(l => l.Where(o => o.Content.GetType() == typeof(PointOfInterestPin)).Count() > 0).FirstOrDefault();
        }

        /// <summary>
        /// Removes all point-of-interest pins from the map
        /// </summary>
        public static void ClearPointOfInterestPins(this Map m)
        {
            // clear all the overlays from the point-of-interest layer
            MapLayer poiLayer = PoiLayer(m);
            if (poiLayer != null) poiLayer.Clear();
        }

        /// <summary>
        /// Adds point-of-interest pin to designated location
        /// </summary>
        /// <param name="pin">Point-of-interest pin instance</param>
        /// <param name="position">Latitude/longitude for pin placement</param>
        public static void AddPointOfInterestPin(this Map m, PointOfInterestPin pin, LatLong position)
        {
            // add a new POI layer if needed
            MapLayer poiLayer = PoiLayer(m);
            if (poiLayer == null)
            {
                poiLayer = new MapLayer();
                m.Layers.Add(poiLayer);
            }

            // add a new overlay for the current pin
            var poiOverlay = new MapOverlay();
            poiOverlay.Content = pin;
            poiOverlay.GeoCoordinate = new GeoCoordinate(position.Latitude, position.Longitude);
            poiOverlay.PositionOrigin = pin.AnchorPoint;

            // add overlay to the layer
            poiLayer.Add(poiOverlay);
        }

        /// <summary>
        /// Hides/shows point-of-interest pin
        /// </summary>
        /// <param name="item">Item associated with the point-of-interest pin</param>
        /// <param name="highlight">Flag indicating whether point-of-interest pin should be highlighted or unhighlighted</param>
        public static void HighlightPointOfInterestPin(this Map m, IMappable item, Boolean highlight)
        {
            MapLayer poiLayer = PoiLayer(m);
            if ((poiLayer != null) && (item != null))
            {
                // search for pin in the layer matching by id - some defensive programming here to not assume everything 
                // in this layer is a point-of-interest pin (but it should be!)
                MapOverlay poiPinOverlay = poiLayer.Where(o => 
                {
                    var p = o.Content as PointOfInterestPin;
                    return ((p != null) && (p.PointOfInterest.Id == item.Id));
                }).FirstOrDefault() as MapOverlay;

                // if pin is found
                if (poiPinOverlay != null)
                {
                    // set highlight appropriately
                    var poiPin = poiPinOverlay.Content as PointOfInterestPin;
                    poiPin.IsHighlighted = highlight;

                    // if it's highlighted, bring to top by removing and adding back in
                    if (highlight)
                    {
                        poiLayer.Remove(poiPinOverlay);
                        poiLayer.Add(poiPinOverlay);
                    }
                }
            }
        }

        /// <summary>
        /// Positions current location indicator on map
        /// </summary>
        /// <param name="pin">Current location pin object</param>
        /// <param name="position">Coordinate at which to place pin</param>
        public static void SetCurrentLocationPin(this Map m, CurrentLocationPin pin, GeoCoordinate position)
        {
            MapOverlay locationOverlay = default(MapOverlay);

            // find first layer that includes a CurrentLocationPin
            var locationLayer = m.Layers.Where(l => l.Where(o => o.Content.GetType() == pin.GetType()).Count() > 0).FirstOrDefault();

            // if there's not an overlay for current location, create it
            if (locationLayer == null)
            {
                // create an overlay
                locationOverlay = new MapOverlay();
                locationOverlay.Content = pin;
                locationOverlay.PositionOrigin = pin.AnchorPoint;

                // create a layer and and add overlay to it
                locationLayer = new MapLayer();
                locationLayer.Add(locationOverlay);

                // add layer to the map
                m.Layers.Add(locationLayer);
            }

            // update the position and visibility of the current location pin
            if (locationOverlay != null)
            {
                locationOverlay.GeoCoordinate = position;
                pin.Visibility = Visibility.Visible;
            }
        }
    }
}