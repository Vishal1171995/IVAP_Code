using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivap.Areas.InputProcessing.Repository
{
    public class PayrollProcessingRepo
    {

        public DataTable GetPayRoll(string TableName,int UID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TABLE_NAME", TableName),
                    new SqlParameter("@UID", UID),

                };
                dt = DataLib.ExecuteDataTable("PAYROLLPROCESS_1", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable FileDownload(int EID, int File_ID)
        {
            try
            {
                UploadInputRepo objUploadRepo = new UploadInputRepo();
                DataTable dt = objUploadRepo.GetSchemaOfInputFile(EID, File_ID);
                string TableName = "IVAP_MAST_TEMP_" + EID;
                StringBuilder SBDynColumn = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string DisplayName = dt.Rows[i]["COMPONENT_DISPLAY_NAME"].ToString().Trim();
                    string ColumnText = dt.Rows[i]["COMPONENT_NAME"].ToString().Trim();
                    string ColumnName = ColumnText + " as " + "'" + DisplayName + "'";
                    SBDynColumn.Append(ColumnName);

                    SBDynColumn.Append(",");
                }
                string StrHRDCreateStatement = " select  " + SBDynColumn.ToString().TrimEnd(',') + " from " + TableName + " where FILE_ID= " + File_ID;

                DataTable DbRes = DataLib.ExecuteDataTable(StrHRDCreateStatement, CommandType.Text, null);
                return DbRes;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}