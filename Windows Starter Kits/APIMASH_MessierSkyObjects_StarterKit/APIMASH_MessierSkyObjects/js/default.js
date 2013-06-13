// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
//   <script src="http://www.worldwidetelescope.org/scripts/wwtsdk.aspx"  type="javaScript"></script>


(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    // declare global Worldwide Telescope object
    var wwt;
    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

   
    var imagePath = WinJS.Binding.converter(function (theName) {
        return "/images/cat/" + theName + ".jpg";
    });

    // export the converter so it's accessible to the html page
    WinJS.Namespace.define("APIMASH.Converters", {
        imagePath: imagePath
    });

    app.onactivated = function (args) {
     
        document.getElementById("WWTCanvas").style.width = window.outerWidth + "px";
        document.getElementById("WWTCanvas").style.height = (window.outerHeight-100) + "px";      

        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll());
          
            window.addEventListener("resize", handleResize);         
            imageList.addEventListener("iteminvoked", itemInvokedHandler, false);                   
        }
    };

    window.onload = loadpage;
    function loadpage() {    
        initialize();
    }

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    app.onsettings = function (args) {
        args.detail.applicationcommands = {           
            "about": {
                title: "About", href: "/about.html"
            }
        };
        WinJS.UI.SettingsFlyout.populateSettings(args);
    };
    app.start();

    function zoomToIndex(index) {

        if (APIMASH.MessierData[index] === null) {
            return;
        }

        var item = APIMASH.MessierData[index];

        annotation_title.innerText = item.name;
        annotation_desc.innerText  = item.desc;

        wwt.gotoRaDecZoom(item.ra, item.dec, 1, false);
    }

    // This function initializes the wwt object and registers the wwtReady event
    // once the initialization is done the wwtReady event will be fired
    function initialize() {       
        wwt = wwtlib.WWTControl.initControl("WWTCanvas");
        wwt.add_ready(wwtReady);
    }

    // This is the ready event function where you would put your custom code for WWT
    // following the initForWwt() call
    function wwtReady() {      
    }  

    function handleResize(eventArgs) {       
        document.getElementById("WWTCanvas").style.width = window.outerWidth + "px";
        document.getElementById("WWTCanvas").style.height = (window.outerHeight - 160) + "px";
    }

    function itemInvokedHandler(eventObject) {
        eventObject.detail.itemPromise.done(function (invokedItem) {
            // access item data from the itemPromise         
            zoomToIndex(invokedItem.index);
        });
    }   

})();
