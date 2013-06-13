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
    public enum APIMASHStatus { FAILURE = 0, SUCCESS = 1 };

    public class APIMASHEvent : EventArgs
    {
        public APIMASHStatus Status { get; set; }

        public string APIName { get; set; }

        public string Message { get; set; }

        public object Object { get; set; }
    }

    public delegate void APIMASHEventHandler(object sender, APIMASHEvent e);

    public class APIMASHInvoke
    {
        public event APIMASHEventHandler OnResponse;

        async public void Invoke<T>(string apiCall)
        {
            var apimashEvent = new APIMASHEvent();

            try
            {
                T response = await APIMASHMap.LoadObject<T>(apiCall);
                apimashEvent.Object = response;
                apimashEvent.Status = APIMASHStatus.SUCCESS;
                apimashEvent.Message = string.Empty;
            }
            catch (Exception e)
            {
                apimashEvent.Message = e.Message;
                apimashEvent.Object = null;
                apimashEvent.Status = APIMASHStatus.FAILURE;
            }

            OnResponse(this, apimashEvent);
        }
    }
}
