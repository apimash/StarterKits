using System;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH
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
        /// Latitude associated with item
        /// </summary>
        Double Latitude {get; }

        /// <summary>
        /// Longitude associated with item
        /// </summary>
        Double Longitude { get;  }
    }
}
