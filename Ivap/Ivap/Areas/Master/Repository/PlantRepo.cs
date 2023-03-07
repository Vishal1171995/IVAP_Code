using Ivap.Areas.Master.Models;
using Ivap.Models;
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
    public class PlantRepo: BaseModel
    {
        public object SqlDataLib { get; private set; }
        public Response AddUpdatePlant(PlantModel model)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@PlantID",model.PlantID),
                    new SqlParameter("@ENTITY_ID",model.EID),
                    new SqlParameter("@PAY_PLANT_CODE",model.PAY_PLANT_CODE),
                    new SqlParameter("@ERP_PLANT_CODE",model.ERP_PLANT_CODE),
                    new SqlParameter("@PLANT_NAME",model.PLANT_NAME),
                    new SqlParameter("@IsAct",model.IsActive),
                    new SqlParameter("@UID",model.CreatedBy),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdatePlant", CommandType.StoredProcedure, parameters));
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
                    Res.Message = "Failed!!! " + model.PAY_PLANT_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                else if (result == -2)
                {
                    Res.Message = "Failed!!! " + model.ERP_PLANT_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                else if (result == -3)
                {
                    Res.Message = "Failed!!! " + model.PLANT_NAME_TEXT + " must be unique.";
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
        public DataTable GetPlant(PlantModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@PlantID", objModel.PlantID),
                    new SqlParameter("@ENTITY_ID", objModel.EID),
                    new SqlParameter("@PLANT_CODE", objModel.PAY_PLANT_CODE),
                    new SqlParameter("@PLANT_NAME", objModel.PLANT_NAME),
                    new SqlParameter("@IsAct", objModel.IsActive),
                     new SqlParameter("@UID", objModel.CreatedBy),
                };
                dt = DataLib.ExecuteDataTable("GetPlant", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetPlantHistory(PlantModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@PlantID", objModel.PlantID),
                };
                dt = DataLib.ExecuteDataTable("GetPlantHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string UploadPlantDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
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

                if (!ExcellUtils.CheckColumnFormat(FilePath,EID,"IVAP_MST_PLANT", "ViewPlant"))
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
                PlantModel Model = new PlantModel();
                Model.EID = EID;
                Model.SetDisplayName();
                string strerr = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        string TID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        Model.PlantID = Convert.ToInt32(TID);
                        Model.EID = EID;
                        Model.PAY_PLANT_CODE = Convert.ToString((dt.Rows[i][Model.PAY_PLANT_CODE_TEXT]).ToString().Trim());
                        Model.ERP_PLANT_CODE = Convert.ToString((dt.Rows[i][Model.ERP_PLANT_CODE_TEXT]).ToString().Trim());
                        Model.PLANT_NAME = Convert.ToString((dt.Rows[i][Model.PLANT_NAME_TEXT]).ToString().Trim());
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
                            ret = AddUpdatePlant(Model);

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