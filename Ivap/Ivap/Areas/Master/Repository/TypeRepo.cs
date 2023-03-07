using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ivap.Areas.Master.Models;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Ivap.Areas.Master.Repository
{
    public class TypeRepo
    {

        public Response AddUpdateType(TypeModel Model)
        {
            try
            {
                Response Res = new Response();
                Res.IsSuccess = false;
                Res.Message = "Something Went Wrong.";

                SqlParameter[] P = new[] {
                    new SqlParameter("@TID", Model.TID),
                    new SqlParameter("@Type_Name", Model.TYPE_NAME),
                    new SqlParameter("@Pay_Type_Code", Model.PAY_TYPE_CODE),
                    new SqlParameter("@Erp_Type_Code", Model.ERP_TYPE_CODE),
                    new SqlParameter("@EID", Model.EID),
                    new SqlParameter("@IsAct", Model.IsActive),
                    new SqlParameter("@Created_By", Model.CreatedBy)
                };
                int InsRes = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateTypes", CommandType.StoredProcedure, P));
                if (InsRes > 0)
                {
                    Res.Message = Model.Screen_Name + " created successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                if (InsRes == 0)
                {
                    Res.Message = Model.Screen_Name + " updated successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                if (InsRes == -1)
                {
                    Res.Message = "Failed!!! " + Model.PAY_TYPE_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                if (InsRes == -2)
                {
                    Res.Message = "Failed!!! " + Model.ERP_TYPE_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                if (InsRes == -3)
                {
                    Res.Message = "Failed!!! " + Model.TYPE_NAME_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                return Res;
            }
            catch
            {
                throw;
            }
        }

        public DataSet GetType(TypeModel Model)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] P = new[] {
                    
                    new SqlParameter("@IsAct", Model.IsActive),
                    new SqlParameter("@TID", Model.TID),
                     new SqlParameter("@EID", Model.EID),
                    new SqlParameter("@UID", Model.CreatedBy),

                };
                 ds= DataLib.ExecuteDataSet("GetType", CommandType.StoredProcedure, P);
                return ds;
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetTypeHis(TypeModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TYID", objModel.TID),
                     new SqlParameter("@EntityID", objModel.EID),
                };
                dt = DataLib.ExecuteDataTable("GetTypeHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string UploadTypeDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
        {
            try
            {
                Response ret = new Response();
                DataTable dt = null;
                try
                {
                    dt = ExcellUtils.ExlToDataTable(FilePath, "xlsx");
                }
                catch (Exception Ex)
                {
                    return Ex.Message;
                }

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_TYPE", "TypeList"))
                {
                    return "Invalid File Format";
                }
                DataColumnCollection columns = dt.Columns;
                if (!columns.Contains("Response"))
                {
                    dt.Columns.Add("Response");
                }

                if (!columns.Contains("Message"))
                {
                    dt.Columns.Add("Message");
                }
                TypeModel Model = new TypeModel();
                Model.EID = EID;
                Model.SetDisplayName();
                string strerr = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        string TID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        Model.TID = Convert.ToInt32(TID);
                        Model.EID = EID;
                        Model.ERP_TYPE_CODE = Convert.ToString((dt.Rows[i][Model.ERP_TYPE_CODE_TEXT]).ToString().Trim());
                        Model.PAY_TYPE_CODE = Convert.ToString((dt.Rows[i][Model.PAY_TYPE_CODE_TEXT]).ToString().Trim());
                        Model.TYPE_NAME = Convert.ToString((dt.Rows[i][Model.TYPE_NAME_TEXT]).ToString().Trim());

                        Model.IsActive = Convert.ToBoolean(dt.Rows[i]["ISACTIVE"].ToString() == "1" ? true : false);
                        Model.CreatedBy = CreatedBy;

                        var results = new List<ValidationResult>();
                        var vc = new ValidationContext(Model, null, null);
                        var isValid = Validator.TryValidateObject(Model, vc, results, true);
                        var errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                        //strerr = string.Join(" ", errors);
                        strerr = MasterMetaRepo.GenerateError(Model, results);
                        if (isValid)
                        {
                            ret = AddUpdateType(Model);

                            if (ret.IsSuccess == true)
                            {
                                SuccessCount += 1;
                                dt.Rows[i]["Message"] = ret.Message;
                                dt.Rows[i]["Response"] = "Success";
                            }
                            else
                            {
                                FailCount += 1;
                                dt.Rows[i]["Message"] = ret.Message;
                                dt.Rows[i]["Response"] = "Failed";
                            }
                        }
                        else
                        {
                            FailCount += 1;
                            dt.Rows[i]["Response"] = "Failed";
                            dt.Rows[i]["Message"] = strerr;
                            //Message
                        }
                    }
                    catch (Exception ex)
                    {
                        FailCount += 1;
                        dt.Rows[i]["Response"] = "Failed";
                        dt.Rows[i]["Message"] = "Invalid Data Format";
                        continue;
                    }
                }
                //Converting dt into csv FIle
                FilePath = ExcellUtils.DataTableToExcel(dt);
                return FilePath;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}