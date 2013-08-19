using System;

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
    public class APIMASHEvent : EventArgs
    {
        public APIMASHStatus Status { get; set; }

        public string APIName { get; set; }

        public string Message { get; set; }

        public object Object { get; set; }
    }
}