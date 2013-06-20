#APIMASH Twitter Starter Kit
##Date: 05.30.2013
##Version: v2.0.0
##Author(s): Tara E. Walker
##http://github.com/apimash/starterkits

----------
###Description
The Twitter Starter Kit is an XAML/C# Windows 8 application using a lite MVVM commanding pattern that demonstrates how to call various Twitter REST APIs.  Each call returns a JSON payload that is then deserialized into a observable collection of set of C# classes that are derived from the Model (Data Model).  That observable collection is returned to the View Model and data therein is displayed via data binding of elements to the WinRT Xaml control.

###ScreenShots
App on Start Screen

![alt text][1]

App Start Page

![alt text][2]

Home Timeline Tweets

![alt text][3]

Specified User Timeline Tweets

![alt text][4]

Keyword Search Tweets

![alt text][5]

Worldwide Trends

![alt text][6]

User Followers

![alt text][7]

###Features
 - Retrieves and creates OAuth required token and signatures to call Twitter OAuth required REST API
 - Can retrieve authorized user Home Timeline
 - Can retrieve any timeline of statuses per user screen name entered (as long as not protected)
 - Can retrieve current Worldwide Trends
 - Can search on keyword or user to return associated tweets
 - Can return followers by user, if no user is specified will return followers by authorized user

###Requirements
 - Windows 8
 - Visual Studio 2012 Express for Windows 8 or higher
 - JSON.NET form Newtonsoft (https://json.codeplex.com/)
 - Twitter Developer Account/App (http://dev.twtter.com/)
 - Twitter Consumer Key and Consumer Secret (http://dev.twitter.com/)
 - List item

###Setup
 1. Create an application record (which includes an API key) by navigating to https://dev.twitter.com/apps . Most integrations with the API will require you to identify your application to Twitter by way of an API key. On the Twitter platform, the term "API key" usually refers to  combination of two keys; an OAuth consumer key and the consumer secret.
 2. Download the Starter Kit Zip Portfolio from ([http://apimash.github.io/StarterKits/][8])
 3. Open the ***APIMASH_TwitterAPI_StarterKit*** Solution in Visual Studio 2012
 4. Add your Consumer Key and your Consumer Secret in the App.xaml file
 5. Compile and Run
 6. You or user of app will be asked to authorize the app for use in obtaining Twitter information the first time of application is used.

###Customization

***Step 1***. Add your Consumer Key and Consumer Secret in the App.xaml source file
 
***Step 2.*** APIMASH_TwitterAPI.cs provides struct called TweetAPIConstants which contains the 9 API’s from Twitter this app supports: 

 - statuses/user_timeline.json : Gets Tweets timeline for a user based upon their screen name
 - statues/home_timeline.json: Get Tweets home timeline for authorized user **Note:** Based on Twitter documentation and discussions this one is more volatile and possibly time based
 - search/tweets.json: Gets/Returns tweets based upon entered keyword search
 - trends/place.json: Gets/Returns Worldwide Twitter trends denoted by hashtags
 - followers/list.json: Get/Returns set of 20 Followers by screen name of user entered OR by authorized user if no screen name is entered
 - users/lookup.json: Gets/Returns information about multiple users (up to 100) by comma-separated screen names entered.
 - users/show.json: Gets/Returns information about one user by screen name entered

Each API returns an observable collection based upon type (Tweet, Trend, User). The Starter Kit can be customized to display results from any of these API’s. 

***Step 3.*** I encourage you to experiment by adding functionality to the User Lookup button that was added in v2.0.0 
as a placeholder for customization by users of the Starter kit, the Twitter API url for the User Lookup API call is already in the API Constants struct as noted above, and a placeholder is in the BuildAPIParams method for User Lookup. Use the Multiple or Individual User Lookup APIs.  Documentation can be found here: [https://dev.twitter.com/docs/api/1.1/get/users/show][9] and 
[https://dev.twitter.com/docs/api/1.1/get/users/lookup][10]

***Step 4.*** You can modify the parameters to each query to utilize paging of the results returned by the Twitter API.

***Step 5.*** Apply Grouping of Tweets or Users by different information; Ex. Group by Location or Date joined

***Step 6.*** Increase number of Tweets returned and implement Semantic Zoom. 

***Step 7.*** Implement a feature that allows user to select a Tweet and retrieve the retweets or replies and display information in an Item Detail page.

***Step 8.*** Implement a feature that allows user to select a Tweet retrieve the user details and display information in an User Detail page.

***Step 9.*** Mash up Twitter Tweets or Users by Location by leveraging Maps and GPS service like Bing (check out [the Bing Map Starter Kit][11]!)

***Step 10.*** Mash up Twitter Tweets with FaceBook to have apps post to both simultaneously (check out the Facebook Starter Kit)


##DISCLAIMER: 

The sample code described herein is provided on an "as is" basis, without warranty of any kind, to the fullest extent permitted by law. Both Microsoft and I do not warrant or guarantee the individual success developers may have in implementing the sample code on their development platforms or in using their own Web server configurations. 

Microsoft and I do not warrant, guarantee or make any representations regarding the use, results of use, accuracy, timeliness or completeness of any data or information relating to the sample code. Microsoft and I disclaim all warranties, express or implied, and in particular, disclaims all warranties of merchantability, fitness for a particular purpose, and warranties related to the code, or any service or software related thereto. 

Microsoft and I shall not be liable for any direct, indirect or consequential damages or costs of any type arising out of any action taken by you or others related to the sample code.


----------

##Change Log
###v1.0.0
Initial Twitter Starter Kit loaded to GitHub
###v2.0.0
Updated the Twitter Starter Kit UI significantly, added feature to Get a specific User followers, added App Bar 


  [1]: https://raw.github.com/apimash/StarterKits/master/Windows%20Starter%20Kits/APIMASH_Twitter_StarterKit/TwitterAPI_Start_ScreenShot.png "APIMASH Twitter Starter Kit on Start Screen"
  [2]: https://raw.github.com/apimash/StarterKits/master/Windows%20Starter%20Kits/APIMASH_Twitter_StarterKit/TwitterAPI_HomeScreen_AppBar.png "App Start Page"
  [3]: https://raw.github.com/apimash/StarterKits/master/Windows%20Starter%20Kits/APIMASH_Twitter_StarterKit/TwitterAPI_TweetsbyHomeTimeline.png "Authorized User Home Timeline"
  [4]: https://raw.github.com/apimash/StarterKits/master/Windows%20Starter%20Kits/APIMASH_Twitter_StarterKit/TwitterAPI_TweetsbyUserTimeline.png "Specified User Timeline"
  [5]: https://raw.github.com/apimash/StarterKits/master/Windows%20Starter%20Kits/APIMASH_Twitter_StarterKit/TwitterAPI_TweetsbySearchTopic.png "Keyword Search Tweets"
  [6]: https://raw.github.com/apimash/StarterKits/master/Windows%20Starter%20Kits/APIMASH_Twitter_StarterKit/TwitterAPI_WorldwideTrendingTopics.png "Show Worldwide Trends"
  [7]: https://raw.github.com/apimash/StarterKits/master/Windows%20Starter%20Kits/APIMASH_Twitter_StarterKit/TwitterAPI_FollowersbyUser.png "User Followers"
  [8]: http://apimash.github.io/StarterKits/ "APIMash Starter Kits"
  [9]: https://dev.twitter.com/docs/api/1.1/get/users/show "Twitter User Lookup for One User"
  [10]: https://dev.twitter.com/docs/api/1.1/get/users/lookup "Twitter API for Multiple User Lookup"
  [11]: https://dev.twitter.com/docs/api/1.1/get/users/lookup "Twitter API for Multiple User Lookup"
