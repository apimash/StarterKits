#Title: Wikipedia Geonames APIMASH
##Date: 2013-05-15
##Version: v1.0.0
##Author:  Joe Healy 
##URL: http://bit.ly/apimash
##License: Microsoft Public License - http://opensource.org/licenses/ms-pl
###Description
Geonames has an easy to access API that exposes Wikipedia articles that are georeferenced.  This sample code shows how to easily access the Wikipedia Webservice on geonames. 
Questions, concerns, comments please relay to jhealy@microsoft.com // josephehealy@hotmail.com // @devfish on twitter ...

###Features
 - findNearbyWikipedia - finds nearby Wikipedia entries by radius, and by latitude/longitude OR postal code
 - wikipediaSearch - Search for a place name and optionally text from the title of the article
 - wikipediaBoundingBox - Search for entries within a south, north, east, and west latitude and longitude bounded box
 - User name is stored in isolated storage between sessions.  No editing .cs or .js files to get it to run
 - XAML and CSharp only at this time.  More consumption samples in other languages coming soon

###Requirements
What do you need to run this project? 
- Windows 8
- Visual Studio 2012 Express for Windows 8 or higher
- JSON.NET form Newtonsoft (https://json.codeplex.com/)
- Geonames login for the web service API ( http://www.geonames.org/login )

###Setup
- Register at Geonames.org (  http://www.geonames.org/login )
- Download the Starter Kit Zip Portfolio from ( http://bit.ly/apimash )
- Open the Solution in Visual Studio
- Build and run the solution
- Go to the Settings => User Name menu and enter your user name (first time only)
- Compile and Run

###Customization
Step 1. Modify the calls to geonames helpers to 'mash in' with any input params you want.
Step 2. Geonames Wikipedia information is EXTREMELY  mashable! Experiment by combining Geonames Wikipedia with Bing Maps showing nearby points of interest (check out the Bing Map Starter Kit at http://bit.ly/apimash to see how to use Bing Maps)

##Change Log
###v1.0.0 Initial release with simple text output for apis, no parameter screens yet
###v1.0.0.1 License changed to bobfam %20 special version
###v1.0.0.2 Moved APIMASHLib local to main wikipedia project.  Bad practice but when in rome...

##DISCLAIMER: 
The sample code described herein is provided on an "as is" basis, without warranty of any kind, to the fullest extent permitted by law. Both Microsoft and I do not warrant or guarantee the individual success developers may have in implementing the sample code on their development platforms or in using their own Web server configurations. 
Microsoft and I do not warrant, guarantee or make any representations regarding the use, results of use, accuracy, timeliness or completeness of any data or information relating to the sample code. Microsoft and I disclaim all warranties, express or implied, and in particular, disclaims all warranties of merchantability, fitness for a particular purpose, and warranties related to the code, or any service or software related thereto. 
Microsoft and I shall not be liable for any direct, indirect or consequential damages or costs of any type arising out of any action taken by you or others related to the sample code.

