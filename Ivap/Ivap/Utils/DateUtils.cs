using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ivap.Utils
{
    public class DateUtils
    {

        public static System.DateTime GetFDate(string strDate)
        {
            DateTime ret;
            try
            {   //Here we are assuming seperator may be /,\,-,.
                char [] p = new char[] {'/','\'','.','-'};
                string[] arr = strDate.Split(p);
                string year="";
                year = arr[2];
                if(arr[2].Length==2)
                    year="20"+arr[2];
                ret = new DateTime(Convert.ToInt32(year), Convert.ToInt16(arr[1]), Convert.ToInt16(arr[0]));    
            }
            catch (Exception ex)
            {
                throw;
            }
            return ret;
        }

    }
}