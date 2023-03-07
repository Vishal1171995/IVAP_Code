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
    public class ProcessRepo
    {
        public Response AddUpdateProcess(ProcessModel Model)
        {
            try
            {
                Response Res = new Response();
                Res.IsSuccess = false;
                Res.Message = "Something Went Wrong.";

                SqlParameter[] P = new[] {
                    new SqlParameter("@TID", Model.TID),
                    new SqlParameter("@ENTITY_ID", Model.EID),
                    new SqlParameter("@Pay_Proc_Code", Model.PAY_PROC_CODE),
                    new SqlParameter("@Erp_Proc_Code", Model.ERP_PROC_CODE),
                    new SqlParameter("@Proc_Name", Model.PROC_NAME),
                    new SqlParameter("@IsAct", Model.IsActive),
                    new SqlParameter("@Created_By", Model.CreatedBy)
                };
                int InsRes = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateProcess", CommandType.StoredProcedure, P));
                Model.SetDisplayName();
                if (InsRes>0)
                {
                    Res.Message = Model.Screen_Name + "Process created successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                if (InsRes ==-1)
                {
                    Res.Message = "Failed!!! " + Model.ERP_PROC_CODE_Text + " Must Be Unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                if (InsRes == -2)
                {
                    Res.Message = "Failed!!!" + Model.PAY_PROC_CODE_Text + " Must Be Unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                if (InsRes == -3)
                {
                    Res.Message = "Failed!!! " + Model.PROC_NAME_Text + " Must Be Unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                if (InsRes == 0)
                {
                    Res.Message = Model.Screen_Name + " updated successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                return Res;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public DataSet GetProcess(ProcessModel Model)
        {
            try
            {
                SqlParameter[] P = new[] {
                    new SqlParameter("@ENTITY_ID", Model.EID),
                    new SqlParameter("@IsAct", Model.IsActive),
                    new SqlParameter("@TID", Model.TID),
                    new SqlParameter("@UID", Model.CreatedBy)

                };
                return DataLib.ExecuteDataSet("GetProcess", CommandType.StoredProcedure, P);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetProcessHis(ProcessModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@PID", objModel.TID),
                     new SqlParameter("@EntityID", objModel.EID),
                };
                dt = DataLib.ExecuteDataTable("GetProcessHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string UploadProcessDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
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

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_PROCESS", "ProcessList"))
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
                ProcessModel Model = new ProcessModel();
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
                        Model.PAY_PROC_CODE = Convert.ToString((dt.Rows[i][Model.PAY_PROC_CODE_Text]).ToString().Trim());
                        Model.ERP_PROC_CODE = Convert.ToString((dt.Rows[i][Model.ERP_PROC_CODE_Text]).ToString().Trim());
                        Model.PROC_NAME = Convert.ToString((dt.Rows[i][Model.PROC_NAME_Text]).ToString().Trim());
                       
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
                            ret = AddUpdateProcess(Model);

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