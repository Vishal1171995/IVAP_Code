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
    public class LocationRepo
    {
        public object SqlDataLib { get; private set; }

        public Response AddUpdateLocation(Location model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@LOCID",model.Location_Id),
                new SqlParameter("@ENTITY_ID", model.EID),
                new SqlParameter("@ERP_LOC_CODE", model.Erp_Loc_Code),
                 new SqlParameter("@PAY_LOC_CODE", model.Pay_Loc_Code),
                new SqlParameter("@LOC_NAME", model.Location_Name),
                new SqlParameter("@STATE_ID", model.State_Id),
                new SqlParameter("@ISMETRO", model.Is_Metro),
                 new SqlParameter("@UID", model.CreatedBy),
                  new SqlParameter("@ISACTIVE", model.IsActive),
                   new SqlParameter("@PARENT_LOC_ID", model.PARENT_LOC_ID),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateLocation", CommandType.StoredProcedure, parameters));
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
                    res.IsSuccess = false;
                    return res;
                }


                if (result == -1)
                {
                    res.Message = model.Location_Name_TEXT +" must be unique.";
                    res.IsSuccess = true;
                    return res;
                }
                if (result == -2)
                {
                    res.Message = model.Erp_Loc_Code_TEXT +" must be unique.";
                    res.IsSuccess = true;
                    return res;
                }
                if (result == -3)
                {
                    res.Message = model.Pay_Loc_Code_TEXT+"must be unique.";
                    res.IsSuccess = true;
                    return res;
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
          }

        public DataTable GetLocation(Location objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@LocID", objModel.Location_Id),
                   new SqlParameter("@ParentID", objModel.PARENT_LOC_ID),
                    new SqlParameter("@StateID", objModel.State_Id),
                     new SqlParameter("@EntityID", objModel.EID),
                    new SqlParameter("@LocName", objModel.Location_Name),
                   new SqlParameter("@UID", objModel.CreatedBy),
                };
                dt = DataLib.ExecuteDataTable("GetLocation", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable GetLoationHis(Location objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@LOCID", objModel.Location_Id),
                     new SqlParameter("@EntityID", objModel.EID),
                };
                dt = DataLib.ExecuteDataTable("GetLocationHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable GetLoationName(Location objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@STATEID", objModel.State_Id),
                new SqlParameter("@LOCName", objModel.Location_Name),
                };
                dt = DataLib.ExecuteDataTable("GetLocationName", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string UploadLocationDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
        {
            try
            {
                StateModel objStateM = new StateModel();
                StateRepo objStateRepo = new StateRepo();
                GlobalLocationRepo objGlobalRepo = new GlobalLocationRepo();
                GlobalLocationModel objGlobalM = new GlobalLocationModel();
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

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_LOCATION", "ViewLocation"))
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
                Location Model = new Location();
                LocationModelVM ModelVW = new LocationModelVM();
                Model.EID = EID;
                Model.SetDisplayName();
                string strerr = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        string TID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        ModelVW.Location_Id = Convert.ToInt32(TID);
                        ModelVW.Erp_Loc_Code = dt.Rows[i][Model.Erp_Loc_Code_TEXT].ToString();
                        ModelVW.Pay_Loc_Code = dt.Rows[i][Model.Pay_Loc_Code_TEXT].ToString();
                        ModelVW.Location_Name = dt.Rows[i][Model.Location_Name_TEXT].ToString();
                        ModelVW.State_Id = dt.Rows[i][Model.State_Id_TEXT].ToString();
                        ModelVW.Parent_Location_Name= dt.Rows[i][Model.PARENT_LOC_ID_TEXT].ToString();
                        ModelVW.Is_Metro = Convert.ToBoolean(dt.Rows[i][Model.Is_Metro_TEXT].ToString() == "1" ? true : false);
                        ModelVW.IsActive = Convert.ToBoolean(dt.Rows[i]["ISACTIVE"].ToString() == "1" ? true : false);
                        ModelVW.CreatedBy = CreatedBy;

                        var results = new List<ValidationResult>();
                        var vc = new ValidationContext(ModelVW, null, null);
                        var isValid = Validator.TryValidateObject(ModelVW, vc, results, true);
                        var errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                        
                        strerr = MasterMetaRepo.GenerateError(ModelVW, results);
                        if (isValid)
                        {
                       
                            Model.Location_Id = ModelVW.Location_Id;
                            Model.Erp_Loc_Code = ModelVW.Erp_Loc_Code;
                            Model.Pay_Loc_Code = ModelVW.Pay_Loc_Code;
                            Model.Is_Metro = ModelVW.Is_Metro;
                            Model.IsActive = ModelVW.IsActive;
                            Model.Parent_Location_Name = ModelVW.Parent_Location_Name;
                            Model.Location_Name = ModelVW.Location_Name;
                            Model.CreatedBy = CreatedBy;
                            var State_ID = objStateRepo.GetState();
                            DataView dvState = new DataView(State_ID);
                            dvState.RowFilter = "STATE_NAME='" + ModelVW.State_Id + "'";
                            DataTable dtState = dvState.ToTable();

                            if (dtState.Rows.Count > 0)
                            {
                                Model.State_Id = Convert.ToInt32(dtState.Rows[0]["TID"]);
                            }
                            else
                            {
                                Model.State_Id = -1;
                            }
                            var LOC_ID = objGlobalRepo.GetGlobalLocation(objGlobalM);
                            DataView dvLocName = new DataView(LOC_ID);
                            dvLocName.RowFilter = "LOC_NAME='" + ModelVW.Parent_Location_Name + "'";
                            DataTable dtLocName = dvLocName.ToTable();

                            if (dtLocName.Rows.Count > 0)
                            {
                                Model.PARENT_LOC_ID = Convert.ToInt32(dtLocName.Rows[0]["TID"]);
                            }
                            else
                            {
                                Model.PARENT_LOC_ID = -1;
                            }
                            
                            var results_Model = new List<ValidationResult>();
                            var vc_Model = new ValidationContext(Model, null, null);
                            var isValid_Model = Validator.TryValidateObject(Model, vc_Model, results, true);
                            var errors_Model = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                            //strerr = string.Join(" ", errors);
                            strerr = MasterMetaRepo.GenerateError(Model, results);
                            if (isValid_Model)
                            {

                                ret = AddUpdateLocation(Model);
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