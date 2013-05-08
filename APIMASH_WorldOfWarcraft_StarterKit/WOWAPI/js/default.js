/*
File: default.js
Author: David Isbitski - @TheDaveDev, david.isbitski@microsoft.com, http://about.me/davedev
Last Mod: 5/1/2013
Description: Helper JavaScript library for working with the World of Warcraft open APIs (http://blizzard.github.io/api-wow-docs/).
             Many thanks to Blizzard for creating the BEST MMO on the market and for caring about the community around
             it!

Latest Source: http://github.com/disbitski/win8wowapikit

*/
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var nav = WinJS.Navigation;
    var appData = Windows.Storage.ApplicationData.current;
    var locale;
    var defaultLocale = "en_US";
    var shareTitle;
    var defaultShareTitle = "WOW API Starter Kit";
    var shareMessage;
    var defaultShareMessage = "Check out this free Windows Store App Starter Kit for the World of Warcraft APIs! http://github.com/disbitski/win8wowapikit";
    

    app.addEventListener("activated", function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                //Roaming Settings
                if (appData.roamingSettings.values.hasKey("locale")) {
                    locale = appData.roamingSettings.values["locale"];
                }
                else {
                    locale = defaultLocale;
                }
                if (appData.roamingSettings.values.hasKey("shareTitle")) {
                    shareTitle = appData.roamingSettings.values["shareTitle"];
                }
                else {
                    shareTitle = defaultShareTitle;
                }
                if (appData.roamingSettings.values.hasKey("shareMessage")) {
                    shareMessage = appData.roamingSettings.values["shareMessage"];
                }
                else {
                    shareMessage = defaultShareMessage;
                }

            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
                locale = app.sessionState.locale;
                shareTitle = app.sessionState.shareTitle;
                shareMessage = app.sessionState.shareMessage;
                
            }

            if (app.sessionState.history) {
                nav.history = app.sessionState.history;
            }
            args.setPromise(WinJS.UI.processAll().then(function () {

                initialize();

                if (nav.location) {
                    nav.history.current.initialPlaceholder = true;
                    return nav.navigate(nav.location, nav.state);
                } else {
                    return nav.navigate(Application.navigator.home);
                }
            }));
        }
    });

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. If you need to 
        // complete an asynchronous operation before your application is 
        // suspended, call args.setPromise().
        app.sessionState.history = nav.history;
        app.sessionState.locale = locale;
        app.sessionState.shareTitle = shareTitle;
        app.sessionState.shareMessage = shareMessage;
        appData.roamingSettings.values["locale"] = locale;
        appData.roamingSettings.values["shareTitle"] = shareTitle;
        appData.roamingSettings.values["shareMessage"] = shareMessage;

    };

    function initialize() {
        //event listeners
        document.getElementById("cmdLocale").addEventListener("click", showLocales, false);
        document.getElementById("submitButton").addEventListener("click", updateLocales, false);

        //Share Contract
        var dataTransferManager = Windows.ApplicationModel.DataTransfer.DataTransferManager.getForCurrentView();
        dataTransferManager.addEventListener("datarequested", shareText);

        //About and Privacy Policy Settings Charm
        WinJS.Application.onsettings = function (e) {
            e.detail.applicationcommands = {
                "aboutSettings": { title: "About", href: "/html/about.html" },
                "privacySettings": { title: "Privacy Policy", href: "/html/privacy.html" }
            };
            WinJS.UI.SettingsFlyout.populateSettings(e);
        };
    }

    //Show flyout control for locale languages
    function showLocales() {
        if ($("#radioUS").val()==locale) {
            $("#radioUS").prop("checked", true);
        }
        if ($("#radioMX").val()==locale) {
            $("#radioMX").prop("checked", true);
        }
        if ($("#radioBR").val()==locale) {
            $("#radioBR").prop("checked", true);
        }
       
        var cmdLocaleBtn = document.getElementById("cmdLocale");
        document.getElementById("localesFlyout").winControl.show(cmdLocaleBtn);
    }

    //Update Locale and save to roaming user settings
    function updateLocales() {
        if ($("#radioUS").is(":checked")) {
            locale = $("#radioUS").val();
        }
        if ($("#radioMX").is(":checked")) {
            locale = $("#radioMX").val();
        }
        if ($("#radioBR").is(":checked")) {
            locale = $("#radioBR").val();
        }

        appData.roamingSettings.values["locale"] = locale;  
        document.getElementById("localesFlyout").winControl.hide();
    }

    //Share out text from within the app ( Realm status, character name, etc)
    function shareText(e) {
        var request = e.request;
        
        request.data.properties.title = app.sessionState.shareTitle==null ? shareTitle : app.sessionState.shareTitle;
        request.data.setText(app.sessionState.shareMessage == null ? shareMessage : app.sessionState.shareMessage);

    }

    app.start();
})();
