using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
* LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
*/

/*
 *
 *  A P I   M A S H
 *
 *  Update the classes here to invoke the RESTful API call(s)
 */

namespace APIMASHLib
{
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
