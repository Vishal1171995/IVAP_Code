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
    public class PTaxRepo
    {
        public Response AddUpdatePTax(PTAXModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@PTAXID",model.PTAXID),
                    new SqlParameter("@STATE_ID",model.STATE_ID),
                    new SqlParameter("@PERIOD_FLAG",model.PERIOD_FLAG),
                    new SqlParameter("@DED_MONTH",model.DED_MONTH),
                    new SqlParameter("@YTD_MONTH_FROM",model.YTD_MONTH_FROM),
                       new SqlParameter("@YTD_MONTH_TO",model.YTD_MONTH_TO),
                    new SqlParameter("@PT_SAL_FROM",model.PT_SAL_FROM),
                    new SqlParameter("@PT_SAL_TO",model.PT_SAL_TO),
                    new SqlParameter("@GENDER",model.GENDER),
                     new SqlParameter("@PTAX",model.PTAX),
                     new SqlParameter("@EFF_FROM_DT",model.EFF_FROM_DT),
                     new SqlParameter("@EFF_TO_DT",model.EFF_TO_DT),
                    new SqlParameter("@CREATED_BY",model.CreatedBy),
                     
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdatePTAX", CommandType.StoredProcedure, parameters));
                Response.operation opration = Response.operation.ADD;
                if (model.PTAXID > 0)
                    opration = Response.operation.Update;
                res = res.GetResponse(opration, "PTax", result, "");
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
         
        public DataTable GetPTax(PTAXModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@PTAXID",model.PTAXID),
                    //new SqlParameter("@CompID",model.COMP_ID),
                };

                return DataLib.ExecuteDataTable("GetPTAX", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetPTaxHistory(PTAXModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@PTAXID",model.PTAXID ),
                };

                return DataLib.ExecuteDataTable("GetPTAXHis", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }

        public string UploadPTAXDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
        {
            try
            {
                StateModel objStateM = new StateModel();
                StateRepo objStateRepo = new StateRepo();
               
                LocationRepo objLocationRepo = new LocationRepo();
                Location objLocationM = new Location();

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

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_PTAX", "ViewPTAX"))
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
                PTAXModel Model = new PTAXModel();
                PtaxModelVW ModelVW = new PtaxModelVW();
                Model.EID = EID;
                Model.SetDisplayName();
                string strerr = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        string TID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        ModelVW.PTAXID = Convert.ToInt32(TID);
                        ModelVW.StateName = dt.Rows[i][Model.STATE_ID_TEXT].ToString();
                        ModelVW.DED_MONTH_ = dt.Rows[i][Model.DED_MONTH_TEXT].ToString();
                        ModelVW.YTD_MONTH_FROM_ = dt.Rows[i][Model.YTD_MONTH_FROM_TEXT].ToString();
                        ModelVW.YTD_MONTH_TO_ = dt.Rows[i][Model.YTD_MONTH_TO_TEXT].ToString();
                        ModelVW.GENDER = dt.Rows[i][Model.GENDER_TEXT].ToString();

                        ModelVW.EFF_FROM_DT = Convert.ToDateTime(dt.Rows[i][Model.EFF_FROM_DT_TEXT].ToString());
                        ModelVW.EFF_TO_DT = Convert.ToDateTime(dt.Rows[i][Model.EFF_TO_DT_TEXT].ToString());
                        ModelVW.PT_SAL_FROM = Convert.ToDecimal(dt.Rows[i][Model.PT_SAL_FROM_TEXT].ToString());
                        ModelVW.PT_SAL_TO = Convert.ToDecimal(dt.Rows[i][Model.PT_SAL_TO_TEXT].ToString());
                        ModelVW.PERIOD_FLAG = dt.Rows[i][Model.PERIOD_FLAG_TEXT].ToString();
                        ModelVW.PTAX = Convert.ToDecimal(dt.Rows[i][Model.PTAX_TEXT].ToString());

                        // ModelVW.IsActive = Convert.ToBoolean(dt.Rows[i]["ISACTIVE"].ToString() == "1" ? true : false);
                        ModelVW.CreatedBy = CreatedBy;

                        var results = new List<ValidationResult>();
                        var vc = new ValidationContext(ModelVW, null, null);
                        var isValid = Validator.TryValidateObject(ModelVW, vc, results, true);
                        var errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                        //strerr = string.Join(" ", errors);
                        strerr = MasterMetaRepo.GenerateError(ModelVW, results);
                        if (isValid)
                        {
                            //New Code
                            Model.EFF_FROM_DT = ModelVW.EFF_FROM_DT;
                            Model.EFF_TO_DT = ModelVW.EFF_TO_DT;
                            Model.PT_SAL_FROM = ModelVW.PT_SAL_FROM;
                            Model.PT_SAL_TO = ModelVW.PT_SAL_TO;
                            Model.GENDER = ModelVW.GENDER;
                            Model.PERIOD_FLAG = ModelVW.PERIOD_FLAG;
                             Model.PTAX = ModelVW.PTAX;
                            Model.CreatedBy = CreatedBy;
                            var State_ID = objStateRepo.GetState();
                            DataView dvState = new DataView(State_ID);
                            dvState.RowFilter = "STATE_NAME='" + ModelVW.StateName + "'";
                            DataTable dtState = dvState.ToTable();

                            if (dtState.Rows.Count > 0)
                            {
                                Model.STATE_ID = Convert.ToInt32(dtState.Rows[0]["TID"]);
                            }
                            else
                            {
                                Model.STATE_ID = -1;
                            }
                            var Month_ID = MonthDropDown();
                            DataView dvMonthName = new DataView(Month_ID);
                            dvMonthName.RowFilter = "MonthName='" + ModelVW.YTD_MONTH_FROM_ + "'";
                            DataTable dtMonthName = dvMonthName.ToTable();

                            if (dtMonthName.Rows.Count > 0)
                            {
                                Model.YTD_MONTH_FROM = Convert.ToInt32(dtMonthName.Rows[0]["TID"]);
                            }
                            else
                            {
                                Model.YTD_MONTH_FROM = -1;
                            }
                            var YTD_Month_To_ID = MonthDropDown();
                            DataView dvYTD_From_Name = new DataView(YTD_Month_To_ID);
                            dvYTD_From_Name.RowFilter = "MonthName='" + ModelVW.YTD_MONTH_TO_ + "'";
                            DataTable dtYTD_From_Name = dvYTD_From_Name.ToTable();

                            if (dtYTD_From_Name.Rows.Count > 0)
                            {
                                Model.YTD_MONTH_TO = Convert.ToInt32(dtYTD_From_Name.Rows[0]["TID"]);
                            }
                            else
                            {
                                Model.YTD_MONTH_TO = -1;
                            }

                            var DED = MonthDropDown();
                            DataView dv_DED_Name = new DataView(DED);
                            dv_DED_Name.RowFilter = "MonthName='" + ModelVW.DED_MONTH_ + "'";
                            DataTable dt_DED_Name = dv_DED_Name.ToTable();

                            if (dt_DED_Name.Rows.Count > 0)
                            {
                                Model.DED_MONTH = Convert.ToInt32(dt_DED_Name.Rows[0]["TID"]);
                            }
                            else
                            {
                                Model.DED_MONTH = -1;
                            }
                            var results_Model = new List<ValidationResult>();
                            var vc_Model = new ValidationContext(Model, null, null);
                            var isValid_Model = Validator.TryValidateObject(Model, vc_Model, results, true);
                            var errors_Model = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                            //strerr = string.Join(" ", errors);
                            strerr = MasterMetaRepo.GenerateError(Model, results);
                            if (isValid_Model)
                            {

                                ret = AddUpdatePTax(Model);


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