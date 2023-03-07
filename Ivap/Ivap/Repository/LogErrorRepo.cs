using Ivap.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Repository
{
    public class LogErrorRepo
    {
        public string LogError(string ControllerName, string ActionName, string Error)
        {
            try
            {
                int UID = 0;
                if (HttpContext.Current.Session["uBo"] != null)
                    UID = ((AppUser)HttpContext.Current.Session["uBo"]).UID;
                SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@p_UID",UID),
                new SqlParameter("@p_ControllerName",ControllerName),
                new SqlParameter("@p_ActionName",ActionName),
                new SqlParameter("@p_Error",Error),
            };
                string res = DataLib.ExecuteScaler("LogError", CommandType.StoredProcedure, parameters);

                return res;
            }
            catch
            {
                return "";
            }
        }
    }
}