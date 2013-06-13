using APIMASH;
using APIMASH_BingMaps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH_StarterKit
{
    /// <summary>
    /// This page displays search results when a global location search is directed to this application.
    /// Most of this code was generated automatically when adding Search Contract via Visual Studio's Add New Item dialog.
    /// </summary>
    public sealed partial class LocationSearchResultsPage : APIMASH_StarterKit.Common.LayoutAwarePage
    {
        BingMapsApi _bingMapsApi = new BingMapsApi();
        public LocationSearchResultsPage()
        {
            this.InitializeComponent();

            // allow type-to-search in search charm
            SearchPane.GetForCurrentView().ShowOnKeyboardInput = true;

            // event callback implementation for dismissing the error panel
            ErrorPanel.Dismissed += (s, e) => this.DefaultViewModel["ApiStatus"] = ApiResponseStatus.Default;
            ErrorPanelSnapped.Dismissed += (s, e) => this.DefaultViewModel["ApiStatus"] = ApiResponseStatus.Default;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            String queryText;

            // get query text, either restored from state or via navigation
            if ((pageState != null) && (pageState.ContainsKey("SearchText")))
                queryText = pageState["SearchText"] as String;
            else
                queryText = navigationParameter as String;

            // TODO: Application-specific searching logic.  The search process is responsible for
            //       creating a list of user-selectable result categories:
            //
            //       filterList.Add(new Filter("<filter name>", <result count>));
            //
            //       Only the first filter, typically "All", should pass true as a third argument in
            //       order to start in an active state.  Results for the active filter are provided
            //       in Filter_SelectionChanged below.

            var filterList = new List<Filter>();
            filterList.Add(new Filter("All", 0, true));

            // Communicate results through the view model
            this.DefaultViewModel["RawQueryText"] = queryText;
            this.DefaultViewModel["QueryText"] = '\u201c' + queryText + '\u201d';
            this.DefaultViewModel["Filters"] = filterList;
            this.DefaultViewModel["ShowFilters"] = filterList.Count > 1;
            this.DefaultViewModel["Results"] = _bingMapsApi.BingMapsViewModel.Results;
            this.DefaultViewModel["AppName"] = App.DisplayName;
            this.DefaultViewModel["ApiStatus"] = ApiResponseStatus.Default;
        }

        protected override void SaveState(Dictionary<string, object> pageState)
        {
            pageState["SearchText"] = this.DefaultViewModel["RawQueryText"];
        }

        /// <summary>
        /// Invoked when a filter is selected using the ComboBox in snapped view state.
        /// </summary>
        /// <param name="sender">The ComboBox instance.</param>
        /// <param name="e">Event data describing how the selected filter was changed.</param>
        async void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String targetState = "ResultsFound";

            // Determine what filter was selected
            var selectedFilter = e.AddedItems.FirstOrDefault() as Filter;
            if (selectedFilter != null)
            {
                // Mirror the results into the corresponding Filter object to allow the
                // RadioButton representation used when not snapped to reflect the change
                selectedFilter.Active = true;

                // query Bing location API
                var apiStatus = await _bingMapsApi.GetLocations(this.DefaultViewModel["RawQueryText"] as String, 0, 200);
                this.DefaultViewModel["ApiStatus"] = apiStatus;

                // display results
                if (apiStatus.IsSuccessStatusCode)
                {
                    // Ensure results are found
                    object results;
                    ICollection resultsCollection;
                    if (this.DefaultViewModel.TryGetValue("Results", out results) &&
                        (resultsCollection = results as ICollection) != null &&
                        resultsCollection.Count != 0)
                    {
                        targetState = "ResultsFound";
                    }
                    else
                    {
                        targetState = "NoResultsFound";
                    }
                }
            }
            else
            {
                targetState = "NoResultsFound";
            }

            // Transition to targeted state
            VisualStateManager.GoToState(this, targetState, true);
        }

        /// <summary>
        /// Invoked when a filter is selected using a RadioButton when not snapped.
        /// </summary>
        /// <param name="sender">The selected RadioButton instance.</param>
        /// <param name="e">Event data describing how the RadioButton was selected.</param>
        void Filter_Checked(object sender, RoutedEventArgs e)
        {
            // Mirror the change into the CollectionViewSource used by the corresponding ComboBox
            // to ensure that the change is reflected when snapped
            if (filtersViewSource.View != null)
            {
                var filter = (sender as FrameworkElement).DataContext;
                filtersViewSource.View.MoveCurrentTo(filter);
            }
        }

        /// <summary>
        /// View model describing one of the filters available for viewing search results.
        /// </summary>
        private sealed class Filter : APIMASH_StarterKit.Common.BindableBase
        {
            private String _name;
            private int _count;
            private bool _active;

            public Filter(String name, int count, bool active = false)
            {
                this.Name = name;
                this.Count = count;
                this.Active = active;
            }

            public override String ToString()
            {
                return Description;
            }

            public String Name
            {
                get { return _name; }
                set { if (this.SetProperty(ref _name, value)) this.OnPropertyChanged("Description"); }
            }

            public int Count
            {
                get { return _count; }
                set { if (this.SetProperty(ref _count, value)) this.OnPropertyChanged("Description"); }
            }

            public bool Active
            {
                get { return _active; }
                set { this.SetProperty(ref _active, value); }
            }

            public String Description
            {
                get { return String.Format("{0} ({1})", _name, _count); }
            }
        }

        /// <summary>
        /// Triggered when item in search result page is selected and navigation back to the main page should occur
        /// </summary>
        /// <param name="sender">Object initiating the event</param>
        /// <param name="e">Event argument including the items selected in the search results list</param>
        private void Location_Clicked(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as BingMapsLocationViewModel;

            Frame.Navigate(typeof(MainPage), item.Position.ToString());
        }
    }
}
