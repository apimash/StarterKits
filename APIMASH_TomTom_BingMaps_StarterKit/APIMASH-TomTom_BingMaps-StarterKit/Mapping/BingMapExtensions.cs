using APIMASH.Mapping;
using Bing.Maps;
using System;
using System.Linq;
using Windows.UI.Xaml;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH_StarterKit.Mapping
{
    /// <summary>
    /// Bing Maps map extension methods
    /// </summary>
    public static partial class BingMapsExtensions
    {
        /// <summary>
        /// Gets the layer of map containing point-of-interest pins
        /// </summary>
        /// <returns>Map layer containing point-of-interest pins</returns>
        public static MapLayer PoiLayer(this Map m)
        {
            if (m == null) return null;

            // this assumes there is only one MapLayer and it contains the pins
            return m.Children.OfType<MapLayer>().FirstOrDefault();
        }

        /// <summary>
        /// Removes all point-of-interest pins from the map
        /// </summary>
        public static void ClearPointOfInterestPins(this Map m)
        {
            MapLayer poiLayer = PoiLayer(m);
            if (poiLayer != null) poiLayer.Children.Clear();
        }

        /// <summary>
        /// Adds point-of-interest pin to designated location
        /// </summary>
        /// <param name="pin">Point-of-interest pin instance</param>
        /// <param name="position">Latitude/longitude for pin placement</param>
        public static void AddPointOfInterestPin(this Map m, PointOfInterestPin pin, LatLong position)
        {
            MapLayer poiLayer = PoiLayer(m);
            if (poiLayer == null)
            {
                poiLayer = new MapLayer();
                m.Children.Add(poiLayer);
            }

            MapLayer.SetPositionAnchor(pin, pin.AnchorPoint);

            MapLayer.SetPosition(pin, new Location(position.Latitude, position.Longitude));
            poiLayer.Children.Add(pin);
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
                PointOfInterestPin poiPin = poiLayer.Children.Where((c) =>
                {
                    var p = c as PointOfInterestPin;
                    return ((p != null) && (p.PointOfInterest.Id == item.Id));
                }).FirstOrDefault() as PointOfInterestPin;

                // if pin is found
                if (poiPin != null)
                {
                    // set highlight appropriately
                    poiPin.IsHighlighted = highlight;

                    // if it's highlighted, bring to top by removing and adding back in
                    if (highlight)
                    {
                        poiLayer.Children.Remove(poiPin);
                        poiLayer.Children.Add(poiPin);
                    }
                }
            }
        }

        /// <summary>
        /// Positions current location indicator on map
        /// </summary>
        /// <param name="pin">Current location pin object</param>
        /// <param name="position">Latitude/longitude at which to place pin</param>
        public static void SetCurrentLocationPin(this Map m, CurrentLocationPin pin, LatLong position)
        {
            MapLayer.SetPositionAnchor(pin, pin.AnchorPoint);
            MapLayer.SetPosition(pin, new Location(position.Latitude, position.Longitude));

            if (!m.Children.Contains(pin))
                m.Children.Add(pin);

            pin.Visibility = Visibility.Visible;
        }
    }
}