using System;

//
// LICENSE: http://opensource.org/licenses/ms-pl
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
        /// Position associated with item
        /// </summary>
        LatLong Position {get; }
    }
}
