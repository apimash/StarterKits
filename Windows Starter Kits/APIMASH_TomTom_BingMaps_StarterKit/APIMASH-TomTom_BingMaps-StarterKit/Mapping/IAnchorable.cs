using Windows.Foundation;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
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
}
