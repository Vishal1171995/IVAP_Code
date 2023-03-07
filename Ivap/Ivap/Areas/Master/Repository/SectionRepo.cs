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
    public class SectionRepo
    {
        public Response AddUpdateSection(SectionModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@SECTID",model.SECTID),
                new SqlParameter("@ENTITYID", model.EID),
                new SqlParameter("@PAY_SECTION_CODE", model.PAY_SECTION_CODE),
                new SqlParameter("@ERP_SECTION_CODE", model.ERP_SECTION_CODE),
                new SqlParameter("@SECTION_NAME", model.SECTION_NAME),
                 new SqlParameter("@CREATED_BY", model.CreatedBy),
                  new SqlParameter("@ISACT", model.IsActive),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateSection", CommandType.StoredProcedure, parameters));
                Response.operation opration = Response.operation.ADD;
                if (model.SECTID > 0)
                    opration = Response.operation.Update;
                res = res.GetResponse(opration, "Section", result, "Section name");
                model.SetDisplayName();
                if (result > 0)
                {
                    res.Message = model.Screen_Name + " created successfully.";
                    res.IsSuccess = true;
                    return res;
                }
                if (result == 0)
                {
                    res.Message = model.Screen_Name + " updated successfully.";
                    res.IsSuccess = true;
                    return res;
                }
                if (result == -1)
                {
                    res.Message = model.SECTION_NAME_TEXT +" must be unique.";
                    res.IsSuccess = false;
                    return res;
                }

                if (result == -2)
                {
                    res.Message = model.PAY_SECTION_CODEE_TEXT + " must be unique.";
                    res.IsSuccess = false;
                    return res;
                }
                if (result == -3)
                {
                    res.Message = model.ERP_SECTION_CODE_TEXT + " must be unique.";
                    res.IsSuccess = false;
                    return res;
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetSection(SectionModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@SectionID", objModel.SECTID),
                    new SqlParameter("@SectionName", objModel.SECTION_NAME),
                    new SqlParameter("@ENTITYID", objModel.EID),
                    new SqlParameter("@ISAct", objModel.IsActive),
                    new SqlParameter("@UID", objModel.CreatedBy),

                };
                dt = DataLib.ExecuteDataTable("GetSection", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetSectionHis(SectionModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@SECTID", objModel.SECTID),
                };
                dt = DataLib.ExecuteDataTable("GetSectionHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string UploadSectionDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
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

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_SECTION", "ViewSection"))
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
                SectionModel Model = new SectionModel();
                Model.EID = EID;
                Model.SetDisplayName();
                string strerr = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        string TID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        Model.SECTID = Convert.ToInt32(TID);
                        Model.EID = EID;
                        Model.PAY_SECTION_CODE = Convert.ToString((dt.Rows[i][Model.PAY_SECTION_CODEE_TEXT]).ToString().Trim());
                        Model.ERP_SECTION_CODE = Convert.ToString((dt.Rows[i][Model.ERP_SECTION_CODE_TEXT]).ToString().Trim());
                        Model.SECTION_NAME = Convert.ToString((dt.Rows[i][Model.SECTION_NAME_TEXT]).ToString().Trim());
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
                            ret = AddUpdateSection(Model);

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