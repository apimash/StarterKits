/*
 * LICENSE: http://opensource.org/licenses/ms-pl 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIMASH_YelpLib
{
    public sealed class APIMASH_Yelp
    {
        private static APIMASH_Yelp _APIMASH_APIName = new APIMASH_Yelp();

        private ObservableCollection<APIMASH_OM_Bindable> _all = new ObservableCollection<APIMASH_OM_Bindable>();
        public ObservableCollection<APIMASH_OM_Bindable> All
        {
            get { return this._all; }
        }

        public static IEnumerable<APIMASH_OM_Bindable> AllItems()
        {
            return _APIMASH_APIName._all;
        }

        public static void Copy(Yelp_Response response)
        {
            try
            {
                _APIMASH_APIName._all.Clear();
                response.Copy(_APIMASH_APIName.All);
            }
            catch (Exception e)
            {
                throw (e);
            }
        }
    }
}
