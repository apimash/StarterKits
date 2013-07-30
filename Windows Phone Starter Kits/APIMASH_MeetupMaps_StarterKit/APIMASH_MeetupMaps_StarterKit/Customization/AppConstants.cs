/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIMASH_MeetupMaps_StarterKit.Customization
{
    class AppConstants
    {
        // These members are provided for customization purposes
        // Add your Meetup API key below, and modify the other properties 
        // as desired to customize the results returned

        public static String meetupUri = "http://api.meetup.com/2/open_events.xml?text_format=plain&and_text=False&limited_events=False&desc=False&offset=0&status=upcoming&country=us&sign=true";
        public static String meetupKey = "YOUR_MEETUP_API_KEY_HERE";
        public static String meetupDistance = "50";
        public static String maxMeetupsToFind = "200";
        public static String meetupCity = "Seattle";
        public static String meetupState = "WA";
        public static String meetupKeywords = "JavaScript";
        public static String searchTerm = "coffee";

    }
}
