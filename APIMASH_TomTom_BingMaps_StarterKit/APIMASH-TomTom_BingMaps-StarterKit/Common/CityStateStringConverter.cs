using APIMASH;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

//
// LICENSE: http://aka.ms/LicenseTerms-SampleApps
//

namespace APIMASH_StarterKit.Common
{
    public sealed class CityStateStringConverter : IValueConverter
    {
        // convert ANY incoming type that has city field and a state abbreviation to "City, State" 
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                // use a dynamic to allow any type in
                dynamic address = value;
                String city = (address.City ?? String.Empty).Trim();
                String state = (address.State ?? String.Empty).Trim();

                // look up the state/provincial abbreviation
                if (StateLookup.ContainsKey(state)) state = StateLookup[state];

                // return a formatted value
                if (String.IsNullOrEmpty(city))
                    return state;
                else
                    return String.Format("{0}, {1}", city, state);
            }
            catch
            {
                // eat the exception (probably from non-existing property on dynamic type) and just return the original value
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Abbreviation dictionary for US states and Canadian provinces
        /// </summary>
        private Dictionary<String, String> StateLookup = new Dictionary<string, string>(50)
        {
            {"AL","Alabama"},
            {"AK","Alaska"},
            {"AZ","Arizona"},
            {"AR","Arkansas"},
            {"CA","California"},
            {"CO","Colorado"},
            {"CT","Connecticut"},
            {"DE","Delaware"},
            {"DC","District of Columbia"},
            {"FL","Florida"},
            {"GA","Georgia"},
            {"HI","Hawaii"},
            {"ID","Idaho"},
            {"IL","Illinois"},
            {"IN","Indiana"},
            {"IA","Iowa"},
            {"KS","Kansas"},
            {"KY","Kentucky"},
            {"LA","Louisiana"},
            {"ME","Maine"},
            {"MD","Maryland"},
            {"MA","Massachusetts"},
            {"MI","Michigan"},
            {"MN","Minnesota"},
            {"MS","Mississippi"},
            {"MO","Missouri"},
            {"MT","Montana"},
            {"NE","Nebraska"},
            {"NV","Nevada"},
            {"NH","New Hampshire"},
            {"NJ","New Jersey"},
            {"NM","New Mexico"},
            {"NY","New York"},
            {"NC","North Carolina"},
            {"ND","North Dakota"},
            {"OH","Ohio"},
            {"OK","Oklahoma"},
            {"OR","Oregon"},
            {"PA","Pennsylvania"},
            {"RI","Rhode Island"},
            {"SC","South Carolina"},
            {"SD","South Dakota"},
            {"TN","Tennessee"},
            {"TX","Texas"},
            {"UT","Utah"},
            {"VT","Vermont"},
            {"VA","Virginia"},
            {"WA","Washington"},
            {"WV","West Virginia"},
            {"WI","Wisconsin"},
            {"WY","Wyoming"},
            {"AB","Alberta"},
            {"BC","British Columbia"},
            {"MB","Manitoba"},
            {"NB","New Brunswick"},
            {"NL","Newfoundland and Labrador"},
            {"NS","Nova Scotia"},
            {"NT","Northwest Territories"},
            {"NU","Nunavut"},
            {"ON","Ontario"},
            {"PE","Prince Edward Island"},
            {"QC","Quebec"},
            {"SK","Saskatchewan"},
            {"YT","Yukon"}
        };
    }
}
