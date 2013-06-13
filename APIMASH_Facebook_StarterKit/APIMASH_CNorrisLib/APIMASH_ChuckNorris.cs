using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

namespace APIMASH_CNorrisLib
{
    public sealed class APIMASH_CNorris
    {
        private static APIMASH_CNorris _APIMASH_APIName = new APIMASH_CNorris();

        private ObservableCollection<APIMASH_OM_Bindable> _all = new ObservableCollection<APIMASH_OM_Bindable>();
        public ObservableCollection<APIMASH_OM_Bindable> All
        {
            get { return this._all; }
        }

        public static IEnumerable<APIMASH_OM_Bindable> AllItems()
        {
            return _APIMASH_APIName._all;
        }

        public static void Copy(CNorrisJoke response)
        {
            try
            {
                _APIMASH_APIName._all.Clear();

                // 
                // implement copy from OM to BINDABLE OM here
                //
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
    }
}
