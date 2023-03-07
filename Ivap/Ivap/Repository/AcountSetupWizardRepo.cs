using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Ivap.Utils;

namespace Ivap.Repository
{
    public class AcountSetupWizardRepo
    {
        public DataSet GetQuickSetup(int EID)
        {
            DataSet Ds = new DataSet();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@EID", EID)
                };
                Ds = DataLib.ExecuteDataSet("GetQuickSetup", CommandType.StoredProcedure, parameters); //GetCalendarDtl
                return Ds;
            }
            catch (Exception ex)
            {
                throw;

            }
        }

        public DataTable GetMasterCount(int EID)
        {
            DataTable Dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@EID", EID)
                };
                Dt = DataLib.ExecuteDataTable("GetMasterCount", CommandType.StoredProcedure, parameters); //GetCalendarDtl
                return Dt;
            }
            catch (Exception ex)
            {
                throw;

            }
        }
    }
}