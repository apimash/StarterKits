#APIMASH Windows Phone 8 World of Warcraft API Kit
##Date: 6.25.2013
##Version: v1.0.0
##Author(s): David Isbitski
##URL: http://github.com/disbitski/WP8WoWAPIKit

----------
###Description
The Windows Phone 8 World of Warcraft API Kit is intended to teach Windows developers how to incorporate the World of Warcraft APIs into their own apps.
This Kit is for educational and entertainment purposes only.  World of Warcraft is a very successful online game from Blizzard Entertainment®.   
Blizzard Entertainment® is the copyright holder to all World of Warcraft content and images used in this application.
Blizzard Entertainment® is the owner of the World of Warcraft API which can be found at http://blizzard.github.io/api-wow-docs/.

![alt text][1]![alt text][2]![alt text][3]
![alt text][4]
###Features
WoW API
  - Realm Status (individual)
  - Realm Status (all)

Windows Phone App
  - Application Bar
  - Settings Page 
  - About Page 
  - Web Browser Control
  - Web Tasks
  - Calling HTML5/JS from C#
  - Small and Medium Tiles

###Requirements

 - Windows 8
 - Visual Studio 2012 Express for Windows Phone or higher

###Setup

 - Download the Starter Kit Zip Portfolio from (http://apimash.github.io/StarterKits/)
 - Open the Solution in Visual Studio
 - Compile and Run

###Customization

Step 1. All of the Blizzard World of Warcraft API's do not require a developer key to use.  However, if you plan on creating an application with heavy api usage Blizzard requests you contact them at api-support@blizzard.com to register your application.

Step 2. Currently only Realm Status is implemented.  Adding additional functionality such as character or pvp info is as easy as calling the appropriate wow api (found in http://blizzard.github.io/api-wow-docs/) and then wrapping it in a function the same way realm status was done in /js/wowapi.js.

Step 3. Once you have data returning you should load the results into a Web Browser Control using divs similar to /html/index.html.  


##DISCLAIMER: 

The sample code described herein is provided on an "as is" basis, without warranty of any kind, to the fullest extent permitted by law. Both Microsoft and I do not warrant or guarantee the individual success developers may have in implementing the sample code on their development platforms or in using their own Web server configurations. 
Microsoft and I do not warrant, guarantee or make any representations regarding the use, results of use, accuracy, timeliness or completeness of any data or information relating to the sample code. Microsoft and I disclaim all warranties, express or implied, and in particular, disclaims all warranties of merchantability, fitness for a particular purpose, and warranties related to the code, or any service or software related thereto. 

Microsoft and I shall not be liable for any direct, indirect or consequential damages or costs of any type arising out of any action taken by you or others related to the sample code.


* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt




----------

##Change Log
###v1.0.0 - Updated to point to new license file


  [1]: https://raw.github.com/disbitski/WP8WoWAPIKit/master/AllRealms.png "All Realms"
  [2]: https://raw.github.com/disbitski/WP8WoWAPIKit/master/SingleRealmMenu.png "Single Page with AppBar"
  [3]: https://raw.github.com/disbitski/WP8WoWAPIKit/master/About.png "About"
  [4]: https://raw.github.com/disbitski/WP8WoWAPIKit/master/settings.png "Settings"
 
