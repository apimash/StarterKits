using Callisto.Controls.Common;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH_StarterKit
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Application display name extracted from the manifest
        /// </summary>
        public static String DisplayName { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // get the name of the app from the mainfest
            DisplayName = (await AppManifestHelper.GetManifestVisualElementsAsync()).DisplayName;

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected async override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            // get the name of the app from the mainfest
            DisplayName = (await AppManifestHelper.GetManifestVisualElementsAsync()).DisplayName;

            // If the Window isn't already using Frame navigation, insert our own Frame
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            // If the app does not contain a top-level frame, it is possible that this 
            // is the initial launch of the app. Typically this method and OnLaunched 
            // in App.xaml.cs can call a common method.
            if (frame == null)
            {
                // Create a Frame to act as the navigation context and associate it with
                // a SuspensionManager key
                frame = new Frame();
                APIMASH_StarterKit.Common.SuspensionManager.RegisterFrame(frame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await APIMASH_StarterKit.Common.SuspensionManager.RestoreAsync();
                    }
                    catch (APIMASH_StarterKit.Common.SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }
            }

            frame.Navigate(typeof(LocationSearchResultsPage), args.QueryText);
            Window.Current.Content = frame;

            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when the application window is created.
        /// </summary>
        /// <param name="args">Details about the window creation.</param>
        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            // handle search request when app is active
            var searchPane = SearchPane.GetForCurrentView();
            searchPane.PlaceholderText = "Enter a city or point of interest";

            searchPane.QuerySubmitted += (s, e) =>
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame != null)
                {
                    rootFrame.Navigate(typeof(LocationSearchResultsPage), e.QueryText);
                    Window.Current.Content = rootFrame;

                    // Ensure the current window is active
                    Window.Current.Activate();
                }
            };

            //
            // TODO: create an optional customized list of result (or query) suggestions; otherwise, remove the event handler
            //
            searchPane.SuggestionsRequested += (s, e) =>
            {
                foreach (var option in APIMASH_TomTom.TomTomApi.SearchSuggestionList)
                {
                    // search both the start of the string and after the comma where the state name starts
                    var alternateSearch = option.Label.Substring(Math.Max(0, option.Label.IndexOf(", ") + 2));
                    if (option.Label.StartsWith(e.QueryText, StringComparison.CurrentCultureIgnoreCase) ||
                        alternateSearch.StartsWith(e.QueryText, StringComparison.CurrentCultureIgnoreCase))
                        e.Request.SearchSuggestionCollection.AppendResultSuggestion(option.Label, String.Empty, option.Id, 
                             RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/cameraSearch.png")), String.Empty);

                    // there's a max of five options in the list
                    if (e.Request.SearchSuggestionCollection.Size >= 5)
                        break;
                }
            };

            //
            // TODO: act upon the option selected from the Search flyout, or remove the event handler if not needed
            //
            searchPane.ResultSuggestionChosen += (s, e) =>
            {
                var selectedLocation = APIMASH_TomTom.TomTomApi.SearchSuggestionList.Where(x => x.Id == e.Tag).FirstOrDefault();

                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame != null)
                {
                    rootFrame.Navigate(typeof(MainPage),
                        new APIMASH.Mapping.LatLong(selectedLocation.Position.Latitude, selectedLocation.Position.Longitude));

                    Window.Current.Content = rootFrame;
                    Window.Current.Activate();
                }
            };
        }
    }
}