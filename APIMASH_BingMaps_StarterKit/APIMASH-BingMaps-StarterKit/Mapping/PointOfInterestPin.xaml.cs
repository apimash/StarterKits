using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_BingMaps_StarterKit.Mapping
{
    /// <summary>
    /// Occurs when point-of-interest pin is selected
    /// </summary>
    public class SelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Unique id of item associated with pin.
        /// </summary>
        public String Id { get; private set; }
        public SelectedEventArgs(String id) { id = Id; }
    }
    public sealed partial class PointOfInterestPin : UserControl, IAnchorable
    {

        public event EventHandler<SelectedEventArgs> Selected;
        private void OnSelected(SelectedEventArgs e)
        {
            if (Selected != null)
                Selected(this, e);
        }

        /// <summary>
        /// Unique id for this marker used as lookup in other view models
        /// </summary>
        public String Id { get; private set; }

        #region Label dependency property (changing it will change the label)
        public String Label
        {
            get { return (String)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(String), typeof(PointOfInterestPin), 
                new PropertyMetadata(0, (d, e) =>
                    {
                        ((PointOfInterestPin)d).PinLabel.Text = e.NewValue.ToString();
                    }));
        #endregion

        #region IsHighlighted dependency property (changing it will highlight map marker)
        public Boolean IsHighlighted
        {
            get { return (Boolean)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHighlighted.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof(Boolean), typeof(PointOfInterestPin),
            new PropertyMetadata(false, (d, e) =>
            {
                ((PointOfInterestPin)d).Corona.Visibility = (Boolean)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }));
        #endregion

        //
        //
        // TODO: (optional) if the indicator graphic is changed, update the AnchorPoint to reflect what point in the 
        //       graphic should be anchored to the lat/long in the location.
        //
        //
        /// <summary>
        /// Anchor point of push pin (for circular marker, the center point)
        /// </summary>
        public Point AnchorPoint
        {
            get { return new Point(40, 40); }
        }


        /// <summary>
        /// Creates a new push pin marking a point of interest on the map
        /// </summary>
        /// <param name="map">Reference to Bing Maps control</param>
        /// <param name="uid">Unique id of associated item view models (use GUID if no other natural key)</param>
        /// <param name="label">String to displayed as label (depends on design of marker)</param>
        /// <param name="location">Location (latitude/longitude) to place the marker)</param>
        public PointOfInterestPin(String uid, String label)
        {
            this.InitializeComponent();

            // set push pin properties
            Id = uid;
            Label = label;
        }
    }
}
