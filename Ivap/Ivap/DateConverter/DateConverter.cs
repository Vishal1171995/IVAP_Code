using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Ivap.DateConverter
{
    
    public class DateConverterBase
    {
        public virtual DateTime ConvertDate(string StrDate, string Formate)
        {

            return Convert.ToDateTime(StrDate);
        }
    }


    public class DateConverterddmmyyforwordslash : DateConverterBase
    {

        public override DateTime ConvertDate(string StrDate, string Formate)
        {

            if (StrDate == "")
            {
                throw new Exception();
            }
            string[] tokens = StrDate.Split(' ');
            string[] Date = tokens[0].ToString().Split('/');
            try
            {
                return new DateTime(Convert.ToInt32("20"+Date[2]), Convert.ToInt32(Date[1]), Convert.ToInt32(Date[0]));
            }
            catch
            {
                throw;

            }
        }
    }

    public class DateConverterddmmyyyyforwordslash : DateConverterBase
    {

        public override DateTime ConvertDate(string StrDate, string Formate)
        {

            if (StrDate == "")
            {
                throw new Exception();
            }
            string[] tokens = StrDate.Split(' ');
            string[] Date = tokens[0].ToString().Split('/');
            try
            {
                return new DateTime(Convert.ToInt32(Date[2]), Convert.ToInt32(Date[1]), Convert.ToInt32(Date[0]));
            }
            catch
            {
                throw;

            }
        }
    }

    public class DateConverterddmmyyyydash : DateConverterBase
    {

        public override DateTime ConvertDate(string StrDate, string Formate)
        {

            if (StrDate == "")
            {
                throw new Exception();
            }
            string[] tokens = StrDate.Split(' ');
            string[] Date = tokens[0].ToString().Split('-');
            try
            {
                return new DateTime(Convert.ToInt32(Date[2]), Convert.ToInt32(Date[1]), Convert.ToInt32(Date[0]));
            }
            catch
            {
                throw;

            }
        }
    }


    public class DateConverteryyyymmdddash : DateConverterBase
    {

        public class DateConverterddmmyyyydash : DateConverterBase
        {

            public override DateTime ConvertDate(string StrDate, string Formate)
            {

                if (StrDate == "")
                {
                    throw new Exception();
                }
                string[] tokens = StrDate.Split(' ');
                string[] Date = tokens[0].ToString().Split('-');
                try
                {
                    return new DateTime(Convert.ToInt32(Date[2]), Convert.ToInt32(Date[1]), Convert.ToInt32(Date[0]));
                }
                catch
                {
                    throw;

                }
            }
        }

       
    }
    public class DateConverterDD_MON_YYYY : DateConverterBase
    {

        public override DateTime ConvertDate(string StrDate, string Formate)
        {
            if (StrDate == "")
            {
                return new DateTime();
            }
            string[] tokens = StrDate.Split(' ');
            string[] Date = tokens[0].ToString().Split('-');
            try
            {
                var Month = DateTime.ParseExact(Date[1].Trim(), "MMM", CultureInfo.CurrentCulture).Month;
                return new DateTime(Convert.ToInt32(Date[2]), Month, Convert.ToInt32(Date[0]));
            }
            catch (Exception Ex)
            {
                throw;

            }
        }
    }
}