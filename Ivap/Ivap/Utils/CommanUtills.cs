using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Globalization;

namespace Ivap.Utils
{
    public static class CommanUtills
    {

        public static string CurrencyFormating(string Currency)
        {
            if (Currency.Trim() == "")
                return "";
            decimal parsed = decimal.Parse(Currency, CultureInfo.InvariantCulture);
            CultureInfo hindi = new CultureInfo("hi-IN");
            string text = string.Format(hindi, "{0:c}", parsed);
            return text;
        }
        public static string GetIPAddress()
        {
            try
            {
                string IP = HttpContext.Current.Request.Params["HTTP_CLIENT_IP"] ?? HttpContext.Current.Request.UserHostAddress;
                return IP;
            }
            catch
            {
                return "";
            }
        }

        public static string MaskString(string StringToBeMasked,int FirstMaskIndex,int lastMaskIndex )
        {
            try
            {
                var firstDigits = StringToBeMasked.Substring(0, FirstMaskIndex);
                var lastDigits = StringToBeMasked.Substring(StringToBeMasked.Length - lastMaskIndex, lastMaskIndex);
                var requiredMask = new String('X', StringToBeMasked.Length - firstDigits.Length - lastDigits.Length);
                string maskedString = string.Concat(firstDigits, requiredMask, lastDigits);
                return maskedString;
            }
            catch
            {
                return "";
            }
        }

        public static string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }
    }
    public class SortDescription
    {
        public string field { get; set; }
        public string dir { get; set; }
    }
    public class FilterContainer
    {
        public List<FilterDescription> filters { get; set; }
        public string logic { get; set; }
    }
    public class FilterDescription
    {
        public string @operator { get; set; }
        public string field { get; set; }
        public string value { get; set; }
    }


   
}