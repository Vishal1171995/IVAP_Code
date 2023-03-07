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
    public class GradeRepo
    {
        public Response AddUpdateGrade(GradeModel Model)
        {
            try
            {
                Response Res = new Response();
                Res.IsSuccess = false;
                Res.Message = "Something Went Wrong.";

                SqlParameter[] P = new[] {
                        new SqlParameter("@TID", Model.TID),
                        new SqlParameter("@ENTITY_ID", Model.EID),
                        new SqlParameter("@PAY_GRADE_CODE", Model.PAY_GRADE_CODE),
                        new SqlParameter("@ERP_GRADE_CODE", Model.ERP_GRADE_CODE),
                        new SqlParameter("@GARDE_NAME", Model.GARDE_NAME),
                        new SqlParameter("@GRADE_SCALE_FROM", Model.GRADE_SCALE_FROM),
                        new SqlParameter("@GRADE_SCALE_TO", Model.GRADE_SCALE_TO),
                        new SqlParameter("@GRADE_MIDPOINT", Model.GRADE_MIDPOINT),
                        new SqlParameter("@Prob_Period", Model.Prob_Period),
                        new SqlParameter("@CREATED_BY", Model.CreatedBy),
                        new SqlParameter("@ISACTIVE", Model.IsActive)
                };
                int InsRes = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateGrade", CommandType.StoredProcedure, P));
                Model.SetDisplayName();
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
                    Res.Message = "Failed!!! " + Model.ERP_GRADE_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                if (InsRes == -2)
                {
                    Res.Message = "Failed!!! " + Model.PAY_GRADE_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                if (InsRes == -3)
                {
                    Res.Message = "Failed!!! " + Model.GARDE_NAME_TEXT + " must be unique.";
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
        public DataTable GetGrade(GradeModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@GradeID", objModel.TID),
                    new SqlParameter("@EntityID", objModel.EID),
                    new SqlParameter("@IsAct", objModel.IsActive),
                     new SqlParameter("@UID", objModel.CreatedBy),
                };
                dt = DataLib.ExecuteDataTable("GetGrade", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string UploadGradeDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
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

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_GRADE", "GradeList"))
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
                GradeModel Model = new GradeModel();
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
                        Model.PAY_GRADE_CODE = Convert.ToString((dt.Rows[i][Model.PAY_GRADE_CODE_TEXT]).ToString().Trim());
                        Model.ERP_GRADE_CODE = Convert.ToString((dt.Rows[i][Model.ERP_GRADE_CODE_TEXT]).ToString().Trim());
                        Model.GARDE_NAME = Convert.ToString((dt.Rows[i][Model.GARDE_NAME_TEXT]).ToString().Trim());
                        Model.GRADE_SCALE_FROM = Convert.ToInt32(dt.Rows[i][Model.GRADE_SCALE_FROM_TEXT].ToString().Trim() == "" ? 0 : dt.Rows[i][Model.GRADE_SCALE_FROM_TEXT]);
                        Model.GRADE_SCALE_TO = Convert.ToInt32(dt.Rows[i][Model.GRADE_SCALE_TO_TEXT].ToString().Trim() == "" ? 0 : dt.Rows[i][Model.GRADE_SCALE_TO_TEXT]);
                        Model.GRADE_MIDPOINT = Convert.ToInt32(dt.Rows[i][Model.GRADE_MIDPOINT_TEXT].ToString().Trim() == "" ? 0 : dt.Rows[i][Model.GRADE_MIDPOINT_TEXT]);
                        Model.Prob_Period = Convert.ToInt32(dt.Rows[i][Model.Prob_Period_TEXT].ToString().Trim() == "" ? 0 : dt.Rows[i][Model.GRADE_MIDPOINT_TEXT]);
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
                            ret = AddUpdateGrade(Model);

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
        public DataTable GetGradHistory(int GradID)
        {
            try
            {
                SqlParameter[] P = new[]
                {
                new SqlParameter("@Grad_ID",GradID)
            };
                return DataLib.ExecuteDataTable("GetGradHis", CommandType.StoredProcedure, P);
            }
            catch { throw; }
        }
    }
}