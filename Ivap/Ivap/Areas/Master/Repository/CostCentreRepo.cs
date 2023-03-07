using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Ivap.Areas.Master.Models;
using Ivap.Utils;

namespace Ivap.Areas.Master.Repository
{
    public class CostCentreRepo
    {
        public Response AddUpdateCostCentre(CostCentreModel model)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@CostCenterID",model.CostCenterID),
                    new SqlParameter("@ENTITYID",model.EID),
                    new SqlParameter("@PAY_COST_CODE",model.PAY_COST_CODE),
                    new SqlParameter("@ERP_COST_CODE",model.ERP_COST_CODE),
                    new SqlParameter("@COST_NAME",model.COST_NAME),
                    new SqlParameter("@CREATED_BY",model.CreatedBy),
                    new SqlParameter("@ISACT",model.IsActive)
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateCostCentre", CommandType.StoredProcedure, parameters));
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
                    Res.Message = "Failed!!! " + model.COST_NAME_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                else if (result == -2)
                {
                    Res.Message = "Failed!!! " + model.PAY_COST_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                else if (result == -3)
                {
                    Res.Message = "Failed!!! " + model.ERP_COST_CODE_TEXT + " must be unique.";
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
        public DataTable GetCostCentre(CostCentreModel model)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@CostCenterID",model.CostCenterID),
                    new SqlParameter("@ENTITYID",model.ENTITY_ID),
                    new SqlParameter("@ISACT",model.IsActive),
                     new SqlParameter("@UID",model.CreatedBy)
                };

                return DataLib.ExecuteDataTable("GetCostCentre", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetCostCenHistory(CostCentreModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@COSTCENID",model.CostCenterID ),
                };
                return DataLib.ExecuteDataTable("GetCostCenterHis", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
        public string DownLoadCostCentre(CostCentreModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = DataLib.ExecuteDataTable("GetCostCentre", CommandType.StoredProcedure, null);
                return CSVUtills.DataTableToCSV(dt, ",");
            }
            catch { throw; }
        }
        public string UploadCostCenterDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
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

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_COSTCENTRE", "ViewCostCentre"))
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
                CostCentreModel Model = new CostCentreModel();
                Model.EID = EID;
                Model.SetDisplayName();
                string strerr = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        string TID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        Model.CostCenterID = Convert.ToInt32(TID);
                        Model.ENTITY_ID = EID;
                        Model.PAY_COST_CODE = Convert.ToString((dt.Rows[i][Model.PAY_COST_CODE_TEXT]).ToString().Trim());
                        Model.ERP_COST_CODE = Convert.ToString((dt.Rows[i][Model.ERP_COST_CODE_TEXT]).ToString().Trim());
                        Model.COST_NAME = Convert.ToString((dt.Rows[i][Model.COST_NAME_TEXT]).ToString().Trim());

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
                            ret = AddUpdateCostCentre(Model);

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