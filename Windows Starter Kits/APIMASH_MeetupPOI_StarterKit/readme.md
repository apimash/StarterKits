#Meetup/Bing Maps Starter Kit
##Date: 05/08/2013
##Version: v0.0.2
##Author(s): G. Andrew Duthie
##URL: http://github.com/apimash/starterkits

----------
###Description
The Meetup/Bing Maps Starter Kit is a native HTML5 and JavaScript Windows Store app based on the Grid App template. It leverages the Meetup.com API to search for upcoming meetups near a given location, with optional keyword filtering. Once the meetups are retrieved, the starter kit maps a selected item, and queries the Bing Maps Search API for nearby coffee shops, and maps those as well.

![alt text][1]

###Features
 - Demonstrates using WinJS.xhr to call the [Meetup.com API][2]
 - Demonstrates integrating Bing Maps, and calling the Bing Maps Search API
 - Demonstrates how to load live data into the JavaScript Grid App template, and how to adapt the live data to display with minimal customization to the HTML and JavaScript files in the template
 - Leverages SVG for item images, for easy scaling
 - Demonstrates the use of the Settings contract to provide About and Privacy pages
 - Demonstrates the use of the Share Source contract, to easily share app content with other installed apps

###Requirements
 - Meetup API Key (don't use your day-to-day meetup ID for this...create a unique account, and sign up for a key [here][3])
 - Bing Maps SDK for Windows Store apps - Download [here][4], and follow the instructions [here][5] for adding a reference to the SDK to your app. 
 - Bing Maps API Key (sign up [here][6] - Microsoft Account required for the Bing Maps Portal)
 - Windows Store Account (to submit app for certification)

###Setup and Customization
Basic customization is done via the file customizeMe.js, found in the js folder. Here are the key items you can customize:

 - meetupKey: Your custom Meetup API key
 - meetupDistance: Distance, in miles, for search. Default is 50
 - maxMeetupstoFind: Maps to the Meetup API parameter 'page'. Use with the 'offset' parameter to implement paging. Default is 200
 - meetupCity: City where you want to find upcoming Meetups
 - meetupState: State where you want to find upcoming Meetups
 - meetupKeywords: Optional keyword(s) to filter response. Can help in limiting response to a specific topic. Default is "JavaScript"
 - bingMapsKey: Your custom Bing Maps API key
 - shareTitle: Default text for Share contract Title property
 - shareMessage: Default text for Share contract Text property

**NOTE**: You will need to add your own developer signing certificate to the project, by opening the package.appxmanifest file, and switching to the Packaging tab. On the packaging tab, click the "Choose Certificate..." button, and in the resulting dialog, click the "Configure Certificate..." drop-down, and select "Create test certificate..." then click OK to dismiss all dialogs, and save the app manifest file.
 
More advanced customization is possible by digging into the code and stylesheets. All logic for retrieving and grouping Meetup data may be found in js/meetupData.js. App-wide styles may be found in css/default.css, and page-specific styles may be found in each respective page folder.

###Known Issues
Here are the currently known issues (aka learning opportunities for the developer):

 - Snapped View: Snapped view is partially implemented (via the CSS media queries from the Grid App template), but the snapped view will need to be improved prior to submitting an app for the Windows Store.
 - App Images: Splash Screen, app tile logos, etc. need to be provided prior to submitting an app based on this starter kit for certification in the Windows Store

##**DISCLAIMER**: 

The sample code described herein is provided on an "as is" basis, without warranty of any kind, to the fullest extent permitted by law. Both Microsoft and I do not warrant or guarantee the individual success developers may have in implementing the sample code on their development platforms or in using their own Web server configurations. 

Microsoft and I do not warrant, guarantee or make any representations regarding the use, results of use, accuracy, timeliness or completeness of any data or information relating to the sample code. Microsoft and I disclaim all warranties, express or implied, and in particular, disclaims all warranties of merchantability, fitness for a particular purpose, and warranties related to the code, or any service or software related thereto. 

Microsoft and I shall not be liable for any direct, indirect or consequential damages or costs of any type arising out of any action taken by you or others related to the sample code.

----------

##Change Log
###v0.0.2

 - Added license information to all source files

 - List item


  [1]: https://raw.github.com/apimash/StarterKits/master/Windows%20Starter%20Kits/APIMASH_MeetupPOI_StarterKit/homePage.png "Home Page"
  [2]: http://www.meetup.com/meetup_api/ "Meetup API"
  [3]: http://www.meetup.com/meetup_api/ "Meetup API"
  [4]: http://visualstudiogallery.msdn.microsoft.com/bb764f67-6b2c-4e14-b2d3-17477ae1eaca
  [5]: http://msdn.microsoft.com/en-us/library/hh852186.aspx
  [6]: https://www.bingmapsportal.com/ "Bing Maps Portal"
