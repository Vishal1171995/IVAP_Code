using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace Ivap.Utils
{
    public static class FileExtUtils
    { 
        public static bool checkFileExt(string ext)
        {
            DataTable dt = new DataTable();
            //List<string> lstExt = new List<string> { ".jpg", ".jpeg", ".pdf", ".csv", ".bmp", ".icon", ".png", ".dbx", ".pps", ".pub", ".doc", ".docx", ".dot", "", ".text", ".txt", ".xls", ".xlsx", ".xlsm", ".zip", ".rar" };
           
            try
            {
                SqlParameter[] param = null;

                dt = DataLib.ExecuteDataTable("GetFileExt", CommandType.StoredProcedure, param);
                List<string> lstExt = (from row in dt.AsEnumerable() select (row["FileExtension"]).ToString()).ToList();

                if (lstExt.Exists(p => p.Equals(ext)))
                    return true;
                else
                    return false;
            }
            catch
            { throw; }
        }
    }
}