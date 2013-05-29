// LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt   <== yes, there's a space in it, dont ask....
// APIMash - http://bit.ly/apimash
// Joe Healy / jhealy@microsoft.com / josephehealy@hotmail.com / @devfish

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIMASH_WikiPediaLib.geonamesHelpers
{
     abstract public class geonamesHelperAbstract
    {
        abstract public string DemoURL{ get;  }
        abstract public string TargetURL { get;  }
        abstract public string UserName { get; set; }
        abstract public string PrePopulatedURL { get; }
    }

     public class FindNearbyWikipediaHelper : geonamesHelperAbstract
     {
         private string m_baseurl = @"http://api.geonames.org/findNearbyWikipediaJSON?postalcode={0}&country={1}&radius={2}&username={3}";
         private string m_workingDemoURL = @"http://api.geonames.org/findNearbyWikipediaJSON?postalcode=33702&country=US&radius=10&username=demo";

         public override string UserName { get; set; }
         public int PostalCode { get; set; }
         public string Country { get;set;}
         public int Radius {get;set;}

         public FindNearbyWikipediaHelper()
         {
             this.UserName = string.Empty;
         }

         public FindNearbyWikipediaHelper(int p_postalCode, string p_country, int p_radius, string p_userName )
         {
             this.UserName = p_userName;
             this.PostalCode = p_postalCode;
             this.Country = p_country;
             this.UserName = p_userName;
             this.Radius = p_radius;
         }

         public override string DemoURL
         {
             get { return this.m_workingDemoURL; }
         }
         public override string PrePopulatedURL
         {
             get
             {
                 if (this.UserName.Length <= 0)
                     throw new ArgumentException("username cannot be blank", "p_userName");

                 this.PostalCode = 33702;
                 this.Country = "US";
                 this.Radius = 10;

                 return TargetURL;
             }
         }
         public override string TargetURL
         {
             get
             {
                 if (this.UserName.Length <= 0)
                     throw new ArgumentException("username cannot be blank", "p_userName");

                 return string.Format(m_baseurl, PostalCode, Country, Radius , UserName);
             }
         }
     }


     public class WikipediaSearchHelper : geonamesHelperAbstract
     {
         private string m_baseurl = "http://api.geonames.org/wikipediaSearchJSON?q={0}&maxRows={1}&username={2}";
         private string m_workingDemoURL = @"http://api.geonames.org/wikipediaSearchJSON?q=Tampa&maxRows=50&username=demo";

         public override string UserName { get; set; }
         public string Query { get; set; }
         public int MaxRows { get; set; }

         public WikipediaSearchHelper()
         {
             this.UserName = string.Empty;
             this.Query = string.Empty;
             this.MaxRows = 0;
         }

         public WikipediaSearchHelper(string p_userName, string p_query, int p_maxRows )
         {
            this.UserName = p_userName;
            this.Query = p_query;
            this.MaxRows = p_maxRows;
         }

         public override string DemoURL
         {
             get { return this.m_workingDemoURL; }
         }
         public override string PrePopulatedURL
         {
             get
             {
                if (this.UserName.Length <= 0)
                    throw new ArgumentException("username cannot be blank", "p_userName");
                    
                this.Query="Tampa";
                this.MaxRows=50;

                return TargetURL;
             }
         }
         public override string TargetURL
         {
             get
             {
                 if (this.UserName.Length <= 0)
                     throw new ArgumentException("username cannot be blank", "p_userName");

                 return string.Format(m_baseurl, Query, MaxRows, UserName);
             }
         }
     }

    public class WikipediaBoundingBoxHelper : geonamesHelperAbstract
    {
        private string m_baseurl = "http://api.geonames.org/wikipediaBoundingBoxJSON?north={0}&south={1}&east={2}&west={3}&username={4}";
        private string m_workingDemoURL = @"http://api.geonames.org/wikipediaBoundingBoxJSON?north=44.1&south=-9.9&east=-22.4&west=55.2&username=demo";

        public WikipediaBoundingBoxHelper()
        {
            this.UserName = string.Empty;
            this.North = this.East = this.West = this.South = 0.0;
        }

        public WikipediaBoundingBoxHelper(string p_userName, double p_north, double p_south, double p_west, double p_east)
        {
            this.UserName = p_userName;
            this.North = p_north;
            this.South = p_south;
            this.East = p_east;
            this.West = p_west;
        }

        public override string DemoURL
        {
            get { return this.m_workingDemoURL; }
        }
        public override string PrePopulatedURL
        {
            get 
            {
                if (this.UserName.Length <= 0)
                    throw new ArgumentException("username cannot be blank", "p_userName");

                this.North = 44.1;
                this.South = -9.9;
                this.East = -22.4;
                this.West = 55.2;
                return TargetURL;
            }
        }
        public override string TargetURL
        {
            get
            {
                if (this.UserName.Length <= 0)
                    throw new ArgumentException("username cannot be blank", "p_userName");

                return string.Format(m_baseurl, North, South, East, West, UserName);
            }
        }

        public override string UserName { get; set; }
        public double North { get; set; }
        public double South { get; set; }
        public double East { get; set; }
        public double West { get; set; }
    }
}
