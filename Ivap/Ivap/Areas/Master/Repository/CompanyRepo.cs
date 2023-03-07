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
    public class CompanyRepo
    {
        public object SqlDataLib { get; private set; }
        public Response AddUpdateCompany(CompanyModel model)
        {
            Response Res = new Response();
            try
            {
                if (model.SIGN_DATE != null && model.SIGN_DATE != "")
                {
                    string[] arrDate = model.SIGN_DATE.Split('/');
                    model.SIGN_DATE = arrDate[2] + "-" + arrDate[1] + "-" + arrDate[0];
                }
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@COMPID",model.CompID),
                    new SqlParameter("@EID",model.EID),
                    new SqlParameter("@COMP_CODE",model.COMP_CODE),
                    new SqlParameter("@COMP_NAME",model.COMP_NAME),
                    new SqlParameter("@COMP_ADDR1",model.COMP_ADDR1),
                    new SqlParameter("@COMP_ADDR2",model.COMP_ADDR2),
                    new SqlParameter("@COMP_CITY",model.COMP_CITY),
                    new SqlParameter("@COMP_STATE",model.COMP_STATE),
                    new SqlParameter("@COMP_PIN",model.COMP_PIN),
                    new SqlParameter("@COMP_CLASS",model.COMP_CLASS),
                    new SqlParameter("@COMP_PANNO",model.COMP_PANNO),
                    new SqlParameter("@COMP_TANNO",model.COMP_TANNO),
                    new SqlParameter("@COMP_TDSCIRCLE",model.COMP_TDSCIRCLE),
                    new SqlParameter("@SIGN_FNAME",model.SIGN_FNAME),
                    new SqlParameter("@SIGN_LNAME",model.SIGN_LNAME),
                    new SqlParameter("@SIGN_FATHER_NAME",model.SIGN_FATHER_NAME),
                    new SqlParameter("@SIGN_ADDR1",model.SIGN_ADDR1),
                    new SqlParameter("@SIGN_ADDR2",model.SIGN_ADDR2),
                    new SqlParameter("@SIGN_CITY",model.SIGN_CITY),
                    new SqlParameter("@SIGN_DSG",model.SIGN_DSG),
                    new SqlParameter("@SIGN_STATE",model.SIGN_STATE),
                    new SqlParameter("@SIGN_PIN",model.SIGN_PIN),
                    new SqlParameter("@SIGN_PLACE",model.SIGN_PLACE),
                    new SqlParameter("@SIGN_DATE",model.SIGN_DATE),
                    new SqlParameter("@RETIRE_AGE",model.RETIRE_AGE),
                    new SqlParameter("@EMP_CODE_GEN",model.EMP_CODE_GEN),
                    new SqlParameter("@EMP_CODE_PREFIX",model.EMP_CODE_PREFIX),
                    new SqlParameter("@EMP_CODE_LEN",model.EMP_CODE_LEN),
                    new SqlParameter("@Comp_Logo",model.Comp_Logo),
                    new SqlParameter("@COMP_URL",model.COMP_URL),
                    new SqlParameter("@CREATED_BY",model.CreatedBy),
                    new SqlParameter("@ISACT",model.IsActive)
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateCompany", CommandType.StoredProcedure, parameters));
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
                    Res.Message = "Failed!!! " + model.COMP_NAME_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                else if (result == -2)
                {
                    Res.Message = "Failed!!! " + model.COMP_CODE_TEXT + " must be unique.";
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
        public DataTable GetCompany(CompanyModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@COMPID", objModel.CompID),
                    new SqlParameter("@EID", objModel.EID),
                    new SqlParameter("@StateID", objModel.COMP_STATE),
                    new SqlParameter("@COMP_CODE", objModel.COMP_CODE),
                    new SqlParameter("@COMP_NAME", objModel.COMP_NAME),
                    new SqlParameter("@ISACT", objModel.IsActive),
                     new SqlParameter("@UID", objModel.CreatedBy),
                };
                dt = DataLib.ExecuteDataTable("GetCompany", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetCompHis(CompanyModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@CompID", objModel.CompID),
                };
                dt = DataLib.ExecuteDataTable("GetCompanyHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string UploadCompanyDetails(string FilePath, int CreatedBy,int EID, ref int SuccessCount, ref int FailCount)
        {
            try
            {
                RoleRepo RoleR = new RoleRepo();
                RoleModel RoleM = new RoleModel();
                StateModel objStateM = new StateModel();
                StateRepo objStateRepo = new StateRepo();
                ClassModel objClassM = new ClassModel();
                ClassRepo objClassRepo = new ClassRepo();
                //string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + filename;  
                //Convert csv file into DataTable
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

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_COMPANY", "ViewCompany"))
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
                CompanyModel CompModel = new CompanyModel();
                CompanyModelVM CompVM = new CompanyModelVM();
                CompModel.EID = EID;
                CompModel.SetDisplayName();
                CompVM.EID = EID;
                CompVM.SetDisplayName();
                string strerr = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        //CompVM = new CompanyModelVM();
                        string CompID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        CompVM.CompID = Convert.ToInt32(CompID);
                        CompVM.EID = EID;
                        CompVM.COMP_CODE = Convert.ToString(dt.Rows[i][CompModel.COMP_CODE_TEXT]).ToString().Trim();
                        CompVM.COMP_NAME = Convert.ToString((dt.Rows[i][CompModel.COMP_NAME_TEXT]).ToString().Trim());
                        CompVM.COMP_ADDR1 = Convert.ToString((dt.Rows[i][CompModel.COMP_ADDR1_TEXT]).ToString().Trim());
                        CompVM.COMP_ADDR2 = Convert.ToString((dt.Rows[i][CompModel.COMP_ADDR2_TEXT]).ToString().Trim());
                        CompVM.COMP_CITY = Convert.ToString((dt.Rows[i][CompModel.COMP_CITY_TEXT]).ToString().Trim());
                        CompVM.COMP_STATE = Convert.ToString((dt.Rows[i][CompModel.COMP_STATE_TEXT]).ToString().Trim());
                        CompVM.COMP_PIN = Convert.ToString((dt.Rows[i][CompModel.COMP_PIN_TEXT]).ToString().Trim());
                        CompVM.COMP_CLASS = Convert.ToString((dt.Rows[i][CompModel.COMP_CLASS_TEXT]).ToString().Trim());
                        CompVM.COMP_PANNO = Convert.ToString((dt.Rows[i][CompModel.COMP_PANNO_TEXT]).ToString().Trim());
                        CompVM.COMP_TANNO = Convert.ToString((dt.Rows[i][CompModel.COMP_TANNO_TEXT]).ToString().Trim());
                        CompVM.COMP_TDSCIRCLE = Convert.ToString((dt.Rows[i][CompModel.COMP_TDSCIRCLE_TEXT]).ToString().Trim());
                        CompVM.SIGN_FNAME = Convert.ToString((dt.Rows[i][CompModel.SIGN_FNAME_TEXT]).ToString().Trim());
                        CompVM.SIGN_LNAME = Convert.ToString((dt.Rows[i][CompModel.SIGN_LNAME_TEXT]).ToString().Trim());
                        CompVM.SIGN_FATHER_NAME = Convert.ToString((dt.Rows[i][CompModel.SIGN_FNAME_TEXT]).ToString().Trim());
                        CompVM.SIGN_ADDR1 = Convert.ToString((dt.Rows[i][CompModel.SIGN_ADDR1_TEXT]).ToString().Trim());
                        CompVM.SIGN_ADDR2 = Convert.ToString((dt.Rows[i][CompModel.SIGN_ADDR2_TEXT]).ToString().Trim());
                        CompVM.SIGN_CITY = Convert.ToString((dt.Rows[i][CompModel.SIGN_CITY_TEXT]).ToString().Trim());
                        CompVM.SIGN_DSG = Convert.ToString((dt.Rows[i][CompModel.SIGN_DSG_TEXT]).ToString().Trim());
                        CompVM.SIGN_STATE = Convert.ToString((dt.Rows[i][CompModel.SIGN_STATE_TEXT]).ToString().Trim());
                        CompVM.SIGN_PIN = Convert.ToString((dt.Rows[i][CompModel.SIGN_PIN_TEXT]).ToString().Trim());
                        CompVM.SIGN_PLACE = Convert.ToString((dt.Rows[i][CompModel.SIGN_PLACE_TEXT]).ToString().Trim());
                        CompVM.SIGN_DATE = Convert.ToString((dt.Rows[i][CompModel.SIGN_DATE_TEXT]).ToString().Trim());
                        CompVM.RETIRE_AGE = Convert.ToInt32(dt.Rows[i][CompModel.RETIRE_AGE_TEXT].ToString().Trim() == "" ? "0" : dt.Rows[i][CompModel.RETIRE_AGE_TEXT].ToString().Trim());
                        CompVM.EMP_CODE_GEN = Convert.ToBoolean(dt.Rows[i][CompModel.EMP_CODE_GEN_TEXT].ToString().Trim() == "1" ? true : false);
                        CompVM.EMP_CODE_PREFIX = Convert.ToString((dt.Rows[i][CompModel.EMP_CODE_PREFIX_TEXT]).ToString().Trim());
                        CompVM.EMP_CODE_LEN = Convert.ToInt32(dt.Rows[i][CompModel.EMP_CODE_LEN_TEXT].ToString().Trim() == "" ? "0" : dt.Rows[i][CompModel.EMP_CODE_LEN_TEXT].ToString().Trim());
                        CompVM.COMP_URL = Convert.ToString((dt.Rows[i][CompModel.COMP_URL_TEXT]).ToString().Trim());
                        CompVM.Comp_Logo = "";
                        CompVM.IsActive = Convert.ToBoolean(dt.Rows[i]["ISACTIVE"].ToString() == "1" ? true : false);
                        CompVM.CreatedBy = CreatedBy;

                        var results = new List<ValidationResult>();
                        var vc = new ValidationContext(CompVM, null, null);
                        var isValid = Validator.TryValidateObject(CompVM, vc, results, true);
                        var errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                        //strerr = string.Join(" ", errors);
                        strerr = MasterMetaRepo.GenerateError(CompVM, results);
                        if (isValid)
                        {
                            //CompModel = new CompanyModel();
                            CompModel.EID = CompVM.EID;
                            CompModel.CompID = CompVM.CompID;
                            CompModel.COMP_CODE = CompVM.COMP_CODE;
                            CompModel.COMP_NAME = CompVM.COMP_NAME;
                            CompModel.COMP_ADDR1 = CompVM.COMP_ADDR1;
                            CompModel.COMP_ADDR2 = CompVM.COMP_ADDR2;
                            CompModel.COMP_CITY = CompVM.COMP_CITY;
                            //CompModel.COMP_STATE= CompVM.COMP_STATE;
                            CompModel.COMP_PIN = CompVM.COMP_PIN;
                            //CompModel.COMP_CLASS= CompVM.COMP_CLASS;
                            CompModel.COMP_PANNO = CompVM.COMP_PANNO;
                            CompModel.COMP_TANNO = CompVM.COMP_TANNO;
                            CompModel.COMP_TDSCIRCLE = CompVM.COMP_TDSCIRCLE;
                            CompModel.SIGN_FNAME = CompVM.SIGN_FNAME;
                            CompModel.SIGN_LNAME = CompVM.SIGN_LNAME;
                            CompModel.SIGN_ADDR1 = CompVM.SIGN_ADDR1;
                            CompModel.SIGN_ADDR2 = CompVM.SIGN_ADDR2;
                            CompModel.SIGN_CITY = CompVM.SIGN_CITY;
                            CompModel.SIGN_DSG = CompVM.SIGN_DSG;
                            //CompModel.SIGN_STATE= CompVM.SIGN_STATE;
                            CompModel.SIGN_PIN = CompVM.SIGN_PIN;
                            CompModel.SIGN_PLACE = CompVM.SIGN_PLACE;
                            CompModel.SIGN_DATE = CompVM.SIGN_DATE;
                            CompModel.RETIRE_AGE = CompVM.RETIRE_AGE;
                            CompModel.EMP_CODE_GEN = CompVM.EMP_CODE_GEN;
                            CompModel.EMP_CODE_PREFIX = CompVM.EMP_CODE_PREFIX;
                            CompModel.EMP_CODE_LEN = CompVM.EMP_CODE_LEN;
                            CompModel.Comp_Logo= CompVM.Comp_Logo;
                            CompModel.COMP_URL = CompVM.COMP_URL;
                            CompModel.CreatedBy = CompVM.CreatedBy;
                            CompModel.IsActive = CompVM.IsActive;

                            objStateM.StateId = 0;
                            //objStateM.State_Name = CompVM.COMP_STATE;

                            var StateID = objStateRepo.GetState();
                            DataView dvState = new DataView(StateID);
                            dvState.RowFilter = "STATE_NAME ='" + CompVM.COMP_STATE + "'";
                            DataTable dtState = dvState.ToTable();

                            if (dtState.Rows.Count > 0)
                            {
                                CompModel.COMP_STATE = Convert.ToInt32(dtState.Rows[0]["TID"]);
                            }
                            else
                            {
                                CompModel.COMP_STATE = -1;
                            }
                            objClassM.CID = 0;
                            var ClassID = objClassRepo.GetClass(objClassM);

                            DataView dvClass = new DataView(ClassID);
                            dvClass.RowFilter = "CLASS_NAME ='" + CompVM.COMP_CLASS + "'";
                            DataTable dtClass = dvClass.ToTable();

                            if (dtClass.Rows.Count > 0)
                            {
                                CompModel.COMP_CLASS = Convert.ToInt32(dtClass.Rows[0]["TID"]);
                            }
                            else
                            {
                                CompModel.COMP_CLASS = -1;
                            }
                            objStateM.StateId = 0;
                            var SignStateID = objStateRepo.GetState();
                            DataView dvSignState = new DataView(SignStateID);
                            dvSignState.RowFilter = "STATE_NAME ='" + CompVM.SIGN_STATE + "'";
                            DataTable dtSignState = dvSignState.ToTable();
                            if (dtSignState.Rows.Count > 0)
                            {
                                CompModel.SIGN_STATE = Convert.ToInt32(dtSignState.Rows[0]["TID"]);
                            }
                            else
                            {
                                CompModel.SIGN_STATE = -1;
                            }

                            results = new List<ValidationResult>();
                            vc = new ValidationContext(CompModel, null, null);
                            isValid = Validator.TryValidateObject(CompModel, vc, results, true);
                            errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                            //strerr = string.Join(" ", errors);
                            strerr = MasterMetaRepo.GenerateError(CompModel, results);
                            if (isValid)
                            {
                                ret = AddUpdateCompany(CompModel);

                                if (ret.IsSuccess == true)
                                {
                                    SuccessCount += 1;
                                    dt.Rows[i]["Message"] = ret.Message;
                                    dt.Rows[i]["Response"] = "Success";
                                }
                                else
                                {
                                    FailCount += 1;
                                    if (ret.Data == "-1")
                                    {
                                        dt.Rows[i]["Message"] = "Duplicate";
                                    }
                                    else
                                    {
                                        dt.Rows[i]["Message"] = ret.Message;
                                    }
                                    dt.Rows[i]["Response"] = "Failed";
                                }
                            }
                            else
                            {
                                FailCount += 1;
                                dt.Rows[i]["Response"] = strerr;
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
                    catch(Exception ex)
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