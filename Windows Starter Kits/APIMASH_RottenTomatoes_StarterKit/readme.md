#APIMASH Rotten Tomatoes Starter Kit
##Date: 5.10.2013
##Version: v1.0.0
##Author(s): Bob Familiar
##URL: http://github.com/apimash/starterkits

----------
###Description
The Rotten Tomatoes Starter Kit is a XAML/C# Windows 8 app based on the Grid Template that demonstrates calling the Rotten Tomatoes REST API's. The JSON payload for Movies or DVD's is deserialized into a set of C# classes that define the Data Model. That data then is selectively copied into the View Model for binding to the WinRT XAML controls.

Blog: [APIMASH Rotten Tomatoes Starter Kit][1]

![alt text][2]
###Features
 - Invokes the Rotten Tomatoes REST API (http://developer.rottentomatoes.com/)
 - Demonstrates how to deserialize JSON to C# and bind to WinRT XAML Controls
 - Provides a baseline for a Windows 8 Store App

###Requirements

 - Windows 8
 - Visual Studio 2012 Express for Windows 8 or higher
 - WintRT XAML Toolkit (https://winrtxamltoolkit.codeplex.com/)
 - JSON.NET form Newtonsoft (https://json.codeplex.com/)
 - Mashery.com Developer Account (http://developer.mashery.com/)
 - Rotten Tomatoes Developer Key (http://developer.rottentomatoes.com/)

###Setup

 - Register at Mashery.com (http://developer.mashery.com/)
 - Request a Developer Key from Rotten Tomatoes (http://developer.rottentomatoes.com/)
 - Download the Starter Kit Zip Portfolio from (http://apimash.github.io/StarterKits/)
 - Open the Solution in Visual Studio
 - Add your Developer Key in the Globals.cs file
 - Update the reference to the Newtonsoft JSON.NET Library in the APIMASHLib project
 - Compile and Run

###Customization

Step 1. Add your Developer Key in the *Globals.cs* source file on line 20

Step 2. *Global.cs* provides URI's for the 9 API’s that Rotten Tomatoes [offers][3]. Note that the Movie payload provides URI's to additional APIs such as reviews, cast, similar and clips. The Starter Kit implements the Review API as an example.

 - **movies.json** - Search for movies    
  - **movie.json** - info on an individual movie
     - **reviews.json** - the reviews for an individual movie 
     - **cast.json** - the full cast for an individual movie
     - **similar.json** - a list of movies similar to the individual movie    
     - **clips.json** - a list of clips related  to an individual movie
 - **box_office.json** - a current list of box office movies  
 - **upcoming.json** -movies that are upcoming
 - **in_theaters.json** - movies that are in theaters 
 - **opening.json** - movies that are opening this week  
 - **top_rentals.json**  - top dvd rentals 
 - **current_releases.json** - dvds that are currently released  
 - **new_releases.json** - dvds that are getting released  
 - **upcoming.json** - dvds that are getting released

Each API returns the same JSON payload format. The Starter Kit can be customized to display results from any of these API’s. I encourage you to experiment  by simple changing the API selection in the *GroupedItemsPage.Xaml.cs* file on line 76 from 

> ROTTEN TOMATOES API MOVIES INTHEATERS

to

> ROTTEN TOMATOES API DVD CURRENTRELEASES

Step 3. You can modify the parameters to each query such as the limit of the number of movies to return in total, or per page and the country. Experiment by changing the ‘country’ parameter on the ‘InTheaters’ API call on line 24 of *Globals.cs* from 'us' to 'uk'. Experiment with other parameter to learn the API. Visit the API [I/O Docs page][4] to experiment on line.

Step 4. Movie information is EXTREMELY mashable! Experiment by combining Rotten Tomatoes with Bing Maps showing nearby movie theaters (check out [the Bing Map Starter Kit][5]!)

##DISCLAIMER: 

The sample code described herein is provided on an "as is" basis, without warranty of any kind, to the fullest extent permitted by law. Both Microsoft and I do not warrant or guarantee the individual success developers may have in implementing the sample code on their development platforms or in using their own Web server configurations. 
Microsoft and I do not warrant, guarantee or make any representations regarding the use, results of use, accuracy, timeliness or completeness of any data or information relating to the sample code. Microsoft and I disclaim all warranties, express or implied, and in particular, disclaims all warranties of merchantability, fitness for a particular purpose, and warranties related to the code, or any service or software related thereto. 

Microsoft and I shall not be liable for any direct, indirect or consequential damages or costs of any type arising out of any action taken by you or others related to the sample code.


  [1]: http://theundocumentedapi.com/index.php/apimash-the-rotten-tomatoes-api-starter-kit/
  [2]: https://raw.github.com/apimash/StarterKits/master/Windows%20Starter%20Kits/APIMASH_RottenTomatoes_StarterKit/RottenTomatoesScreenshot.png "Rotten Tomatoes Starter Kit"
  [3]: http://developer.rottentomatoes.com/docs/read/JSON "other API's:"
  [4]: http://developer.mashery.com/iodocs "I/O Docs page"
  [5]: https://github.com/apimash/StarterKits/tree/master/APIMASH_BingMaps_StarterKit "Bing Maps Starter Kit"
