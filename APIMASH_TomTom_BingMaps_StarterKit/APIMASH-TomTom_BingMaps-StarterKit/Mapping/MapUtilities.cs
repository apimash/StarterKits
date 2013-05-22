using APIMASH;
using Bing.Maps;
using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_StarterKit.Mapping
{
    /// <summary>
    /// Interface implemented for custom map push pins
    /// </summary>
    public interface IAnchorable
    {
        /// <summary>
        /// Offset into graphic element that defines how the object should be overlaid on the target map location 
        /// </summary>
        Point AnchorPoint { get; }
    }

    /// <summary>
    /// Helper class for comment map functionality
    /// </summary>
    public static class MapUtilities
    {
        /// <summary>
        /// Return the layer of map containing point-of-interest pins
        /// </summary>
        /// <param name="m">Map</param>
        /// <returns>Map layer containing point-of-interest pins</returns>
        public static MapLayer PoiLayer(Map m)
        {
            if (m == null) return null;

            // this assumes there is only one MapLayer and it contains the pins
            return m.Children.OfType<MapLayer>().FirstOrDefault();
        }

        public static void ClearPointOfInterestPins(Map m)
        {
            MapLayer poiLayer = PoiLayer(m);
            if (poiLayer != null) poiLayer.Children.Clear();
        }

        public static void RemovePointOfInterestPin(Map m)
        {
            MapLayer poiLayer = PoiLayer(m);
            if (poiLayer != null) poiLayer.Children.Clear();
        }

        public static void AddPointOfInterestPin(this PointOfInterestPin p, Map m, Location l)
        {
            MapLayer poiLayer = PoiLayer(m);
            if (poiLayer == null)
            {
                poiLayer = new MapLayer();
                m.Children.Add(poiLayer);
            }

            MapLayer.SetPositionAnchor(p, p.AnchorPoint);

            MapLayer.SetPosition(p, l);
            poiLayer.Children.Add(p);
        }

        public static void HighlighPointOfInterestPin(Map m, IMappable item, Boolean highlight)
        {
            MapLayer poiLayer = PoiLayer(m);
            if ((poiLayer != null) && (item != null))
            {
                // search for pin in the layer matching by id - some defensive programming here to not assume everything in this layer is a 
                // point-of-interest pin (but it should be!)
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

                    // if it's highlighted bring to top by removing and adding back in
                    if (highlight)
                    {
                        poiLayer.Children.Remove(poiPin);
                        poiLayer.Children.Add(poiPin);
                    }
                }
            }
        }

        public static void SetLocation(this CurrentLocationPin p, Map m, Location l)
        {
            MapLayer.SetPositionAnchor(p, p.AnchorPoint);
            MapLayer.SetPosition(p, l);

            if (!m.Children.Contains(p))
                m.Children.Add(p);

            p.Visibility = Visibility.Visible;
        }
    }
}
