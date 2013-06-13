// LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt   <== yes, there's a space in it, dont ask....
// APIMash - http://bit.ly/apimash
// Joe Healy / jhealy@microsoft.com / josephehealy@hotmail.com / @devfish

// More on storage at
// http://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh700362.aspx

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIMASH_WikiPedia_StarterKit.Common
{
    static class RoamingStorageHelper
    {
        static public bool write(string key, string value)
        {
            Windows.Storage.ApplicationDataContainer _roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            System.Diagnostics.Debug.Assert(key != null && key.Length > 0 && value!=null );
            System.Diagnostics.Debug.WriteLine("TODO: Check to see if Roaming is over the quota");
            bool _retval = true;
            _roamingSettings.Values[key] = value;
            System.Diagnostics.Debug.WriteLine(string.Format("saved {0}={1}", key, value));
            return _retval;
        }

        static public string read(string key)
        {
            Windows.Storage.ApplicationDataContainer _roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            System.Diagnostics.Debug.Assert( key != null );
            string _retval = string.Empty;
            _retval = _roamingSettings.Values[key] as string;
            System.Diagnostics.Debug.WriteLine(string.Format("retrieved {0}={1}", key, _retval));
            return _retval;
        }

        static public ulong quota
        {
            get
            {
                Windows.Storage.ApplicationData _appdata = Windows.Storage.ApplicationData.Current;
                ulong _quota = _appdata.RoamingStorageQuota;
                if (_quota <= 0.0)
                {
                    _quota = 100; // local dev runapparently doesn't return a quota
                    System.Diagnostics.Debug.WriteLine("!! roaming quota zero. adjusting to 100");
                }
                return _quota;
            }
        }
    }
}



