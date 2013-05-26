/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

/*
 *
 *  A P I   M A S H 
 *
 * This class provides easy access to the Rotten Tomatoes API's supported by this starter kit
*/

namespace APIMASH_RottenTomatoes_StarterKit
{
    /// <summary>
    /// The GLOBALS class is a collection of global constants that define the Rotten Tomato API
    /// </summary>
    public static class Globals
    {
        // STEP 1. ADD YOUR DEV KEY HERE
        public static string ROTTEN_TOMATOES_API_KEY = @"apikey=[YOUR-DEV-KEY-HERE]";

        // API URI's for Movies and DVD's
        public static string ROTTEN_TOMATOES_API_MOVIES_BOXOFFICE = @"http://api.rottentomatoes.com/api/public/v1.0/lists/movies/box_office.json?limit=20&country=us&" + ROTTEN_TOMATOES_API_KEY;
        public static string ROTTEN_TOMATOES_API_MOVIES_INTHEATERS = @"http://api.rottentomatoes.com/api/public/v1.0/lists/movies/in_theaters.json?page_limit=20&page=1&country=us&" + ROTTEN_TOMATOES_API_KEY;
        public static string ROTTEN_TOMATOES_API_MOVIES_OPENING = @"http://api.rottentomatoes.com/api/public/v1.0/lists/movies/opening.json?limit=20&country=us&" + ROTTEN_TOMATOES_API_KEY;
        public static string ROTTEN_TOMATOES_API_MOVIES_UPCOMING = @"http://api.rottentomatoes.com/api/public/v1.0/lists/movies/upcoming.json?page_limit=20&page=1&country=us&" + ROTTEN_TOMATOES_API_KEY;
        public static string ROTTEN_TOMATOES_API_DVD_TOPRENTALS = @"http://api.rottentomatoes.com/api/public/v1.0/lists/dvds/top_rentals.json?limit=20&country=us&" + ROTTEN_TOMATOES_API_KEY;
        public static string ROTTEN_TOMATOES_API_DVD_CURRENTRELEASES = @"http://api.rottentomatoes.com/api/public/v1.0/lists/dvds/current_releases.json?page_limit=20&page=1&country=us&" + ROTTEN_TOMATOES_API_KEY;
        public static string ROTTEN_TOMATOES_API_DVD_NEWRELEASES = @"http://api.rottentomatoes.com/api/public/v1.0/lists/dvds/new_releases.json?page_limit=20&page=1&country=us&" + ROTTEN_TOMATOES_API_KEY;
        public static string ROTTEN_TOMATOES_API_DVD_UPCOMING = @"http://api.rottentomatoes.com/api/public/v1.0/lists/dvds/upcoming.json?page_limit=20&page=1&country=us&apikey=&" + ROTTEN_TOMATOES_API_KEY;

        // API URI for Movie Search
        public static string ROTTEN_TOMATOES_API_SEARCH = @"http://api.rottentomatoes.com/api/public/v1.0/movies.json?" + ROTTEN_TOMATOES_API_KEY + @"&q=";
    }
}
