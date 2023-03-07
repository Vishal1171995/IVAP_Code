using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Ivap.Utils;

namespace Ivap.Areas.Master.Repository
{
    public class CountryRepo
    {
        public DataTable GetCountry()
        {
            DataTable dt = new DataTable();
            try
            {
                return DataLib.ExecuteDataTable("GetCountry", CommandType.StoredProcedure, null);
            }
            catch
            {
                throw;
            }
        }

    }
}