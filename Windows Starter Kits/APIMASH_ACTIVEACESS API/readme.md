#Title:  ACTIVE ACCESS APIMASHUP STARTKIT
Active Access APIMASHUP STARTERKIT 
##Date: 2/30/2013
##Version: v0.0.2
##Author(s): Maria Naggaga


----------
###Description

The Active Access Starter Kit is an HTML5 and JS  Windows 8 App  using the blank template. It leverages the Active Access API to populate a sporting , camping, concert etc based on a geographical location. 
APIMASH_ACTIVEACCESS API_StarterKit shows you how to interact with an external webservice((in this case Active Access API) using the datadapter, Binding.ListAPI , ListDataSourceAPI, and how to display the grouped data using custom group and item datasources. 

Starter Kit was inspired by the <a href="http://code.msdn.microsoft.com/windowsapps/ListView-custom-data-4dcfb128/sourcecode?fileId=50893&pathId=1185249808">`[HTML ListView working data sources sample][1]`
This scenario demonstrates how to create a data adapter to interface with a web
service. A data adapter is the component to interface between the list view and
 the supply of data, enabling virtualization. 

This specific example uses XmlHttpRequest
 to access the Active Access search feature of <a href="http://developer.active.com"> [Active Access][2]</a>
 which is exposed as a web service.Key Rates Limits: 2 call per second and 10,000 calls per day.

###Features
 -  Make Calls to the  Active Access API - [WinJS.xhr][3] 
 - Creates a custom [IListDataAdapter][4] that connects to a web service and displays the data in a [ListView][5] control.
 -Implements the IListDataAdapter interface and creates a custom IListDataSource by inheriting from the [VirtualizedDataSource][6] calls. 


 
###Requirements

 - [Sign up for Mashery API Developers Key][7]
 - [Active Access Search API Key][8]
 - For Future Versions :  [Bing Maps SDK][9]
 - Windows Store Account 

 

###Setup

 1. Step 1 : Setup your developer's environment.  Install  Windows 8 and Visual Studio 2012. If you are on a Mac [Download a VM][10] .
 2. Step 2: Download the Starter Kits from [Github][11]

###Kit Customization 

 1. Open the ActiveAccessAPI.JSGo to the
    getCount:Function ( ) edit the following: 

Var 
    requestStr to call the data you want. 

Code snippet 
 
                // Build up a URL to request 50 items so we can get the count if less than that
                var requestStr = "http://api.amp.active.com/search/"
                    + "?v=json"
                    + "&k='" + that._query + "'"
                    + "&l=New York City'" // this can also be a zip code , latitude or longitude
                    + "&r=50" // radius number consider using this search content in the future 
                    + "&m=''"// metadata the ultimate search.  Split up and make into drop boxes natural language applies, date time 
                    + "&f='actvities'"// other options are result, trainining, and articles. Consider making an app focused on each one
                    + "&s='relevance'" //other options include date_asc, dat_desc switch this option to a drop down box
                    + "&num=25"
                    + "&page=50"
                    //+ "&api_key=";
                    +"&api_key="+that._devkey+"";// change key
2.  Return the Promise by making an XMLHttpRequest to the server 
return WinJS.xhr({ url: requestStr}).then(

                    //Callback for success
                    function (request)
                    {
                        var results = [], count;

                        // Use the JSON parser on the results, safer than eval
                        var obj = JSON.parse(request.responseText);

                        // Verify if the service has returned activities
                        if (obj.numberOfResults)
                        {
                            var items = obj._results;

                            // Data adapter results needs an array of items of the shape:
                            // items =[{ key: key1, data : { field1: value, field2: value, ... }}, { key: key2, data : {...}}, ...];
                            // Form the array of results objects
                            for (var i = 0, itemsLength = items.length; i < itemsLength; i++)
                            {
                                var dataItem = items[i];
                                results.push({
                                    key: (fetchIndex + i).toString(),
                                    data: { // edit the data request below to customize your display choices 
                                        title: dataItem.title,
                                        thumbnail: dataItem.meta.image1,
                                        width: dataItem.Width,
                                        height: dataItem.Height,
                                        linkurl: dataItem.SourceUrl,
                                        url: dataItem.MediaUrl,
                                        description : dataItem.meta.description
                                    }
                                });
                            }
         

----------
Screen Shots 

Start Screen
![Start Screen][12] 

Enter Your API KEY and  activity Search
![Key&query][13]

Search Results 
![results][14]
----------


##Change Log
###v0.0.0
Add version data here


  [2]: a href="http://developer.active.com


  [1]: http://code.msdn.microsoft.com/windowsapps/ListView-custom-data-4dcfb128/sourcecode?fileId=50893&pathId=1185249808
  [2]: http://msdn.microsoft.com/en-us/library/windows/apps/br229787.aspx
  [3]: http://msdn.microsoft.com/en-us/library/windows/apps/br229787.aspx
  [4]: http://msdn.microsoft.com/library/windows/apps/BR212603
  [5]: http://msdn.microsoft.com/library/windows/apps/BR211837
  [6]: http://msdn.microsoft.com/en-us/library/windows/apps/hh701413.aspx
  [7]: http://developer.mashery.com/apis
  [8]: http://developer.active.com/
  [9]: http://visualstudiogallery.msdn.microsoft.com/bb764f67-6b2c-4e14-b2d3-17477ae1eaca
  [10]: http://msdn.microsoft.com/en-us/library/windows/apps/jj945492.aspx
  [11]: http://apimash.github.io/StarterKits/
  [12]: https://ghrheg.blu.livefilestore.com/y2poVWhU__Hcl8RDqeBjiAdFpp1tSDnYTw9760Er6ueBAbzIR9jAL-qIIIRcLTGTKt7armIZFAv03cBTByvPl515hxP9ivwIjeVvka2Dfb_VP0/screenshot_05302013_092011.png?psid=1
  [13]: https://hxpi7w.blu.livefilestore.com/y2pFp9UsgrI68U7NamgJnaNCl2bWGQUi4X5IsFvJV-HSW03PUPL7-YXLeoijEoafDMfTO8S7Wk1AA2MXYFm4_GZ_MEIV6X4zeYzmprCGC8dplg/screenshot_05302013_090942.png?psid=1
  [14]: https://cwroea.blu.livefilestore.com/y2pgLWxr4kUDCqdaOXN7I4KfaR9mtiIKtMRfmnc85zbD9xkAp0MYAAWIfPXaP-NdFpGCHNwVD2akJb1CyiJgAMSkDpWjt9SKa6WR7bpSXo2HCg/screenshot_05302013_091434.png?psid=1
