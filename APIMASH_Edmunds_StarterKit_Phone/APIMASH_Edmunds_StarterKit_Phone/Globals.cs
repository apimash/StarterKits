/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

/*
 *
 *  A P I   M A S H 
 *
 * This class provides easy access to the Edmunds Vehicle API's supported by this starter kit
*/

namespace APIMASH_Edmunds_StarterKit_Phone
{
    internal class Globals
    {
        // edmunds developer key
        public static string EDMUNDS_API_DEVKEY = "&api_key=[YOUR DEV KEY HERE]";

        // get initial lists of makes and models by year
        public static string EDMUNDS_API_FINDBYYEAR = @"http://api.edmunds.com/v1/api/vehicle/makerepository/findmakesbymodelyear?fmt=json" + EDMUNDS_API_DEVKEY + @"&year=";
        public static string EDMUNDS_API_FINDALL = @"http://api.edmunds.com/v1/api/vehicle/makerepository/findall?fmt=json" + EDMUNDS_API_DEVKEY + @"&year=";

        // model specs by make-model-year, using this to get style id so we can retrieve vehicle photos
        public static string EDMUNDS_API_MODELSPECS_MAKE = @"http://api.edmunds.com/v1/api/vehicle/modelyearrepository/foryearmakemodel?fmt=json&make=";
        public static string EDMUNDS_API_MODELSPECS_MODEL = @"&model=";
        public static string EDMUNDS_API_MODELSPECS_YEAR = @"&year=";

        // vehicle photos by style id
        public static string EDMUNDS_API_PHOTOS = @"http://api.edmunds.com/v1/api/vehiclephoto/service/findphotosbystyleid?fmt=json" + EDMUNDS_API_DEVKEY + @"&styleId=";
        public static string EDMUNDS_API_PHOTOBASE = @"http://media.ed.edmunds-media.com/";
    }
}
