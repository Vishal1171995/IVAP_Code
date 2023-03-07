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
    public class SubFunctionRepo
    {
       
        public Response AddUpdateSubFunction(SubFunctionModel model)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@SID",model.SID),
                new SqlParameter("@ENTITY_ID", model.EID),
                new SqlParameter("@PAY_SUB_FUNC_CODE", model.PAY_SUB_FUNC_CODE),
                new SqlParameter("@ERP_SUB_FUNC_CODE", model.ERP_SUB_FUNC_CODE),
                new SqlParameter("@SUB_FUNC_NAME", model.SUB_FUNC_NAME),
                 new SqlParameter("@UID", model.CreatedBy),
                  new SqlParameter("@ISACTIVE", model.IsActive),
                   new SqlParameter("@PARENT_FUNC_ID", model.PARENT_FUNC_ID),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateSubFunction", CommandType.StoredProcedure, parameters));
                model.SetDisplayName();
                if (result > 0)
                {
                    Res.Message = model.Screen_Name + " created successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                if (result == 0)
                {
                    Res.Message = model.Screen_Name + " updated successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                else if (result == -1)
                {
                    Res.Message = "Failed!!! " + model.PAY_SUB_FUNC_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                else if (result == -2)
                {
                    Res.Message = "Failed!!! " + model.ERP_SUB_FUNC_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                else if (result == -3)
                {
                    Res.Message = "Failed!!! " + model.SUB_FUNC_NAME_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                return Res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetSubFunction(SubFunctionModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID", objModel.SID),
                    new SqlParameter("@ENTITYID", objModel.EID),
                    new SqlParameter("@IsAct", objModel.IsActive),
                     new SqlParameter("@UID", objModel.CreatedBy),

                };
                dt = DataLib.ExecuteDataTable("GetSubFunction", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetSubFunctionHis(SubFunctionModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@SID", objModel.SID),
                };
                dt = DataLib.ExecuteDataTable("GetSubFunctionHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string UploadSubFunctionDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
        {
            try
            {
                FunctionModel objFuntionModel = new FunctionModel();
                FunctionRepo objFunctionRepo = new FunctionRepo();

                Response ret = new Response();
                DataTable dt = null;
                DataSet ds = null;
                try
                {
                    dt = ExcellUtils.ExlToDataTable(FilePath, "xlsx");
                }
                catch (Exception Ex)
                {
                    return Ex.Message;
                }

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_SUB_FUNCTION", "ViewSubFunction"))
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
                SubFunctionModel Model = new SubFunctionModel();
                SubFunctionModelVW ModelVW = new SubFunctionModelVW();
                Model.EID = EID;
                Model.SetDisplayName();
                string strerr = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        string TID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        ModelVW.SID = Convert.ToInt32(TID);
                        ModelVW.PAY_SUB_FUNC_CODE = dt.Rows[i][Model.PAY_SUB_FUNC_CODE_TEXT].ToString();
                        ModelVW.ERP_SUB_FUNC_CODE = dt.Rows[i][Model.ERP_SUB_FUNC_CODE_TEXT].ToString();
                        ModelVW.PARENT_FUNC_ID = dt.Rows[i][Model.PARENT_FUNC_ID_TEXT].ToString();
                        ModelVW.SUB_FUNC_NAME = dt.Rows[i][Model.SUB_FUNC_NAME_TEXT].ToString();
                        ModelVW.IsActive = Convert.ToBoolean(dt.Rows[i]["ISACTIVE"].ToString() == "1" ? true : false);
                        ModelVW.CreatedBy = CreatedBy;

                        var results = new List<ValidationResult>();
                        var vc = new ValidationContext(ModelVW, null, null);
                        var isValid = Validator.TryValidateObject(ModelVW, vc, results, true);
                        var errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                        //strerr = string.Join(" ", errors);
                        strerr = MasterMetaRepo.GenerateError(ModelVW, results);
                        if (isValid)
                        {

                            Model.SID = ModelVW.SID;
                            Model.ERP_SUB_FUNC_CODE = ModelVW.ERP_SUB_FUNC_CODE;
                            Model.PAY_SUB_FUNC_CODE = ModelVW.PAY_SUB_FUNC_CODE;
                            Model.SUB_FUNC_NAME = ModelVW.SUB_FUNC_NAME;
                            Model.IsActive = ModelVW.IsActive;
                            Model.CreatedBy = CreatedBy;
                            ds = objFunctionRepo.GetFunction(objFuntionModel);
                            var Function_ID = ds.Tables[0];

                            DataView dvFunction = new DataView(Function_ID);
                            dvFunction.RowFilter = "FUNC_NAME='" + ModelVW.PARENT_FUNC_ID + "'";
                            DataTable dtFunction = dvFunction.ToTable();

                            if (dtFunction.Rows.Count > 0)
                            {
                                Model.PARENT_FUNC_ID = Convert.ToInt32(dtFunction.Rows[0]["TID"]);
                            }
                            else
                            {
                                Model.PARENT_FUNC_ID = -1;
                            }
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



                            var results_Model = new List<ValidationResult>();
                            var vc_Model = new ValidationContext(Model, null, null);
                            var isValid_Model = Validator.TryValidateObject(Model, vc_Model, results, true);
                            var errors_Model = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                            //strerr = string.Join(" ", errors);
                            strerr = MasterMetaRepo.GenerateError(Model, results);
                            if (isValid_Model)
                            {

                                ret = AddUpdateSubFunction(Model);


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