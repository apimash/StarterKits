#Title APIMASH Univision Starter Kit
##Date: 5.10.2013
##Version: v1.0.0
##Author(s): Diego Lizarazo R.
##URL: http://github.com/apimash/starterkits

----------
###Description
The Univision API test is a XAML/C# Windows 8 app that demonstrates calling a Web Service that returned a simple XML payload from one of the best Spanish-language content and services provider.

![alt text][1]

###Features
• Invokes the Internet Univision API (http://developer.univision.com/)
• Demonstrates how to add XML feeds to C#
• Provides a baseline for a Windows 8 Store App

###Requirements
• Windows 8
• Visual Studio 2012 Express for Windows 8 or higher


###Setup
• Download the Starter Kit Zip Portfolio from  http://apimash.github.io/StarterKits/ 
• Open the Solution in Visual Studio
• Compile and Run

###Customization

Step 1. Add your Developer Key in the Globals.cs source file on line 20

Step 2. Global.cs provides URI's for 2 of the Univision sites. 

 - **UNIVISION CHANNEL ENTERTAINMENT** - Search in the entertainment channel.    
 - **UNIVISION CHANNEL NOTICIAS** - Search in the news channel.
     
Each site returns the same XML payload format. The Starter Kit can be customized to display results from any of these sites. I encourage you to experiment  by simple changing the site selection in the ItemsPage.xaml.cs file on line 82, to include any channel that you want to define in Globals.cs


Step 3. You can modify the parameters to each query such as the limit of the number of articles to return in total per channel. Experiment by changing the ‘sites’ parameter on the ‘default query’ call on line 24 of Globals.cs. Visit the API [link text][2] to experiment on line.

Step 4. Gossip information is EXTREMELY  mashable! Experiment by combining Univision's API with Bing showing more information about the user's searches.



##DISCLAIMER: 

The sample code described herein is provided on an "as is" basis, without warranty of any kind, to the fullest extent permitted by law. Both Microsoft and I do not warrant or guarantee the individual success developers may have in implementing the sample code on their development platforms or in using their own Web server configurations. 

Microsoft and I do not warrant, guarantee or make any representations regarding the use, results of use, accuracy, timeliness or completeness of any data or information relating to the sample code. Microsoft and I disclaim all warranties, express or implied, and in particular, disclaims all warranties of merchantability, fitness for a particular purpose, and warranties related to the code, or any service or software related thereto. 

Microsoft and I shall not be liable for any direct, indirect or consequential damages or costs of any type arising out of any action taken by you or others related to the sample code.


----------

##Change Log
###v0.0.0
Add version data here


  [1]: https://raw.github.com/apimash/StarterKits/master/Windows%20Starter%20Kits/APIMASH_Univision_StarterKit/UnivisonAPIscreenshot.png "Screenshot"
  [2]: http://developer.univision.com/ "Univision API Documentation"
