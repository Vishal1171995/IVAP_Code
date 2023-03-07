using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using Ivap.Areas.Master.Models;
using Ivap.Areas.Master.Repository;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Ivap.Areas.Master.Repository
{
    public class DepartmentRepo
    {
        public Response AddUpdateDepartment(DepartmentModel model)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@DEPTID",model.DEPTID),
                    new SqlParameter("@ENTITYID",model.ENTITY_ID),
                    new SqlParameter("@PAY_DEPT_CODE",model.PAY_DEPT_CODE),
                    new SqlParameter("@ERP_DEPT_CODE",model.ERP_DEPT_CODE),
                    new SqlParameter("@DEPT_NAME",model.DEPT_NAME),
                    new SqlParameter("@CREATED_BY",model.CreatedBy),
                    new SqlParameter("@ISACT",model.IsActive)
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateDepartment", CommandType.StoredProcedure, parameters));
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
                    Res.Message = "Failed!!! " + model.DEPT_NAME_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                else if (result == -2)
                {
                    Res.Message = "Failed!!! " + model.PAY_DEPT_CODE_TEXT + " must be unique.";
                    Res.IsSuccess = false;
                    return Res;
                }
                else if (result == -3)
                {
                    Res.Message = "Failed!!! " + model.ERP_DEPT_CODE_TEXT + " must be unique.";
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
        public DataTable GetDepartment(DepartmentModel model)
        {

            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@DEPTID",model.DEPTID),
                    new SqlParameter("@ENTITYID",model.ENTITY_ID),
                    new SqlParameter("@ISACT",model.IsActive),
                     new SqlParameter("@UID",model.CreatedBy),
                };

                return DataLib.ExecuteDataTable("GetDepartment", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetDeptHistory(DepartmentModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@DEPTID",model.DEPTID ),
                };
                return DataLib.ExecuteDataTable("GetDepartmentHis", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
        public string UploadDepartmentDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
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

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_DEPARTMENT", "ViewDepartment"))
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
                DepartmentModel Model = new DepartmentModel();
                Model.EID = EID;
                Model.SetDisplayName();
                string strerr = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        string TID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        Model.DEPTID = Convert.ToInt32(TID);
                        Model.ENTITY_ID = EID;
                        Model.PAY_DEPT_CODE = Convert.ToString((dt.Rows[i][Model.PAY_DEPT_CODE_TEXT]).ToString().Trim());
                        Model.ERP_DEPT_CODE = Convert.ToString((dt.Rows[i][Model.ERP_DEPT_CODE_TEXT]).ToString().Trim());
                        Model.DEPT_NAME = Convert.ToString((dt.Rows[i][Model.DEPT_NAME_TEXT]).ToString().Trim());

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
                            ret = AddUpdateDepartment(Model);

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