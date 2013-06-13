// LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt   <== yes, there's a space in it, dont ask....
// APIMash - http://bit.ly/apimash
// Joe Healy / jhealy@microsoft.com / josephehealy@hotmail.com / @devfish

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace APIMASH_WikiPediaLib
{
    [DataContract]
    public class APIMASH_OM
    {
        public void Copy(geonamesCollection geocoll)
        {
            geocoll.Items.Clear();

            int _length = geonames.Length;
            for (int ii = 0; ii < _length; ii++)
            {
                geonameItem gn = new geonameItem(
                    geonames[ii].summary, geonames[ii].distance, geonames[ii].rank, geonames[ii].title, geonames[ii].wikipediaUrl,
                    geonames[ii].elevation, geonames[ii].countryCode, geonames[ii].lat, geonames[ii].lng, geonames[ii].lang, 
                    geonames[ii].geoNameId, geonames[ii].thumbnailImg, geonames[ii].feature);
                
                geocoll.Items.Add(gn);
            }
        }

        public void Copy(ObservableCollection<APIMASH_WikiPediaLib.geonameItem> geocoll )
        {
            geocoll.Clear();

            int _length = geonames.Count();
            for (int ii = 0; ii < _length; ii++)
            {
                geonameItem gn = new geonameItem(
                    geonames[ii].summary, geonames[ii].distance, geonames[ii].rank, geonames[ii].title, geonames[ii].wikipediaUrl,
                    geonames[ii].elevation, geonames[ii].countryCode, geonames[ii].lat, geonames[ii].lng, geonames[ii].lang, 
                    geonames[ii].geoNameId, geonames[ii].thumbnailImg, geonames[ii].feature);
                
                geocoll.Add(gn);
            }
        }

        [DataMember( Name="geonames") ]
        public geoname[] geonames { get; set; }
    }
}
