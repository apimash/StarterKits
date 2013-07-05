using System.Windows;
using System.Windows.Controls;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace TomTom_StarterKit_Phone.Mapping
{
    public sealed partial class CurrentLocationPin : UserControl, IAnchorable
    {
        /// <summary>
        /// Reflects the offset into the graphic that defines the point to be associated with the given location (range is 0 to 1).
        /// </summary>
        public Point AnchorPoint
        {
            //
            // TODO: (optional) if the indicator graphic is changed, update the AnchorPoint to reflect what point in the 
            //       graphic should be anchored to the lat/long in the location.
            //
            get { return new Point(0.5, 0.5); }
        }

        public CurrentLocationPin()
        {
            this.InitializeComponent();
            Visibility = Visibility.Collapsed;
        }
    }
}