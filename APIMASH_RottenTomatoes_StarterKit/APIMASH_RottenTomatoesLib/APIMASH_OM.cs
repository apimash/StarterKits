using System.Runtime.Serialization;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

/*
 *
 *  A P I   M A S H 
 *
 * These classes define the object model for the Rotten Tomatoes API
*/

namespace APIMASH_RottenTomatoesLib
{
    [DataContract]
    public class RottenTomatoesMovies
    {
        [DataMember(Name="total", IsRequired=false)]
        public int Total { get; set; }

        [DataMember(Name="movies")]
        public Movie[] Movies { get; set; }

        [DataMember(Name="links")]
        public SelfLinks SelfLinks { get; set; }

        [DataMember(Name="link_template")]
        public string LinkTemplate { get; set; }
    }

    [DataContract]
    public class Movie
    {
        [DataMember(Name="id")]
        public string Id { get; set; }

        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="year")]
        public int Year { get; set; }

        [DataMember(Name="mpaa_rating")]
        public string MPAARating { get; set; }

        [DataMember(Name="runtime")]
        public int Runtime { get; set; }

        [DataMember(Name="critics_concensus")]
        public string CriticsConsensus { get; set; }

        [DataMember(Name="releasedates")]
        public ReleaseDates ReleaseDates { get; set; }

        [DataMember(Name="ratings")]
        public Ratings Ratings { get; set; }

        [DataMember(Name="synopsis")]
        public string Synopsis { get; set; }

        [DataMember(Name="posters")]
        public Posters Posters { get; set; }

        [DataMember(Name="abridged_cast")]
        public CastMember [] AbridgedCast { get; set; }

        [DataMember(Name="alternate_ids")]
        public AlternateIds AlternateIds { get; set; }

        [DataMember(Name="links")]
        public MovieLinks Links { get; set; }
    }

    [DataContract]
    public class ReleaseDates
    {
        [DataMember(Name="theater")]
        public string Theater { get; set; }
    }

    [DataContract]
    public class Ratings
    {
        [DataMember(Name="critics_rating")]
        public string CriticsRating { get; set; }

        [DataMember(Name="critics_score")]
        public int CriticsScore { get; set; }

        [DataMember(Name="audience_rating")]
        public string AudienceRating { get; set; }

        [DataMember(Name="audience_score")]
        public int AudienceScore { get; set; }
    }

    [DataContract]
    public class Posters
    {
        [DataMember(Name="thumbnail")]
        public string Thumbnail { get; set; }

        [DataMember(Name="profile")]
        public string Profile { get; set; }

        [DataMember(Name="detailed")]
        public string Detailed { get; set; }

        [DataMember(Name="original")]
        public string Original { get; set; }
    }

    [DataContract]
    public class CastMember
    {
        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="id")]
        public string Id { get; set; }

        [DataMember(Name="characters", IsRequired=false)]
        public string[] Characters { get; set; }
    }

    [DataContract]
    public class AlternateIds
    {
        [DataMember(Name="imdb")]
        public string Imdb { get; set; }
    }

    [DataContract]
    public class MovieLinks
    {
        [DataMember(Name = "self")]
        public string Self { get; set; }

        [DataMember(Name = "alternate")]
        public string Alternate { get; set; }

        [DataMember(Name = "cast")]
        public string Cast { get; set; }

        [DataMember(Name = "clips")]
        public string Clips { get; set; }

        [DataMember(Name = "reviews")]
        public string Reviews { get; set; }

        [DataMember(Name = "similar")]
        public string Similar { get; set; }
    }

    [DataContract]
    public class SelfLinks
    {
        [DataMember(Name="self")]
        public string Self { get; set; }

        [DataMember(Name="next", IsRequired=false)]
        public string Next { get; set; }

        [DataMember(Name="alternate")]
        public string Alternate { get; set; }

        [DataMember(Name = "rel", IsRequired = false)]
        public string Rel { get; set; }
    }


    [DataContract]
    public class PreviewLinks
    {
        [DataMember(Name = "alternate")]
        public string Alternate { get; set; }
    }

    [DataContract]
    public class MoviePreviews
    {
        [DataMember]
        public Clip[] Clips { get; set; }

        [DataMember(Name="links")]
        public SelfLinks Links { get; set; }
    }

    [DataContract]
    public class Clip
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "duration")]
        public string Duration { get; set; }

        [DataMember(Name = "thumbnail")]
        public string Thumbnail { get; set; }

        [DataMember(Name = "links")]
        public PreviewLinks Links { get; set; }
    }

    [DataContract]
    public class MovieReviews
    {
        [DataMember(Name="total")]
        public int Total { get; set; }

        [DataMember(Name="reviews")]
        public Review[] Reviews { get; set; }
    }

    [DataContract]
    public class Review
    {
        [DataMember(Name = "critic")]
        public string Critic { get; set; }

        [DataMember(Name = "date")]
        public string Date { get; set; }
        
        [DataMember(Name = "freshness")]
        public string Freshness { get; set; }
        
        [DataMember(Name = "publication")]
        public string Publication { get; set; }
        
        [DataMember(Name = "quote")]
        public string Quote { get; set; }
        
        [DataMember(Name = "links")]
        public ReviewLink Link { get; set; }
    }

    [DataContract]
    public class ReviewLink
    {
        [DataMember(Name = "review")]
        public string Review { get; set; }
    }
}
