using System.Collections.Generic;
using System.Runtime.Serialization;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

/*
 *
 *  A P I   M A S H 
 *
 * These classes define the object model for the Edmunds Vehicle API makeHolder JSON payload
*/

namespace APIMASH_EdmundsLib
{
    [DataContract]
    public class MakeCollection
    {
        [DataMember(Name="makeHolder")]
        public Make[] Makes;
    }

    [DataContract]
    public class Make
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "models")]
        public Model[] Models { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "niceName")]
        public string NiceName { get; set; }
        [DataMember(Name = "manufacturer", IsRequired = false)]
        public string Manufacturer { get; set; }
        [DataMember(Name = "attributeGroups", IsRequired = false)]
        public AttributeGroup AttributeGroups { get; set; }
    }

    [DataContract]
    public class Model
    {
        [DataMember(Name="link")]
        public string Link { get; set; }
        [DataMember(Name="id")]
        public string Id { get; set; }
        [DataMember(Name="name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class AttributeGroup
    {
        [DataMember(Name="id")]
        public long Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "attribute")]
        public Attribute[] Attributes { get; set; }
    }

    [DataContract]
    public class Attribute
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "value")]
        public string Value { get; set; }
    }

    [DataContract]
    public class PhotoCollection : List<Photo>
    {
        //public Photo[] Photos { get; set; }
    }

    [DataContract]
    public class Photo
    {
        [DataMember(Name = "children")]
        public Child[] Children { get; set; }
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "captionTranscript")]
        public string CaptionTranscript { get; set; }
        [DataMember(Name = "subType")]
        public string SubType { get; set; }
        [DataMember(Name = "shotTypeAbbreviation")]
        public string ShotTypeAbbreviation { get; set; }
        [DataMember(Name = "photoSrcs")]
        public string[] PhotoSources { get; set; }
        [DataMember(Name = "source")]
        public string Source { get; set; }
        [DataMember(Name = "vdpOrder", IsRequired = false)]
        public long VDPOrder { get; set; }
    }

    [DataContract]
    public class Child
    {
        /* ?? */
    }
}
