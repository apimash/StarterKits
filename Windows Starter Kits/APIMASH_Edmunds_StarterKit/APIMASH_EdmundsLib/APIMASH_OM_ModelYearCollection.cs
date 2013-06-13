using System.Runtime.Serialization;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

/*
 *
 *  A P I   M A S H 
 *
 * These classes provide the object model for the Edmunds Vehicle API modelYearHolder JSON payload
*/

namespace APIMASH_EdmundsLib
{
    [DataContract]
    public class ModelSpecCollection
    {
        [DataMember(Name="modelYearHolder")]
        public ModelSpec[] ModelSpecs { get; set; }    
    }

    [DataContract]
    public class ModelSpec
    {
        [DataMember(Name = "makeId")]
        public string MakeId { get; set; }
        [DataMember(Name = "makeName")]
        public string MakeName { get; set; }
        [DataMember(Name = "makeNiceName")]
        public string MakeNiceName { get; set; }
        [DataMember(Name = "modelId")]
        public string ModelId { get; set; }
        [DataMember(Name = "modelName")]
        public string ModelName { get; set; }
        [DataMember(Name = "modelNiceName")]
        public string ModelNiceName { get; set; }
        [DataMember(Name = "modelLinkCode")]
        public string ModelLinkCode { get; set; }
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "styles")]
        public Style[] Styles { get; set; }
        [DataMember(Name = "newDefaultStyle")]
        public NewDefaultStyle NewDefaultStyleLink { get; set; }
        [DataMember(Name = "usedDefaultStyle")]
        public UsedDefaultStyle UsedDefaultStyleLink { get; set; }
        [DataMember(Name = "equipment")]
        public Equipment[] EquipmentList { get; set; }
        [DataMember(Name = "squishVinFeatures")]
        public SquishVinFeature[] SquishVinFeatures { get; set; }
        [DataMember(Name = "subModels")]
        public SubModel[] SubModels { get; set; }
        [DataMember(Name = "midYear")]
        public bool MidYear { get; set; }
        [DataMember(Name = "year")]
        public long Year { get; set; }
        [DataMember(Name = "dateAvailable")]
        public string DateAvailable { get; set; }
        [DataMember(Name = "model")]
        public Model ModelDetail { get; set; }
        [DataMember(Name = "tmvCorePercents")]
        public TmvCorePercent[] TmvCorePercents { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "attributeGroups")]
        public AttributeGroups AttributeGroupDetails { get; set; }
        [DataMember(Name = "categories")]
        public Categories CategorieDetails { get; set; }
        [DataMember(Name = "publicationStates")]
        public string[] PublicationStates { get; set; }
        [DataMember(Name = "msrpLowBound")]
        public double MSRPLowBound { get; set; }
        [DataMember(Name = "msrpHighBound")]
        public double MSRPHighBound { get; set; }
    }

    [DataContract]
    public class Style
    {
        [DataMember(Name = "link")]
        public string Link { get; set; }
        [DataMember(Name = "publicationState")]
        public string PublicationState { get; set; }
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class NewDefaultStyle
    {
        [DataMember(Name = "link")]
        public string Link { get; set; }        
    }

    [DataContract]
    public class UsedDefaultStyle
    {
        [DataMember(Name = "link")]
        public string Link { get; set; }                
    }

    [DataContract]
    public class Equipment
    {
        [DataMember(Name = "link")]
        public string Link { get; set; }                
    }

    [DataContract]
    public class SquishVinFeature
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "year")]
        public string Year { get; set; }
        [DataMember(Name = "make")]
        public string Make { get; set; }
        [DataMember(Name = "model")]
        public string Model { get; set; }
        [DataMember(Name = "vinModelGroup")]
        public string VinModelGroup { get; set; }
        [DataMember(Name = "vehicleStyle")]
        public string VehicleStyle { get; set; }
        [DataMember(Name = "numOfDoors")]
        public long NumOfDoors { get; set; }
        [DataMember(Name = "manualAutomatic")]
        public string ManualAutomatic { get; set; }
        [DataMember(Name = "automaticType")]
        public string AutomaticType { get; set; }
        [DataMember(Name = "drivenWheels")]
        public string DrivenWheels { get; set; }
        [DataMember(Name = "fuelType")]
        public string FuelType { get; set; }
        [DataMember(Name = "cylinders")]
        public long Cylinders { get; set; }
        [DataMember(Name = "displacement")]
        public string Displacement { get; set; }
        [DataMember(Name = "compressorType")]
        public string CompressorType { get; set; }
        [DataMember(Name = "trimLevel")]
        public string TrimLevel { get; set; }
        [DataMember(Name = "specialIdentifier")]
        public string SpecialIdentifier { get; set; }
        [DataMember(Name = "styleLinkCode")]
        public string StyleLinkCode { get; set; }
        [DataMember(Name = "tmvCategory")]
        public string TmvCategory { get; set; }
        [DataMember(Name = "manufacturer")]
        public string Manufacturer { get; set; }
        [DataMember(Name = "manufacturerCode")]
        public string ManufacturerCode { get; set; }
        [DataMember(Name = "plant")]
        public string Plant { get; set; } 
    }

    [DataContract]
    public class SubModel
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "identifier")]
        public string Identifie { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "ruleType")]
        public string RuleType { get; set; }
        [DataMember(Name = "styleIds")]
        public long[] StyleIds { get; set; }
        [DataMember(Name = "publicationStates")]
        public string[] PublicationStates { get; set; }
        [DataMember(Name = "submodelNewDefaultStyle")]
        public NewDefaultStyle SubmodelNewDefaultStyle { get; set; }
        [DataMember(Name = "submodelUsedDefaultStyle")]
        public UsedDefaultStyle SubmodelUsedDefaultStyle { get; set; } 
    }

    [DataContract]
    public class TmvCorePercent
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "tmvCorePercent")]
        public double TmvCorePercentage { get; set; }
        [DataMember(Name = "trim")]
        public string Trim { get; set; }
        [DataMember(Name = "specialIdentifier")]
        public string SpecialIdentifier { get; set; }
        [DataMember(Name = "modelGroup")]
        public string ModelGroup { get; set; }
        [DataMember(Name = "bodyType")]
        public string BodyType { get; set; }
        [DataMember(Name = "driveType")]
        public string DriveType { get; set; }
        [DataMember(Name = "fuelType")]
        public string FuelType { get; set; }
        [DataMember(Name = "numCylinders")]
        public long NumCylinders { get; set; }
        [DataMember(Name = "aspiration")]
        public string Aspiration { get; set; }
        [DataMember(Name = "tmvModelName")]
        public string TmvModelName { get; set; } 
    }

    [DataContract]
    public class AttributeGroups
    {
        [DataMember(Name = "RELIABILITY_RATINGS", IsRequired = false)]
        public ReliabliityRatings RatingDetails { get; set; }
        [DataMember(Name = "MAIN", IsRequired = false)]
        public Main MainDetails { get; set; }
        [DataMember(Name = "LEGACY", IsRequired = false)]
        public Legacy LlegacyDetails { get; set; }
    }

    [DataContract]
    public class ReliabliityRatings
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "attributes")]
        public ReliablityRatingsAttributes Attributes { get; set; }

    }

    [DataContract]
    public class ReliablityRatingsAttributes
    {
        [DataMember(Name = "POWERTRAIN_QUALITY_MECHANICAL")]
        public Attribute PowertrainQualityMechanical { get; set; }
        [DataMember(Name = "OVERALL_QUALITY_DESIGN")]
        public Attribute OverallQualityDesign { get; set; }
        [DataMember(Name = "OVERALL_QUALITY")]
        public Attribute OverallQuality { get; set; }
        [DataMember(Name = "OVERALL_QUALITY_MECHANICAL")]
        public Attribute OverallQualityMechanical { get; set; }
        [DataMember(Name = "POWERTRAIN_QUALITY_DESIGN")]
        public Attribute PowertrainQualityDesign { get; set; }
        [DataMember(Name = "FEATURES_AND_ACCESS_QUALITY_DESIGN")]
        public Attribute FeaturesAndAccessQualityDesign { get; set; }
        [DataMember(Name = "BODY_AND_INTERIOR_QUALITY_MECHANICAL")]
        public Attribute BodyAndInteriorQualityMechanical { get; set; }
        [DataMember(Name = "FEATURES_AND_ACCESS_QUALITY_MECHANICAL")]
        public Attribute FeaturesAndAccessQualityMechanical { get; set; }
        [DataMember(Name = "BODY_AND_INTERIOR_QUALITY_DESIGN")]
        public Attribute BodyAndInteriorQualityDesign { get; set; }         
    }

    [DataContract]
    public class Main
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "attributes")]
        public MainAttributes Attibutes { get; set; }

    }

    [DataContract]
    public class MainAttributes
    {
        [DataMember(Name = "NAME", IsRequired = false)]
        public Attribute Name { get; set; }
        [DataMember(Name = "USE_IN_NEW", IsRequired = false)]
        public Attribute UseInNew { get; set; }
        [DataMember(Name = "YEAR", IsRequired = false)]
        public Attribute Year { get; set; }
        [DataMember(Name = "WORKFLOWSTATUS", IsRequired = false)]
        public Attribute Workflowstatus { get; set; }
        [DataMember(Name = "USE_IN_USED", IsRequired = false)]
        public Attribute UseInUsed { get; set; }
        [DataMember(Name = "USE_IN_PRE_PRODUCTION", IsRequired = false)]
        public Attribute UseInPreProduction { get; set; }
        [DataMember(Name = "USE_IN_FUTURE", IsRequired = false)]
        public Attribute UseInFuture { get; set; }
        [DataMember(Name = "MODEL_LINK_CODE", IsRequired = false)]
        public Attribute ModelLinkCode { get; set; }         
    }

    [DataContract]
    public class Legacy
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "attributes")]
        public LegacyAttributes Attributes { get; set; }
    }

    [DataContract]
    public class LegacyAttributes
    {
        [DataMember(Name = "ED_MODEL_ID")]
        public Attribute EdModelId { get; set; }       
    }

    [DataContract]
    public class Categories
    {
        [DataMember(Name = "Crossover")]
        public string[] Crossover { get; set; }
        [DataMember(Name = "PRIMARY_BODY_TYPE")]
        public string[] PrimaryBodyType { get; set; }
        [DataMember(Name = "Vehicle Size")]
        public string[] VehicleSize { get; set; }
        [DataMember(Name = "Vehicle Style")]
        public string[] VehicleStyle { get; set; }
        [DataMember(Name = "Vehicle Type")]
        public string[] VehicleType { get; set; }
        [DataMember(Name = "Tier Five  - Vehicle Market Perception")]
        public string[] VehicleMarketPerception { get; set; } 
    }
}
