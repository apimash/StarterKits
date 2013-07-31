using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace APIMASHTwitterAPI_WP8Lib.APICommon
{
    //public static class CommonDataTemplateSelector : System.Windows.Controls.ContentControl 
    //{
    //    protected override void OnContentChanged(object oldContent, object newContent)
    //    {
    //        base.OnContentChanged(oldContent, newContent);
    //        ContentTemplate = SelectCustomTemplate<object>(newContent, this) as DataTemplate;
    //    }

    //    public virtual T SelectCustomTemplate<T>(this object item, DependencyObject container)
    //    {
    //        return default(T);
    //    }

    //    public static T FindTemplateResource<T>(this DependencyObject initial, string key) where T : DependencyObject
    //    {
    //        DependencyObject current = initial;

    //        while (current != null)
    //        {
    //            if (current is FrameworkElement)
    //            {
    //                if ((current as FrameworkElement).Resources.Contains(key))
    //                {
    //                    return (T)(current as FrameworkElement).Resources[key];
    //                }
    //            }

    //            current = System.Windows.Media.VisualTreeHelper.GetParent(current);
    //        }

    //        if (Application.Current.Resources.Contains(key))
    //        {
    //            return (T)Application.Current.Resources[key];
    //        }

    //        return default(T);
    //    }
    
    //}

    public abstract class APICommonDataTemplateSelector : System.Windows.Controls.ContentControl
    {
        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            //base.ContentTemplate = SelectTemplate(newContent, this);
                    
               
        }

        

    }


}

//namespace APIMASHTwitterAPI_WP8Lib.APICommon.UIHelper
//{
//        public class TwitterDataTemplateSelector : APICommonDataTemplateSelector
//        {
//            public TwitterDataTemplateSelector() : base()
//            {
//                Microsoft.Phone.Controls.PhoneApplicationPage phoneAppPage = 
//                    (Microsoft.Phone.Controls.PhoneApplicationPage)(((Microsoft.Phone.Controls.PhoneApplicationFrame)Application.Current.RootVisual)).Content;

//                TweetItemTemplate = phoneAppPage.Resources["TweetItemTemplate"] as DataTemplate;
//                UserItemTemplate = phoneAppPage.Resources["UserItemTemplate"] as DataTemplate;
//                TrendItemTemplate = phoneAppPage.Resources["TrendItemTemplate"] as DataTemplate;
              
//                //TweetItemTemplate = Application.Current.Resources["TweetItemTemplate"] as DataTemplate;
//                //UserItemTemplate = Application.Current.Resources["UserItemTemplate"] as DataTemplate;
//                //TrendItemTemplate = Application.Current.Resources["TrendItemTemplate"] as DataTemplate;
            
//            }
            
//            public DataTemplate TweetItemTemplate
//            {
//                get;
//                set;
//            }

//            public DataTemplate UserItemTemplate
//            {
//                get;
//                set;
//            }

//            public DataTemplate TrendItemTemplate
//            {
//                get;
//                set;
//            }
 
//        public override DataTemplate SelectTemplate(object item, DependencyObject container)
//        {
//            object itemData = null;
//            //Setting a default so the compiler will stop whining about value not being set
//            TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection templateSelected = TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TweetItemTemplate; ;

//            if (item.GetType() == typeof(TwitterAPIWinPhone8Lib.Model.Tweet))
//                templateSelected = TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TweetItemTemplate;

//            if (item.GetType() == typeof(TwitterAPIWinPhone8Lib.Model.User))
//                templateSelected = TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.UserItemTemplate;

//            if (item.GetType() == typeof(TwitterAPIWinPhone8Lib.Model.Trend))
//                templateSelected = TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TrendItemTemplate;

            
//            switch (templateSelected)
//            {
//                case TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TweetItemTemplate:
//                    itemData = item as TwitterAPIWinPhone8Lib.Model.Tweet;
//                    return TweetItemTemplate;
                
//                case TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.UserItemTemplate:
//                    itemData = item as TwitterAPIWinPhone8Lib.Model.User;
//                    return UserItemTemplate;
                
//                case TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TrendItemTemplate:
//                    itemData = item as TwitterAPIWinPhone8Lib.Model.Trend;
//                    return TrendItemTemplate;
//                    break;
//            }

//            return base.SelectTemplate(itemData, this);
//        }

//     }
//}
