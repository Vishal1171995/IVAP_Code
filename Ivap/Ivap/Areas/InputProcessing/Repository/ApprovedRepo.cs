using Ivap.Areas.Configuration.Repository;
using Ivap.Areas.InputProcessing.Models;
using Ivap.Areas.Master.Models;
using Ivap.Areas.Master.Repository;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Ivap.Areas.InputProcessing.Repository
{
    public class ApprovedRepo
    {
        #region Approved
        public int SendToApproved(int FileID, int EntityID, string Status)
        {
            int result = 0;
            string TempTableName = "Ivap_MAST_TEMP_" + EntityID;
            try
            {
                    SqlParameter[] parameters = new SqlParameter[]
                            {
                         new SqlParameter("@TempTableName",TempTableName),
                        new SqlParameter("@FileID",FileID),
                        new SqlParameter("@Status", Status),
                            };
                    result = Convert.ToInt32(DataLib.ExecuteScaler("ApprovedTempTableData", CommandType.StoredProcedure, parameters));
                
                return result;
            }
            catch
            {
                throw;
            }
        }
        #endregion Approved
    }
}