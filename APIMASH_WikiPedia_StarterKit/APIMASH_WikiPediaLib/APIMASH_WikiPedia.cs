using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace APIMASH_WikiPediaLib
{
    public sealed class APIMASH_geonamesCollection
    {
        private static APIMASH_geonamesCollection _geonames = new APIMASH_geonamesCollection();

        private ObservableCollection<APIMASH_WikiPediaLib.geonameItem> _all = new ObservableCollection<APIMASH_WikiPediaLib.geonameItem>();
        public ObservableCollection<APIMASH_WikiPediaLib.geonameItem> All
        {
            get { return this._all; }
        }

        public void Copy(APIMASH_OM response)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(response.ToString());
                _all.Clear();

                foreach (APIMASH_WikiPediaLib.geoname gn in response.geonames)
                {
                    _all.Add( new geonameItem(gn) );
                }

                response.Copy(_all);

                // copy from response to a bindable collection
                System.Diagnostics.Debug.WriteLine("lets break here and see what comes in");
            }
            catch (Exception e)
            {
                throw (e);
            }
        } // copy
    }
}
