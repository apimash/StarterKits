// LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt   <== yes, there's a space in it, dont ask....
// APIMash - http://bit.ly/apimash
// Joe Healy / jhealy@microsoft.com / josephehealy@hotmail.com / @devfish

using System.Runtime.Serialization;
using System.Text;

namespace APIMASH_WikiPediaLib
{
    [DataContract]
    public class geoname
    {
        [DataMember(Name = "geoNameId")]
        public int geoNameId;

        [DataMember(Name = "distance")]
        public float distance { get; set; }

        [DataMember(Name = "feature")]
        public string feature { get; set; }

        [DataMember(Name = "rank")]
        public int rank { get; set; }

        [DataMember(Name="summary")]
        public string summary { get; set; }

        [DataMember(Name = "title")]
        public string title { get; set; }

        [DataMember(Name ="wikipediaUrl")]
        public string wikipediaUrl { get; set; }

        [DataMember(Name = "elevation")]
        public int elevation { get; set; }

        [DataMember(Name = "countryCode")]
        public string countryCode { get; set; }

        [DataMember(Name = "lng")]
        public float lng { get; set; }

        [DataMember(Name = "lang")]
        public string lang { get; set; }

        [DataMember(Name = "lat")]
        public float lat { get; set; }

        [DataMember(Name = "thumbnailImg")]
        public string thumbnailImg { get; set; }

        public string ToNearbyPlaceString()
        {
            StringBuilder _sb = new StringBuilder(1024);
            
            if (this.title != null && this.title.Length > 0)
            {
                _sb.Append(title);
            }
            _sb.Append(distance.ToString());
            _sb.Append("m :: geo(");
            _sb.Append(lat);
            _sb.Append(",");
            _sb.Append(lng);
            _sb.Append(") :: ");
            if ( this.wikipediaUrl != null && this.wikipediaUrl.Length > 0)
            {
                _sb.Append("http://");
                _sb.Append(wikipediaUrl);
            }
            if ( this.summary != null && summary.Length > 0)
            {
                _sb.Append(" :: ");
                _sb.Append(summary);
            }
            if (geoNameId != 0)
            {
                _sb.Append( " :: " );
                _sb.Append( geoNameId);
            }

            return _sb.ToString();
        }

        public string ToWikipediaSearchResultString()
        {
            StringBuilder _sb = new StringBuilder(1024);
            if (geoNameId != 0)
            {
                if (_sb.Length > 0) _sb.Append(" :: ");
                _sb.Append(geoNameId);
            }
            else
            {
                if (_sb.Length > 0) _sb.Append(" :: ");
                _sb.Append("null");
            }

            if ( this.title != null && this.title.Length > 0)
            {
                if (_sb.Length > 0) _sb.Append(" :: ");
                _sb.Append(title);
            }

            if ( this.feature != null && this.feature.Length > 0)
            {
                if (_sb.Length > 0) _sb.Append(" :: feature:");
                _sb.Append(this.feature);
            }

            if (this.thumbnailImg != null && this.thumbnailImg.Length > 0)
            {
                if (_sb.Length > 0) _sb.Append(" :: thumb:");
                _sb.Append(this.thumbnailImg);
            }

            _sb.Append("m :: geo(");
            _sb.Append(lat);
            _sb.Append(",");
            _sb.Append(lng);
            _sb.Append(") :: ");
            if ( this.wikipediaUrl != null && wikipediaUrl.Length > 0)
            {
                if (_sb.Length > 0) _sb.Append(" :: ");
                _sb.Append("http://");
                _sb.Append(wikipediaUrl);
            }

            return _sb.ToString();
        }
    } // class
}
