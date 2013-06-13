/*
 * LICENSE: http://opensource.org/licenses/ms-pl 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace APIMASH_YelpLib
{
    /*  
     *  Message - results of API request
     *    Properties: text, code, version
     *
     *  Businesses - zero to many matching Business entries
     *    Business:
     *      Properties - id, name, address/1/2/3, city, state, etc...
     *      Collection: Categories
     *      Collection: Neighborhoods
     *      Collection: Reviews
     *      
     *   Details at http://www.yelp.com/developers/documentation/search_api
     */

    [DataContract]
    public class Yelp_Response
    {
        private Message _message;
        [DataMember(Name = "message")]
        public Message message
        {
            get { return _message; }
            set { _message = value; }
        }

        private Business[] _businesses;
        [DataMember(Name = "businesses")]
        public Business[] businesses
        {
            get { return _businesses; }
            set { _businesses = value; }
        }

        public void Copy(ObservableCollection<APIMASH_OM_Bindable> bindableColl)
        {
            foreach (var bi in _businesses.Select(b => new BusinessItem(b)))
            {
                bindableColl.Add(bi);
            }
        }
    }

    [DataContract]
    public class Message
    {
        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "code")]
        public int Code { get; set; }

        [DataMember(Name = "version")]
        public string Version { get; set; }
    }

    [DataContract]
    public class Business
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "address1")]
        public string Address1 { get; set; }
        [DataMember(Name = "address2")]
        public string Address2 { get; set; }
        [DataMember(Name = "address3")]
        public string Address3 { get; set; }

        [DataMember(Name = "city")]
        public string City { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }
        [DataMember(Name = "state_code")]
        public string StateCode { get; set; }

        [DataMember(Name = "zip")]
        public string ZIP { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }
        [DataMember(Name = "country_code")]
        public string CountryCode{ get; set; }

        [DataMember(Name = "distance")]
        public double Distance{ get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude{ get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude{ get; set; }

        [DataMember(Name = "mobile_url")]
        public string MobileUrl { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "phone")]
        public string Phone{ get; set; }

        [DataMember(Name = "photo_url")]
        public string PhotoUrl{ get; set; }

        [DataMember(Name = "photo_url_small")]
        public string PhotoUrlSmall{ get; set; }

        [DataMember(Name = "avg_rating")]
        public double AvgRating{ get; set; }

        [DataMember(Name = "rating_img_url")]
        public string RatingImgUrl{ get; set; }

        [DataMember(Name = "rating_img_url_small")]
        public string RatingImgUrlSmall { get; set; }

        [DataMember(Name = "review_count")]
        public int ReviewCount{ get; set; }

        [DataMember(Name = "reviews")]
        public List<Review> Reviews{ get; set; }

        [DataMember(Name = "nearby_url")]
        public string NearbyUrl{ get; set; }

        [DataMember(Name = "neighborhoods")]
        public List<Neighborhood> Neighborhoods{ get; set; }

        [DataMember(Name = "categories")]
        public List<Category> Categories{ get; set; }

        [DataMember(Name = "is_closed")]
        public bool IsClosed{ get; set; }
    }

    [DataContract]
    public class Category
    {
        [DataMember(Name = "category_filter")]
        public string CategoryFilter{ get; set; }

        [DataMember(Name = "name")]
        public string Name{ get; set; }

        [DataMember(Name = "search_url")]
        public string SearchUrl{ get; set; }
    }

    [DataContract]
    public class Neighborhood
    {
        [DataMember(Name = "name")]
        public string Name{ get; set; }

        [DataMember(Name = "url")]
        public string Url{ get; set; }
    }

    [DataContract]
    public class Review
    {
        [DataMember(Name = "date")]
        public string Date{ get; set; }

        [DataMember(Name = "id")]
        public string Id{ get; set; }

        [DataMember(Name = "mobile_uri")]
        public string MobileURI{ get; set; }

        [DataMember(Name = "rating")]
        public int Rating{ get; set; }

        [DataMember(Name = "rating_img_url")]
        public string RatingImgUrl{ get; set; }

        [DataMember(Name = "rating_img_url_small")]
        public string RatingImgUrlSmall{ get; set; }

        [DataMember(Name = "text_excerpt")]
        public string TextExcerpt{ get; set; }

        [DataMember(Name = "url")]
        public string Url{ get; set; }

        [DataMember(Name = "user_name")]
        public string UserName{ get; set; }

        [DataMember(Name = "user_photo_url")]
        public string UserPhotoUrl{ get; set; }

        [DataMember(Name = "user_photo_url_small")]
        public string UserPhotoUrlSmall{ get; set; }

        [DataMember(Name = "name")]
        public string Name{ get; set; }

        [DataMember(Name = "user_url")]
        public string UserUrl{ get; set; }
    }
}