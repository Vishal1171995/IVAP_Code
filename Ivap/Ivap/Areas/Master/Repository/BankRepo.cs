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
    public class BankRepo
    {
        public Response AddUpdateBank(BankModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@BANKID",model.BANKID),
                    new SqlParameter("@ENTITYID",model.EID),
                    new SqlParameter("@GLOBAL_BANK_ID",model.GLOBAL_BANK_ID),
                    new SqlParameter("@BANK_NAME",model.BANK_NAME),
                    new SqlParameter("@BANK_ADDR",model.BANK_ADDR),
                    new SqlParameter("@BANK_CITY",model.BANK_CITY),
                    new SqlParameter("@BANK_STATE",model.BANK_STATE),
                    new SqlParameter("@BANK_PIN",model.BANK_PIN),
                    new SqlParameter("@BANK_PHONE",model.BANK_PHONE),
                    new SqlParameter("@CREATED_BY",model.CreatedBy),
                    new SqlParameter("@ISACT",model.IsActive),
                     new SqlParameter("@ERP_BANK_CODE",model.ERP_BANK_CODE),
                    new SqlParameter("@PAY_BANK_CODE",model.PAY_BANK_CODE),
                    new SqlParameter("@IFSC",model.IFSC_Code)
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateBANK", CommandType.StoredProcedure, parameters));
                // Response.operation opration = Response.operation.ADD;
                if (result > 0)
                {
                    res.Message = "Bank created Successfully.";
                    res.IsSuccess = true;
                    return res;
                }
                if (result == 0)
                {
                   
                        res.Message = "Bank updated Successfully.";
                        res.IsSuccess = true;
                        return res;
                    
                }
            
                if (result == -2)
                {
                    res.Message = "Creation failed. "+ model.PAY_BANK_CODE_TEXT+"must be unique.";
                    res.IsSuccess = false;
                    return res;
                }
                if (result == -3)
                {
                    res.Message = "Creation failed. "+ model.ERP_BANK_CODE_TEXT+" must be unique.";
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
        public DataTable GetBank(BankModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@BANKID",model.BANKID),
                    new SqlParameter("@ENTITYID",model.EID),
                    new SqlParameter("@ISACT",model.IsActive),
                    new SqlParameter("@UID",model.CreatedBy),
                };

                return DataLib.ExecuteDataTable("GetBank", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }


        public DataTable GetGlobalBank(BankModel objModel)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@BANK_ID", objModel.GLOBAL_BANK_ID),
                   
                };
                return DataLib.ExecuteDataTable("GETGLOBALBANK", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetGlobalBank_Upload()
        {
            DataSet ds = new DataSet();
            try
            {
              
                return DataLib.ExecuteDataTable("GETGLOBALBANK", CommandType.StoredProcedure, null);
            }
            catch
            {
                throw;
            }
        }


        public DataTable GetBankHistory(BankModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@BANKID",model.BANKID ),
                };
                return DataLib.ExecuteDataTable("GetBankHis", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
        public string UploadBankDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
        {
            try
            {
                StateModel objStateM = new StateModel();
                StateRepo objStateRepo = new StateRepo();
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

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_BANK", "ViewBank"))
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
                BankModel Model = new BankModel();
                BankVM ModelVW = new BankVM();
                Model.EID = EID;
                Model.SetDisplayName();

                ModelVW.EID = EID;
                ModelVW.SetDisplayName();
                string strerr = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        string TID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        ModelVW.BANKID = Convert.ToInt32(TID);
                        ModelVW.Global_Bank_Name = dt.Rows[i][Model.GLOBAL_BANK_NAME_TEXT].ToString();
                        ModelVW.BANK_STATE = dt.Rows[i][Model.BANK_STATE_TEXT].ToString();
                        ModelVW.BANK_NAME = dt.Rows[i][Model.BANK_NAME_TEXT].ToString();
                        ModelVW.BANK_ADDR = dt.Rows[i][Model.BANK_ADDR_TEXT].ToString();
                        ModelVW.BANK_CITY = dt.Rows[i][Model.BANK_CITY_TEXT].ToString();
                        //ModelVW.BANK_CODE = dt.Rows[i][Model.BANK_CODE_TEXT].ToString();

                        ModelVW.BANK_PHONE = dt.Rows[i][Model.BANK_PHONE_TEXT].ToString();
                        ModelVW.BANK_PIN = dt.Rows[i][Model.BANK_PIN_TEXT].ToString();
                        ModelVW.ERP_BANK_CODE = dt.Rows[i][Model.ERP_BANK_CODE_TEXT].ToString();
                        ModelVW.PAY_BANK_CODE = dt.Rows[i][Model.PAY_BANK_CODE_TEXT].ToString();
                        ModelVW.IFSC_Code = dt.Rows[i][Model.IFSC_Code_TEXT].ToString();


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

                            Model.ERP_BANK_CODE = ModelVW.ERP_BANK_CODE;
                            Model.PAY_BANK_CODE = ModelVW.PAY_BANK_CODE;
                            Model.BANK_NAME = ModelVW.BANK_NAME;
                            Model.BANK_ADDR = ModelVW.BANK_ADDR;
                            Model.BANK_CITY = ModelVW.BANK_CITY;
                            Model.BANK_CODE = ModelVW.BANK_CODE;
                            Model.BANK_PHONE = ModelVW.BANK_PHONE;
                            Model.BANK_PIN = ModelVW.BANK_PIN;
                            Model.IFSC_Code = ModelVW.IFSC_Code;
                            Model.IsActive = ModelVW.IsActive;
                            Model.CreatedBy = CreatedBy;
                            //Bank Name
                            var BankNameList = GetGlobalBank_Upload();
                            DataView dvBankName = new DataView(BankNameList);

                            dvBankName.RowFilter = "BANK_NAME='" + ModelVW.Global_Bank_Name + "'";
                            DataTable dtBank = dvBankName.ToTable();

                            if (dtBank.Rows.Count > 0)
                            {
                                Model.GLOBAL_BANK_ID = Convert.ToInt32(dtBank.Rows[0]["TID"]);
                            }
                            else
                            {
                                Model.GLOBAL_BANK_ID = -1;
                            }



                            //Bank END
                            var State_ID = objStateRepo.GetState();
                            DataView dvState = new DataView(State_ID);
                            dvState.RowFilter = "STATE_NAME='" + ModelVW.BANK_STATE + "'";
                            DataTable dtState = dvState.ToTable();

                            if (dtState.Rows.Count > 0)
                            {
                                Model.BANK_STATE = Convert.ToInt32(dtState.Rows[0]["TID"]);
                            }
                            else
                            {
                                Model.BANK_STATE = 0;
                            }




                            var results_Model = new List<ValidationResult>();
                            var vc_Model = new ValidationContext(Model, null, null);
                            var isValid_Model = Validator.TryValidateObject(Model, vc_Model, results, true);
                            var errors_Model = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                            //strerr = string.Join(" ", errors);
                            strerr = MasterMetaRepo.GenerateError(Model, results);
                            if (isValid_Model)
                            {

                                ret = AddUpdateBank(Model);


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