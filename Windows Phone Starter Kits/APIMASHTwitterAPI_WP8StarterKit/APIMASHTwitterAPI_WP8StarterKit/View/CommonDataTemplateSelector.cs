using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace APIMASHTwitterAPI_WP8StarterKit
{
   
    public abstract class APICommonDataTemplateSelector : System.Windows.Controls.ContentControl
    {
        
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
           
            ContentTemplate = SelectTemplate(newContent, this);
            
        }

        public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return null;
        }

    }

    public class TwitterDataTemplateSelector : TemplateSelector //APICommonDataTemplateSelector
    {
        //public TwitterDataTemplateSelector() //: base()
        //{
        //    TweetTemplate = TwitterHomePage.currentPage.Resources["TweetItemTemplate"] as DataTemplate;
        //    UserTemplate = TwitterHomePage.currentPage.Resources["UserItemTemplate"] as DataTemplate;
        //    TrendTemplate = TwitterHomePage.currentPage.Resources["TrendItemTemplate"] as DataTemplate;

        //}

        public DataTemplate TweetTemplate
        {
            get;
            set;
        }

        public DataTemplate UserTemplate
        {
            get;
            set;
        }

        public DataTemplate TrendTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            object itemData = null;
            //Setting a default so the compiler will stop whining about value not being set
            TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection templateSelected = TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TweetItemTemplate; 

            if (item.GetType() == typeof(TwitterAPIWinPhone8Lib.Model.Tweet))
                templateSelected = TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TweetItemTemplate;

            if (item.GetType() == typeof(TwitterAPIWinPhone8Lib.Model.User))
                templateSelected = TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.UserItemTemplate;

            if (item.GetType() == typeof(TwitterAPIWinPhone8Lib.Model.Trend))
                templateSelected = TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TrendItemTemplate;


            switch (templateSelected)
            {
                case TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TweetItemTemplate:
                    itemData = item as TwitterAPIWinPhone8Lib.Model.Tweet;
                    return TweetTemplate;

                case TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.UserItemTemplate:
                    itemData = item as TwitterAPIWinPhone8Lib.Model.User;
                    return UserTemplate;

                case TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TrendItemTemplate:
                    itemData = item as TwitterAPIWinPhone8Lib.Model.Trend;
                    return TrendTemplate;
                    break;
            }

            return null;
        }

    }

    public abstract class TemplateSelector : ContentControl
    {
        public abstract DataTemplate SelectTemplate(object item, DependencyObject container);

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            //var parent = GetParentByType<LongListSelector>(this);
            //var index = parent.ItemsSource.IndexOf(newContent);
            //var totalCount = parent.ItemsSource.Count;

            ContentTemplate = SelectTemplate(newContent, this);
        }

        private static T GetParentByType<T>(DependencyObject element) where T : FrameworkElement
        {
            T result = null;
            DependencyObject parent = VisualTreeHelper.GetParent(element);

            while (parent != null)
            {
                result = parent as T;

                if (result != null)
                {
                    return result;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }
    
    public class TweetDataTemplateSelector : IValueConverter
 
    {
 
        public DataTemplate TweetItemTemplate { get; set; }
 
        public DataTemplate UserItemTemplate { get; set; }
 
        public DataTemplate TrendItemTemplate { get; set; }
  
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object itemData;
            TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection templateSelected = TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TweetItemTemplate; ;

            if (value.GetType() == typeof(TwitterAPIWinPhone8Lib.Model.Tweet))
                templateSelected = TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TweetItemTemplate;

            if (value.GetType() == typeof(TwitterAPIWinPhone8Lib.Model.User))
                templateSelected = TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.UserItemTemplate;

            if (value.GetType() == typeof(TwitterAPIWinPhone8Lib.Model.Trend))
                templateSelected = TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TrendItemTemplate;


            switch (templateSelected)
            {
                case TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TweetItemTemplate:
                    itemData = value as TwitterAPIWinPhone8Lib.Model.Tweet;
                    return TweetItemTemplate;

                case TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.UserItemTemplate:
                    itemData = value as TwitterAPIWinPhone8Lib.Model.User;
                    return UserItemTemplate;

                case TwitterAPIWinPhone8Lib.ViewModel.TweetViewModel.TweetTemplateSelection.TrendItemTemplate:
                    itemData = value as TwitterAPIWinPhone8Lib.Model.Trend;
                    return TrendItemTemplate;
                    break;
            }
 
 
 
            return null;
 
        }
 
 
 
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
 
        {
 
            throw new NotImplementedException();
 
        }
 
    }
 
}
 
 




    