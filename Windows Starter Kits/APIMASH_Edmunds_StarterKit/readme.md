#APIMASH Edmunds Starter Kit
##Date: 5.10.2013
##Version: v1.0.0
##Author(s): Bob Familiar
##URL: http://github.com/apimash/starterkits

----------
##Description
The Edmunds Starter Kit is a XAML/C# Windows 8 app based on the Blank Template that demonstrates calling the Edmunds REST API's. The JSON payload for Makes, Models, Model Specs and Pictures is deserialized into a set of C# classes that define the Data Model. That data then is selectively copied into the View Model for binding to WinRT XAML controls. You can use the breadth and detail of the automotive information available through the Edmunds API to create mashups, visualizations and other applications that will provide an added dimension of user experience for the automotive consumer.

Blog: [APIMASH Edmunds Starter Kit][1]

![alt text][2]

##Features
 - Invokes the [Edmunds REST API][3]
 - Demonstrates how to deserialize JSON to C# and bind to WinRT XAML Controls
 - Provides a baseline for a Windows 8 Store App

##Requirements

 - Windows 8
 - Visual Studio 2012 Express for Windows 8 or higher
 - [JSON.NET form Newtonsoft][4]
 - [Mashery.com Developer Account][5]
 - [Edmunds Developer Key][6]

##Setup

 - [Register at Mashery.com][7]
 - [Request a Developer Key at Edmunds][8]
 - [Download the Starter Kit Zip Portfolio][9] 
 - Open the Solution in Visual Studio
 - Add your Developer Key in the *Globals.cs* file
 - Update the reference to the *Newtonsoft JSON.NET Library* in the *APIMASHLib* project
 - Compile and Run

##Customization

The Edmunds API, one a scale of 1 to 10, where 1 is simple and 10 is complex, is an 11 :). Edmunds provides a rich set of API collections each with several API's and methods that give you access to Articles, Vehicle Data, Dealer info and Inventory data that can be used together or with other API's to create compelling apps.

Edmunds provides these 4 API collections:

 - [Editorial API][10]
 - [Vehicle API][11]
 - [Inventory API][12] - available to Edmunds Partners
 - [Dealer API][13]

The Edmunds Starter Kit uses the following API's in the Vehicle API collection:

 - [Make Repository][14] - The Make repository provides information according to the Make of a vehicle
 - [Model Year Repository][15] - The Model Year repository is the root entity of the Edmunds data repository. All vehicles data is organized according to Model Year
 - [Photo Repository][16] - The Photo Repository provides links that resolve back to photo media on the Edmunds Media Server

The Starter Kit starts by calling the ***findmakesbymodelyear*** method of the Make Repository API to retrieve a list of makes and models by a particular year. [*See Line 57 in the *MainPage.Xaml.cs* file*].

Next It then invokes the Model Year Repository API ***foryearmakemodel*** method to get the Model Spec data which contains a StylerId. [*See line 88 in the *MainPage.Xaml.cs* file*]

The StyleId is used as input to the third call, the Vehicle Photo Repository ***findphotosbystyleid*** method that is used to get a list of vehicle images. [*See line 142 of the *MainPage.Xaml.cs* file*.]

You can customize the initial call to the Make Repository method by commenting out line 61 and uncommenting line 62 in *MainPage.Xaml.cs* to invoke the ***findall*** method.

To experiment further you can look at the additional capabilities of the Make and Model/Year Repository API's.

The Make Repository API makes the following methods available:

 - ***findall*** - Get the list of all makes and their all their models
 - ***findbyid*** - Find a make and its models by providing a make ID
 - ***findfuturemakes*** - Find all future makes and their models
 - ***findmakebyname*** - Find a make details by its name 
 - ***findmakesbymodelyear*** - Find a make by a year 
 - ***findmakesbypublicationstate*** - Find makes by their state (new or used) 
 - ***findnewandused*** - Find all new and used makes 
 - ***findnewandusedmakesbymodelyear*** - Find all new and used makes for a particular year
 - ***findnewmakes*** - Find only new makes 
 - ***findusedmakes*** - Find only old makes 

The Model Year Repository API also provides these methods:

 - ***findbyid*** - Get details on a specifc vehicle by its model year ID
 - ***finddistinctyearwithnew*** - Get a list of years under which there are new vehicle listings 
 - ***finddistinctyearwithneworused*** - Get a list of years under which there are new or used vehicle listings
 - ***finddistinctyearwithused*** - Get a list of years under which there are used vehicle listings 
 - ***findfuturemodelyearsbymodelid*** - Get a list of future model years by the model ID 
 - ***findmodelyearsbymakeandyear*** - Get a list of model years for a specific make and year
 - ***findmodelyearsbymakemodel*** - Get a list of model years for a specific make and model 
 - ***findnewandusedmodelyearsbymakeidandyear*** - Get a list of new and used model years for a specific make ID and year
 - ***findnewmodelyearsbymodelid*** - Get a list of new model years by the model ID 
 - ***findusedmodelyearsbymodelid*** - Get a list of used model years by the model ID 
 - ***findyearsbycategoryandpublicationstate*** - Get a list of model years for a specific category and publication state 
 - ***formodelid*** - Get a list of model years by the model ID 
 - ***foryearmakemodel*** - Get a list of model years for a specific make, model and year 

You can also experiment with adding in the Editorial or Dealer API's to add color commentary and availability information.

##DISCLAIMER: 

The sample code described herein is provided on an "as is" basis, without warranty of any kind, to the fullest extent permitted by law. Both Microsoft and I do not warrant or guarantee the individual success developers may have in implementing the sample code on their development platforms or in using their own Web server configurations. 
Microsoft and I do not warrant, guarantee or make any representations regarding the use, results of use, accuracy, timeliness or completeness of any data or information relating to the sample code. Microsoft and I disclaim all warranties, express or implied, and in particular, disclaims all warranties of merchantability, fitness for a particular purpose, and warranties related to the code, or any service or software related thereto. 

Microsoft and I shall not be liable for any direct, indirect or consequential damages or costs of any type arising out of any action taken by you or others related to the sample code.


  [1]: http://theundocumentedapi.com/index.php/apimash-the-edmunds-starter-kit/
  [2]: https://raw.github.com/apimash/StarterKits/master/Windows%20Starter%20Kits/APIMASH_Edmunds_StarterKit/EdmundsScreenshot.png "Edmunds Starter Kit"
  [3]: http://developer.edmunds.com "Edmunds"
  [4]: https://json.codeplex.com/ "JSON.NET"
  [5]: http://developer.mashery.com/ "Mashery.com"
  [6]: http://developer.edmunds.com "Edmunds Developer Key"
  [7]: http://developer.mashery.com/ "Register at Mashery.com"
  [8]: http://developer.edmunds.com "Edmunds Developer Key"
  [9]: http://apimash.github.io/StarterKits "APIMASH Starter Kits"
  [10]: http://developer.edmunds.com/docs/read/the_editorial_api "Editorial API"
  [11]: http://developer.edmunds.com/docs/read/The_Vehicle_API "Vehicle API"
  [12]: http://developer.edmunds.com/docs/read/the_inventory_api "Inventory API"
  [13]: http://developer.edmunds.com/docs/read/The_Dealer_API "Dealer API"
  [14]: http://developer.edmunds.com/docs/read/the_vehicle_api/Make_Repository "Make Repository"
  [15]: http://developer.edmunds.com/docs/read/the_vehicle_api/Year_Repository "Model Year Repository"
  [16]: http://developer.edmunds.com/docs/read/the_vehicle_api/Photos "Photo Repository"