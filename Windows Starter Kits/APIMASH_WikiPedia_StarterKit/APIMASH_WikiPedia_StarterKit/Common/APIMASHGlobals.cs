// LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt   <== yes, there's a space in it, dont ask....
// APIMash - http://bit.ly/apimash
// Joe Healy / jhealy@microsoft.com / josephehealy@hotmail.com / @devfish

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// http://msdn.microsoft.com/en-us/library/ff650316.aspx
// http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.aspx
// http://blog.mrlacey.co.uk/2011/03/binding-to-static-classes-in-windows.html

namespace APIMASH_WikiPedia_StarterKit.Common
{
    public class APIMASHGlobals : INotifyPropertyChanged
    {
        // singleton pattern
        public APIMASHGlobals() 
        {
            m_Instance = this; // singleton hook
        }

        private static APIMASHGlobals m_Instance;
        public static APIMASHGlobals Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new APIMASHGlobals();
                }
                return m_Instance;
            }
        }

        public string STR_USERID = "userid";
        
        private string m_UserId = string.Empty;
        public string UserID
        {
            get
            {
                if (m_UserId == string.Empty)
                {
                    m_UserId = devfish.utils.RoamingStorageHelper.read(STR_USERID);
                }
                
                // if its not set we can get a null
                if (m_UserId == null) m_UserId = string.Empty;

                return m_UserId;
            }
            set
            {
                m_UserId = value;
                devfish.utils.RoamingStorageHelper.write(STR_USERID, m_UserId);
                OnPropertyChanged("UserID");
                OnPropertyChanged("UserIDDefaultIfBlank");
                System.Diagnostics.Debug.WriteLine("OnStaticPropertyChanged fired");
            }
        }
        
        public string UserIDDefaultIfBlank
        {
            get
            {
                // force it through the instance so it refetched the user id if necessary
                string _userid = APIMASHGlobals.Instance.UserID;  
                return _userid.Length <= 0 ? "[not set]" : _userid;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

