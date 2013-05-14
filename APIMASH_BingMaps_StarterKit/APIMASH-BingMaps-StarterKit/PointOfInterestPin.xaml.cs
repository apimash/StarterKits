using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_BingMaps_StarterKit
{
    public sealed partial class PointOfInterestPin : UserControl
    {

        #region Id dependency property (changing id will change the label)
        public Int32 Id
        {
            get { return (Int32)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(Int32), typeof(PointOfInterestPin), 
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

        // TODO: (optional) if the indicator graphic is changed, update the AnchorPoint to reflect what point in the 
        //   graphic should be anchored to the lat/long in the location.
        /// <summary>
        /// Anchor point of pushpin (for circular marker, the center point)
        /// </summary>
        public Point AnchorPoint 
        { 
            get { return new Point(40, 40); }
        }

        public PointOfInterestPin()
        {
            this.InitializeComponent();
        }

        protected override void OnTapped(Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.IsHighlighted = !this.IsHighlighted;
        }
    }
}
