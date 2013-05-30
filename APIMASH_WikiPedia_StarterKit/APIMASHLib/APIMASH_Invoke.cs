using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

///////////////////////////////////////////////////////////////////////////////////////////////
//
//  A P I   M A S H
//
//  Update the classes here to invoke the RESTful API call(s)
//
///////////////////////////////////////////////////////////////////////////////////////////////

namespace APIMASHLib
{
    public enum APIMASH_STATUS { FAILURE = 0, SUCCESS = 1 };

    public class APIMASHEvent : EventArgs
    {
        private APIMASH_STATUS _status;
        private string _apiname;
        private string _message;
        private object _object;

        public APIMASH_STATUS Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public string APIName
        {
            get { return _apiname; }
            set { _apiname = value; }
        }
        
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public object Object
        {
            get { return _object; }
            set { _object = value; }
        }
    }

    public delegate void APIMASHEventHandler(object sender, APIMASHEvent e);

    public class APIMASH_Invoke
    {
        public event APIMASHEventHandler OnResponse;

        async public void Invoke<T>(string apiCall)
        {
            APIMASHEvent apimashEvent = new APIMASHEvent();

            try
            {
                T response = await APIMASH_Map.LoadObject<T>(apiCall);
                apimashEvent.Object = response;
                apimashEvent.Status = APIMASH_STATUS.SUCCESS;
                apimashEvent.Message = string.Empty;
            }
            catch (Exception e)
            {
                apimashEvent.Message = e.Message;
                apimashEvent.Object = null;
                apimashEvent.Status = APIMASH_STATUS.FAILURE;
            }

            OnResponse(this, apimashEvent);
        }
    }
}
