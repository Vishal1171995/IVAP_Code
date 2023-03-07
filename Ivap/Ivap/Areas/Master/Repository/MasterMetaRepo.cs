using Ivap.Areas.Master.Models;
using Ivap.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace Ivap.Areas.Master.Repository
{
    public class MasterMetaRepo
    {
        public int EID { set; get; }
        public string Table_Name { set; get; }

        public string Menu_Name { set; get; }

        public string Screen_Name { set; get; }

        public DataSet DsMetaData { set; get; }

        public DataSet GetMasterMetaData()
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@EID",this.EID),
                    new SqlParameter("@SCREEN_NAME",this.Table_Name ),
                    new SqlParameter("@Menu_Name",this.Menu_Name)
                };
                return DataLib.ExecuteDataSet("GetMasterMetaData", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }

        public MasterMetaRepo(int EID,string Table_Name,string Menu_Name)
        {
            this.EID = EID;
            this.Table_Name = Table_Name.Trim();
            this.Menu_Name = Menu_Name.Trim();
            this.DsMetaData = GetMasterMetaData();
            this.Screen_Name = this.DsMetaData.Tables[1].Rows[0]["Name"].ToString().Trim();
        }

        public string GetDisPlayName(string Field_Name)
        {
            try
            {
                DataView dv = new DataView(this.DsMetaData.Tables[0]);
                dv.RowFilter = "Field_Name='" + Field_Name.Trim() + "'";
                DataTable dtMeta = dv.ToTable();
                string Display_Name = dtMeta.Rows[0]["Display_Name"].ToString().Trim();
                return Display_Name;
            }
            catch
            {
                return Field_Name;
            }
        }
        public static string GenerateError(BaseModel ModelToValidate, List<ValidationResult> ValidationResult)
        {
            try
            {
                StringBuilder SBError = new StringBuilder();
                foreach(var item in ValidationResult)
                {
                    try
                    {
                        var MemberName = item.MemberNames.ToArray()[0] + "_TEXT";
                        //var TextProperty=
                        var ErrorMsg = ModelToValidate.GetType().GetProperty(MemberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(ModelToValidate, null)+" Required,";
                        SBError.Append(ErrorMsg);
                    }
                    catch
                    {
                        SBError.Append(item.MemberNames.ToArray()[0]).Append(" Required,");
                    }
                }
                return SBError.ToString().TrimEnd(',');
            }
            catch
            {
                throw;
            }
        }

    }
}