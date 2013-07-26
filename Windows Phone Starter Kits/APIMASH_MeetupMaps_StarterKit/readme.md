#Meetup and Maps Starter Kit
##Date: 07/26/2013
##Version: v0.0.1
##Author(s): G. Andrew Duthie
##URL: http://github.com/apimash/starterkits

----------
###Description
The Meetup and Maps Starter Kit is a Windows Phone 8 app based on the Databound App template. It leverages the Meetup.com API to search for upcoming meetups near a given location, with optional keyword filtering. Once the meetups are retrieved, the starter kit maps a selected item, and queries the Bing Maps Search API for nearby coffee shops, and maps those as well.

![alt text][1]

###Features
 - Demonstrates using System.Net.WebClient to call the [Meetup.com API][2]
 - Demonstrates integrating the [Maps API][3], and leveraging the [MapsTask][4] object to find local points of interest
 - Demonstrates how to use Linq to XML to load live data into the Databound App template, and how to adapt the live data to display with minimal customization to the XAML and C# files in the template

###Requirements
 - Meetup API Key (don't use your day-to-day meetup ID for this...create a unique account, and sign up for a key [here][5])
 - Windows Phone Marketplace Account (to submit app for certification)

###Setup and Customization
Basic customization is done via the file AppConstants.cs, found in the Customiztion folder. Here are the key items you can customize:

 - meetupUri: base uri for the Meetup API to be called
 - meetupKey: Your custom Meetup API key
 - meetupDistance: Distance, in miles, for search. Default is 50
 - maxMeetupstoFind: Maps to the Meetup API parameter 'page'. Use with the 'offset' parameter to implement paging. Default is 200
 - meetupCity: City where you want to find upcoming Meetups
 - meetupState: State where you want to find upcoming Meetups
 - meetupKeywords: Optional keyword(s) to filter response. Can help in limiting response to a specific topic. Default is "JavaScript"
 - searchTerm: Search term to be passed to the MapsTask. Default is "coffee"
 
More advanced customization is possible by digging into the code and XAML. All logic for retrieving and grouping Meetup data may be found in ViewModels\MainViewModel.cs.

###Known Issues
Here are the currently known issues (aka learning opportunities for the developer):

##**DISCLAIMER**: 

The sample code described herein is provided on an "as is" basis, without warranty of any kind, to the fullest extent permitted by law. Both Microsoft and I do not warrant or guarantee the individual success developers may have in implementing the sample code on their development platforms or in using their own Web server configurations. 

Microsoft and I do not warrant, guarantee or make any representations regarding the use, results of use, accuracy, timeliness or completeness of any data or information relating to the sample code. Microsoft and I disclaim all warranties, express or implied, and in particular, disclaims all warranties of merchantability, fitness for a particular purpose, and warranties related to the code, or any service or software related thereto. 

Microsoft and I shall not be liable for any direct, indirect or consequential damages or costs of any type arising out of any action taken by you or others related to the sample code.

----------

##Change Log
###v0.0.1

 - initial release


  [1]: https://raw.github.com/apimash/StarterKits/master/Windows%20Phone%20Starter%20Kits/APIMASH_MeetupMaps_StarterKit/MainPage.png "Home Page"
  [2]: http://www.meetup.com/meetup_api/ "Meetup API"
  [3]: http://msdn.microsoft.com/en-US/library/windowsphone/develop/jj207045(v=vs.105).aspx
  [4]: http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj206989(v=vs.105).aspx
  [5]: http://www.meetup.com/meetup_api/ "Meetup API"