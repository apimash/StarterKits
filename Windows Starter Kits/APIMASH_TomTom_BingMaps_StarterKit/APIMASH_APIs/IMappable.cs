using System;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH.Mapping
{
    /// <summary>
    /// Interface implemented by view model types that will be overlayed on map
    /// </summary>
    public interface IMappable
    {
        /// <summary>
        /// Unique id of element (use a GUID if there is no natural key in the data)
        /// </summary>
        String Id { get; }

        /// <summary>
        /// Label to appear in push pin on map (use and appearance dependent on design of PointOfInterestPin)
        /// </summary>
        String Label { get;  }        

        /// <summary>
        /// Latitude/longitude associated with item
        /// </summary>
        LatLong Position {get; }
    }

    /// <summary>
    /// Implementation of IMappable used to populate result suggestions for the Search charm
    /// </summary>
    public class SearchResultSuggestion : IMappable
    {
        /// <summary>
        /// Unique id of item, used as the Tag value in the ResultSuggestionChosen event of the SearchPane
        /// </summary>
        public String Id { get; private set; }

        /// <summary>
        /// Text appearing in search result
        /// </summary>
        public String Label { get; private set; }

        /// <summary>
        /// Latitude/longitude associated with search result
        /// </summary>
        public LatLong Position { get; private set; }

        /// <summary>
        /// Creates a new search result option for use in the SearchPane
        /// </summary>
        /// <param name="label">Text that should appear for item in SearchPane</param>
        /// <param name="latitude">Latitude of the associated point-of-interest</param>
        /// <param name="longitude">Longitude of the associated point-of-interest</param>
        public SearchResultSuggestion(String label, Double latitude, Double longitude)
        {
            Id = Guid.NewGuid().ToString();
            Label = label;
            Position = new LatLong(latitude, longitude);
        }
    }
}