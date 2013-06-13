/*
 * LICENSE: http://opensource.org/licenses/ms-pl) 
 */

/*
 *
 *  A P I   M A S H 
 *
 * This class provides easy access to the Univision API's supported by this starter kit
*/

namespace APIMASH_Univision_StarterKit
{
    /// <summary>
    /// The GLOBALS class is a collection of global constants that define the UNIVISION API
    /// </summary>
    public static class Globals
    {
        // DEV KEY
        public static string UNIVISION_API_KEY = @"YOUR-DEV-KEY-HERE";

        // 
        public static string UNIVISION_DEFAULT_QUERY = @"http://apiservice.univision.com/rest/feed/getFeed?url=%2Fsearch%2Farticles%3Fcount%3D10%26site%3Dnoticias%26tags%3DNovedades%26outputMode%3Dxml&api_key=" + UNIVISION_API_KEY;
        public static string UNIVISION_DEFAULT_QUERY_TITLE = @"Novedades";
        public static string UNIVISION_URL_BASE = @"http://apiservice.univision.com/rest/feed/getFeed?url=%2Fsearch%2Farticles%3Fcount%3D5%26site%3D";
        public static string UNIVISION_CHANNEL_ENTERTAINMENT = @"entretenimiento";
        public static string UNIVISION_CHANNEL_NOTICIAS = @"noticias";
        public static string UNIVISION_CHANNEL_ENTERTAINMENT_TITLE = @" Entretenimiento";
        public static string UNIVISION_CHANNEL_NOTICIAS_TITLE = @" Noticias";

        public static string UNIVISION_TAGS_PORTION = @"%26tags%3D";
        public static string UNIVISION_ENCODING_PORTION = @"%26outputMode%3Dxml&api_key=";
    }
}
