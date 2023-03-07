using Ivap.Areas.Master.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Master.Repository
{
    public class ServiceAvailedRepo
    {
        public DataTable GetServiceAvailed(ServicesAvailedModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@SID", objModel.SID),
                    new SqlParameter("@ISAct", objModel.IsActive),

                };
                dt = DataLib.ExecuteDataTable("GetServiceAvailed", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}