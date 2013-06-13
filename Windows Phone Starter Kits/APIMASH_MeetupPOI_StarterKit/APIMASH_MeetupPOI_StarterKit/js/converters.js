(function () {

    /*
     * LICENSE: https://raw.github.com/apimash/StarterKits/master/LicenseTerms-SampleApps%20.txt
     */

    WinJS.Namespace.define('Converters', {
        friendlyDate: WinJS.Binding.converter(parseDate),
        parseDate: parseDate,
    });

    function parseDate(dateval) {
        if (dateval) {
            var d = new Date(parseInt(dateval.toString())).toLocaleDateString();
            return d;
        }
    }

})();