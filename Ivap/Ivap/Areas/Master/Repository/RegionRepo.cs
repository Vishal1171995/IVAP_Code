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
    public class RegionRepo
    {
        public object SqlDataLib { get; private set; }
        public Response AddUpdateRegion(RegionModel model)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@RegionID",model.RegionID),
                    new SqlParameter("@ENTITY_ID",model.EID),
                    new SqlParameter("@PAY_REGION_CODE",model.PAY_REGION_CODE),
                    new SqlParameter("@ERP_REGION_CODE",model.ERP_REGION_CODE),
                    new SqlParameter("@REGION_NAME",model.REGION_NAME),
                    new SqlParameter("@ISACTIVE",model.IsActive),
                    new SqlParameter("@UID",model.CreatedBy),
                    
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateRegion", CommandType.StoredProcedure, parameters));
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
                    Res.Message = "Failed!!! " + model.PAY_REGION_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                else if (result == -2)
                {
                    Res.Message = "Failed!!! " + model.ERP_REGION_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                else if (result == -3)
                {
                    Res.Message = "Failed!!! " + model.REGION_NAME_TEXT + " must be unique.";
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
        public DataTable GetRegion(RegionModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@RegionID", objModel.RegionID),
                    new SqlParameter("@ENTITY_ID", objModel.EID),
                    new SqlParameter("@REGION_CODE", objModel.PAY_REGION_CODE),
                    new SqlParameter("@REGION_NAME", objModel.REGION_NAME),
                    new SqlParameter("@IsAct", objModel.IsActive),
                    new SqlParameter("@UID", objModel.CreatedBy),
                };
                dt = DataLib.ExecuteDataTable("GetRegion", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetRegionHistory(RegionModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@RegionID", objModel.RegionID),
                };
                dt = DataLib.ExecuteDataTable("GetRegionHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string UploadRegionDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
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

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_REGION", "ViewRegion"))
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
                RegionModel Model = new RegionModel();
                Model.EID = EID;
                Model.SetDisplayName();
                string strerr = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        string TID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        Model.RegionID = Convert.ToInt32(TID);
                        Model.EID = EID;
                        Model.PAY_REGION_CODE = Convert.ToString((dt.Rows[i][Model.PAY_REGION_CODE_TEXT]).ToString().Trim());
                        Model.ERP_REGION_CODE = Convert.ToString((dt.Rows[i][Model.ERP_REGION_CODE_TEXT]).ToString().Trim());
                        Model.REGION_NAME = Convert.ToString((dt.Rows[i][Model.REGION_NAME_TEXT]).ToString().Trim());

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
                            ret = AddUpdateRegion(Model);

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