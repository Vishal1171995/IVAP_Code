using Ivap.Areas.Configuration.Repository;
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
    public class MyRequestRepo
    {
        public DataTable MyRequestFile(string TableName,int UID,string Status)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TABLE_NAME", TableName),
                    new SqlParameter("@UID", UID),
                    new SqlParameter("@Status", Status),

                };
                dt = DataLib.ExecuteDataTable("MY_REQUEST_FILE_0", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public DataTable FileDownload(int EID,int File_ID,string Status)
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
                    string ColumnName= ColumnText+" as "+"'"+DisplayName+"'";
                    SBDynColumn.Append(ColumnName);
                     
                    SBDynColumn.Append(",");
                }
                string StrHRDCreateStatement = " select  " + SBDynColumn.ToString().TrimEnd(',') + " from "+TableName+ " where FILE_ID= "+ File_ID+" AND TEMP_STATUS='"+Status+"'";
                 
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