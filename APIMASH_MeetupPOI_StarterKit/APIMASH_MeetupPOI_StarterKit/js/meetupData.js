/*
 * LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
 */


(function () {
    "use strict";

    var list = new WinJS.Binding.List();
    var groupedItems = list.createGrouped(
        function groupKeySelector(item) {
            return item.group.key;
        },
        function groupDataSelector(item) {
            return item.group;
        }
    );
    var meetupUrl;

    getUpcomingMeetups();

    WinJS.Namespace.define("MeetupData", {
        items: groupedItems,
        groups: groupedItems.groups,
        getItemReference: getItemReference,
        getItemsFromGroup: getItemsFromGroup,
        resolveGroupReference: resolveGroupReference,
        resolveItemReference: resolveItemReference
    });

    // Get a reference for an item, using the group key and item title as a
    // unique reference to the item that can be easily serialized.
    function getItemReference(item) {
        return [item.group.key, item.name];
    }

    // This function returns a WinJS.Binding.List containing only the items
    // that belong to the provided group.
    function getItemsFromGroup(group) {
        return list.createFiltered(function (item) { return item.group.key === group.key; });
    }

    // Get the unique group corresponding to the provided group key.
    function resolveGroupReference(key) {
        for (var i = 0; i < groupedItems.groups.length; i++) {
            if (groupedItems.groups.getAt(i).key === key) {
                return groupedItems.groups.getAt(i);
            }
        }
    }

    // Get a unique item from the provided string array, which should contain a
    // group key and an item title.
    function resolveItemReference(reference) {
        for (var i = 0; i < groupedItems.length; i++) {
            var item = groupedItems.getAt(i);
            if (item.group.key === reference[0] && item.name === reference[1]) {
                return item;
            }
        }
    }

    function getUpcomingMeetups() {

        meetupUrl = "http://api.meetup.com/2/open_events?text_format=plain&and_text=False&limited_events=False&desc=False&offset=0&status=upcoming&country=us&sign=true&city=" + Common.meetupCity
            + "&state=" + Common.meetupState
            + "&page=" + Common.maxMeetupsToFind
            + "&key=" + Common.meetupKey
            + "&radius=" + Common.meetupDistance;
        if (Common.meetupKeywords) {
            meetupUrl = meetupUrl + "&text=" + Common.meetupKeywords;
        }

        // execute request to the Meetup API
        WinJS.xhr({ url: meetupUrl, responseType: "json" }).done(function (d) {
            var meetups = JSON.parse(d.responseText).results;
            var location = Common.meetupCity + ", " + Common.meetupState;

            meetups.forEach(function (item) {
                if (item.distance < 5) {
                    item.group.key = "0_5";
                    item.group.title = "Less than 5 mi";
                    item.group.subtitle = "Meetups less than 5 miles from " + location;
                }
                else if (item.distance >= 5 && item.distance < 10) {
                    item.group.key = "5_10";
                    item.group.title = "5 to 10 mi";
                    item.group.subtitle = "Meetups between 5 and 10 miles from " + location;
                }
                else if (item.distance >= 10 && item.distance < 25) {
                    item.group.key = "10_25"
                    item.group.title = "10 to 25 mi";
                    item.group.subtitle = "Meetups between 10 and 25 miles from " + location;
                }
                else {
                    item.group.key = "gt_25"
                    item.group.title = "More than 25 mi";
                    item.group.subtitle = "Meetups more than 25 miles from " + location;
                }
                if (item.venue) {
                    item.cityState = item.venue.city + ", " + item.venue.state;
                    list.push(item);
                }
            });
        },
        function (e) {
            // handle errors
        });
    }
})();
