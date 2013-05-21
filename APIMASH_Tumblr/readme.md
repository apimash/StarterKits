#APIMASH Tumblr Starter Kit
##Date: 5.10.2013
##Version: v1.0.1
##Author(s): Stacey Mulcahy 
##URL: http://github.com/apimash/starterkits

----------
###Description
The Tumblr Daily Lawls App is an application that shows how to call a web service ( tumblr ) and present a UI based on the data.
![Alt text](https://nkz01w.dm1.livefilestore.com/y2pQ9n58fsXoTpY_laEYMY2uIsg8n3frMHpD08vrR4vjL3OeFpNYWTYGb_HqpoyxZXjwzlqXnvublW2sBMtdmHEifHesHsf8mR4-Nrl4xJdeXw/APIMash_Tumblr.png?psid=1)
###Features
 - Invokes the Tumblr API (http://www.tumblr.com/docs/en/api/v2), specifically the tags endpoint
 - Demonstrates how to download an image to the pictures directory
 - Demonstrates how to write a file to the local application directory ( cacheing of data)
 - Provides a baseline for a Windows 8 Store App
 - Sample result call can be seen in the browser via this url http://api.tumblr.com/v2/tagged?tag=gif&api_key=[YOUR_APP_KEY]

###Requirements

 - Windows 8
 - Visual Studio 2012 Express for Windows 8 or higher
 - Tumblr API Developer key (http://www.tumblr.com/oauth/apps ) 
 - Utilizes jQuery version 2.0.0 (http://code.jquery.com/jquery-2.0.0.js)

###Setup

 - Download the Starter Kit Zip Portfolio from http://apimash.github.io/StarterKits/
 - Open the Solution in Visual Studio
 - Compile and Run
 **NOTE**: You will need to add your own developer signing certificate to the project, by opening the package.appxmanifest file, and switching to the Packaging tab. On the packaging tab, click the "Choose Certificate..." button, and in the resulting dialog, click the "Configure Certificate..." drop-down, and select "Create test certificate..." then click OK to dismiss all dialogs, and save the app manifest file.

###Customization
What is being searched for from the Tumblr api can be changed. In this example, "lol" and "gifs" are being searched for. Two ways exist to change
these parameters.
- Modify the developer key, which you will need to get from Tumblr.(http://www.tumblr.com/docs/en/api/v2)
- Modify the __tags array in the apimashtumblr.js and put in the items you want to search for
- Modify the call to get the data in tumblr.js to pass an array of tags you would like to search for
- The UI and overall experience can easily be changed by writing your own view logic in tumblr.js - replacing what is there. 

###Future Features
 - Include OAuth and OAuth related endpoints 

----------

##Change Log
###v1.0.1
- Modified readme

##DISCLAIMER: 
 
The sample code described herein is provided on an "as is" basis, without warranty of any kind, to the fullest extent permitted by law. Both Microsoft and I do not warrant or guarantee the individual success developers may have in implementing the sample code on their development platforms or in using their own Web server configurations. 
 
Microsoft and I do not warrant, guarantee or make any representations regarding the use, results of use, accuracy, timeliness or completeness of any data or information relating to the sample code. Microsoft and I disclaim all warranties, express or implied, and in particular, disclaims all warranties of merchantability, fitness for a particular purpose, and warranties related to the code, or any service or software related thereto. 
 
Microsoft and I shall not be liable for any direct, indirect or consequential damages or costs of any type arising out of any action taken by you or others related to the sample code.

