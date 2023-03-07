using Ivap.Areas.Master.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Master.Repository
{
    public class FunctionRepo
    {
        public Response AddUpdateFunction(FunctionModel Model)
        {
            try
            {
                Response Res = new Response();
                Res.IsSuccess = false;
                Res.Message = "Something Went Wrong.";

                SqlParameter[] P = new[] {
                        new SqlParameter("@TID", Model.TID),
                        new SqlParameter("@ENTITYID", Model.EID),
                        new SqlParameter("@PAY_FUNC_CODE", Model.PAY_FUNC_CODE),
                        new SqlParameter("@ERP_FUNC_CODE", Model.ERP_FUNC_CODE),
                        new SqlParameter("@FUNC_NAME", Model.FUNC_NAME),
                        new SqlParameter("@CREATED_BY", Model.CreatedBy),
                        new SqlParameter("@ISACTIVE", Model.IsActive),
                };
                int InsRes = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateFunction", CommandType.StoredProcedure, P));
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
                    Res.Message = "Failed!!! " + Model.ERP_FUNC_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                if (InsRes == -2)
                {
                    Res.Message = "Failed!!! " + Model.PAY_FUNC_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                if (InsRes == -3)
                {
                    Res.Message = "Failed!!! " + Model.FUNC_NAME_TEXT + " must be unique.";
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

        public DataSet GetFunction(FunctionModel Model)
        {
            try
            {
                SqlParameter[] P = new[] {
                    new SqlParameter("@ENTITYID", Model.EID),
                    //new SqlParameter("@EID", Model.EID),
                    new SqlParameter("@TID", Model.TID),
                    new SqlParameter("@IsAct", Model.IsActive),
                     new SqlParameter("@UID", Model.CreatedBy),
                };
                return DataLib.ExecuteDataSet("GetFunction", CommandType.StoredProcedure, P);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public DataTable GetFunctionHistory(int FunctionID)
        {
            try
            {
                SqlParameter[] P = new[]
                {
                new SqlParameter("@FunctionID",FunctionID)
            };
             return   DataLib.ExecuteDataTable("GetFunctionHis", CommandType.StoredProcedure, P);
            }
            catch { throw; }
        }
        public string UploadFunctionDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
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

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_FUNCTION", "ViewFunction"))
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
                FunctionModel Model = new FunctionModel();
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
                        Model.PAY_FUNC_CODE = Convert.ToString((dt.Rows[i][Model.PAY_FUNC_CODE_TEXT]).ToString().Trim());
                        Model.ERP_FUNC_CODE = Convert.ToString((dt.Rows[i][Model.ERP_FUNC_CODE_TEXT]).ToString().Trim());
                        Model.FUNC_NAME = Convert.ToString((dt.Rows[i][Model.FUNC_NAME_TEXT]).ToString().Trim());

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
                            ret = AddUpdateFunction(Model);

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