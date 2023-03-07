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
    public class LwfRepo
    {
        public object SqlDataLib { get; private set; }
        public Response AddUpdateLwf(LWFModel model)
        {
            Response res = new Response();
            try
            {
                if (model.Eff_From_DT != null)
                {
                    string[] arrDate = model.Eff_From_DT.Split('/');
                    model.Eff_From_DT = arrDate[2] + "-" + arrDate[1] + "-" + arrDate[0];
                }
                if (model.Eff_To_DT != null)
                {
                    string[] arrDate = model.Eff_To_DT.Split('/');
                    model.Eff_To_DT = arrDate[2] + "-" + arrDate[1] + "-" + arrDate[0];
                }
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@LWFID",model.LWFID),
                   // new SqlParameter("@ENTITY_ID", model.EID),
                    new SqlParameter("@StateId",model.State_Id),
                    new SqlParameter("@LocationId",model.Location_Id),
                    new SqlParameter("@Periodflag",model.Period_Flag),
                    new SqlParameter("@DedMonth",model.Ded_Month),
                    new SqlParameter("@LwfEmployee",model.Lwf_Employee),
                    new SqlParameter("@LwfEmployer",model.Lwf_Employer),
                  //  new SqlParameter("@CreatedBy",model.CreatedBy),
                     new SqlParameter("@UID",model.CreatedBy),
                     new SqlParameter("@EFF_FROM_DT",model.Eff_From_DT),
                     new SqlParameter("@EFF_TO_DT",model.Eff_To_DT),
                    new SqlParameter("@ISACT",model.IsActive)
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateLwf", CommandType.StoredProcedure, parameters));
                Response.operation opration = Response.operation.ADD;
                if (model.LWFID > 0)
                    opration = Response.operation.Update;
                res = res.GetResponse(opration, "Lwf", result, "LWF");

                if (result == -2)
                {
                    res.Message = "Creation failed. Combination of Entity Name,Code must be unique.";
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

        public DataTable MonthDropDown()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TID", typeof(int));
            dt.Columns.Add("MonthName", typeof(String));
            dt.Rows.Add(1, "January");
            dt.Rows.Add(2, "February");
            dt.Rows.Add(3, "March");
            dt.Rows.Add(4, "April");
            dt.Rows.Add(5, "May");
            dt.Rows.Add(6, "June");
            dt.Rows.Add(7, "July");
            dt.Rows.Add(8, "August");
            dt.Rows.Add(9, "September");
            dt.Rows.Add(10, "October");
            dt.Rows.Add(11, "November");
            dt.Rows.Add(12, "December");
            return dt;
        }
        public DataTable GetLwf(LWFModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@LwfID", objModel.LWFID),
                    new SqlParameter("@StateID", objModel.State_Id),
                
                };
                dt = DataLib.ExecuteDataTable("GetLwf", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable GetLwfHis(LWFModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@HISID", objModel.LWFID),
                };
                dt = DataLib.ExecuteDataTable("GetLwfHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string UploadLWFDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
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

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_LWF", "ViewLwf"))
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
                LWFModel Model = new LWFModel();
                LWFModelVW ModelVW = new LWFModelVW();
                Model.EID = EID;
                Model.SetDisplayName();
                string strerr = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        string TID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        ModelVW.LWFID = Convert.ToInt32(TID);
                        ModelVW.StateName = dt.Rows[i][Model.State_Id_TEXT].ToString();
                        ModelVW.LocationName = dt.Rows[i][Model.Location_Id_TEXT].ToString();
                        ModelVW.Lwf_Employee = dt.Rows[i][Model.Lwf_Employee_TEXT].ToString();
                        ModelVW.Lwf_Employer = dt.Rows[i][Model.Lwf_Employer_TEXT].ToString();
                        ModelVW.Period_Flag = dt.Rows[i][Model.Period_Flag_TEXT].ToString();

                        ModelVW.DedMonthName = dt.Rows[i][Model.Ded_Month_TEXT].ToString();
                        ModelVW.Eff_From_DT = dt.Rows[i][Model.Eff_From_DT_TEXT].ToString();
                        ModelVW.Eff_To_DT = dt.Rows[i][Model.Eff_To_DT_TEXT].ToString();
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

                            Model.Lwf_Employee = ModelVW.Lwf_Employee;
                            Model.Lwf_Employer = ModelVW.Lwf_Employer;
                            Model.Eff_From_DT = ModelVW.Eff_From_DT;
                            Model.Eff_To_DT = ModelVW.Eff_To_DT;
                            Model.Period_Flag = ModelVW.Period_Flag;
                            //Model.BANK_CODE = ModelVW.BANK_CODE;
                            //Model.BANK_PHONE = ModelVW.BANK_PHONE;
                            //Model.BANK_PIN = ModelVW.BANK_PIN;
                            //Model.IFSC_Code = ModelVW.IFSC_Code;
                            Model.CreatedBy = CreatedBy;
                            var State_ID = objStateRepo.GetState();
                            DataView dvState = new DataView(State_ID);
                            dvState.RowFilter = "STATE_NAME='" + ModelVW.StateName + "'";
                            DataTable dtState = dvState.ToTable();
                            if (dtState.Rows.Count > 0)
                            {
                                Model.State_Id = Convert.ToInt32(dtState.Rows[0]["TID"]);
                            }
                            else
                            {
                                Model.State_Id = -1;
                            }
                            var Month_ID = MonthDropDown();
                            DataView dvMonthName = new DataView(Month_ID);
                            dvMonthName.RowFilter = "MonthName='" + ModelVW.DedMonthName + "'";
                            DataTable dtMonthName = dvMonthName.ToTable();

                            if (dtMonthName.Rows.Count > 0)
                            {
                                Model.Ded_Month = Convert.ToInt32(dtMonthName.Rows[0]["TID"]);
                            }
                            else
                            {
                                Model.Ded_Month = -1;
                            }
                            var LOC_ID = objGlobalRepo.GetGlobalLocation(objGlobalM);
                            DataView dvLocName = new DataView(LOC_ID);
                            dvLocName.RowFilter = "LOC_NAME='" + ModelVW.LocationName + "'";
                            DataTable dtLocName = dvLocName.ToTable();

                            if (dtLocName.Rows.Count > 0)
                            {
                                Model.Location_Id = Convert.ToInt32(dtLocName.Rows[0]["TID"]);
                            }
                            else
                            {
                                Model.Location_Id = -1;
                            }



                            var results_Model = new List<ValidationResult>();
                            var vc_Model = new ValidationContext(Model, null, null);
                            var isValid_Model = Validator.TryValidateObject(Model, vc_Model, results, true);
                            var errors_Model = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                            //strerr = string.Join(" ", errors);
                            strerr = MasterMetaRepo.GenerateError(Model, results);
                            if (isValid_Model)
                            {

                                ret = AddUpdateLwf(Model);


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
 
