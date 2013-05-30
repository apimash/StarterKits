using APIMASH.Mapping;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH_StarterKit.Mapping
{
    /// <summary>
    /// Occurs when point-of-interest pin is selected
    /// </summary>
    public class SelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Point-of-interest associated with pin (as IMappable)
        /// </summary>
        public readonly IMappable PointOfInterest;

        public SelectedEventArgs(IMappable poi) 
        { 
            PointOfInterest = poi; 
        }
    }
    public sealed partial class PointOfInterestPin : UserControl, IAnchorable
    {
        /// <summary>
        /// Triggered when a point-of-interest pin is selected on the map
        /// </summary>
        public event EventHandler<SelectedEventArgs> Selected;
        private void OnSelected(SelectedEventArgs e)
        {
            if (Selected != null)
                Selected(this, e);
        }

        /// <summary>
        /// Point-of-interest object (an IMappable object typically part of the view model assocated with map items)
        /// </summary>
        public IMappable PointOfInterest { get; private set; }

        #region Label dependency property (changing it will change the label on the PinLabel UIElement)
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

        #region IsHighlighted dependency property (changing it will highlight map marker via storyboard)
        public Boolean IsHighlighted
        {
            get { return (Boolean)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, value); }
        }

        // IsHighlighted dependency property enabling animation, styling, binding, etc...
        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof(Boolean), typeof(PointOfInterestPin),
            new PropertyMetadata(false, (d, e) =>
            {
                PointOfInterestPin p = (PointOfInterestPin)d;
                Visibility v = (Boolean)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
                p.Corona.Visibility = p.CoronaEffect.Visibility = v;
                if (v == Visibility.Visible) p.RotateEffect.Begin();
            }));
        #endregion

        /// <summary>
        /// Anchor point of push pin (for circular marker, the center point)
        /// </summary>
        public Point AnchorPoint
        {
            //
            // TODO: (optional) if the indicator graphic is changed, update the AnchorPoint to reflect what point in the 
            //       graphic should be anchored to the lat/long in the location.
            //
            get { return new Point(40, 40); }
        }

        /// <summary>
        /// Creates a new push pin marking a point of interest on the map
        /// </summary>
        /// <param name="poi">Reference to an IMappable instance</param>
        public PointOfInterestPin(IMappable poi)
        {
            this.InitializeComponent();
            PointOfInterest = poi;

            // track label as a separate property, but it's populated on creation from the point-of-interest info
            Label = poi.Label;

            this.PointerPressed += (s, e) => { OnSelected(new SelectedEventArgs(PointOfInterest)); };
        }

        #region cursor hover feedback for mouse input
        protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 0);
        }

        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }

        protected override void OnPointerCanceled(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }
        #endregion
    }
}
