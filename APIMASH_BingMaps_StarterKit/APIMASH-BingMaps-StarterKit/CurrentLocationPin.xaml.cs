using Windows.Foundation;
using Windows.UI.Xaml.Controls;

//
// LICENSE: http://opensource.org/licenses/ms-pl
//

namespace APIMASH_BingMaps_StarterKit
{
    public sealed partial class CurrentLocationPin : UserControl
    {
        public Point AnchorPoint 
        { 
            get { return new Point(10, 10); }
        }

        public CurrentLocationPin()
        {
            this.InitializeComponent();
        }
    }
}
