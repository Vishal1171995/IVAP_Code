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
    public class ClassRepo
    {
        public Response AddUpdateClass(ClassModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@CID",model.CID),
                new SqlParameter("@ENTITY_ID", model.EID),
                new SqlParameter("@PAY_CLASS_CODE", model.PAY_CLASS_CODE),
                new SqlParameter("@ERP_CLASS_CODE", model.ERP_CLASS_CODE),
                new SqlParameter("@CLASS_NAME", model.CLASS_NAME),
               
                 new SqlParameter("@UID", model.CreatedBy),
                  new SqlParameter("@ISACTIVE", model.IsActive),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateClass", CommandType.StoredProcedure, parameters));
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
                    res.Message = model.CLASS_NAME_TEXT + " must be unique.";
                    res.IsSuccess = true;
                    return res;
                }
                if (result == -2)
                {
                    res.Message = model.ERP_CLASS_CODE_TEXT + " must be unique.";
                    res.IsSuccess = true;
                    return res;
                }
                if (result == -3)
                {
                    res.Message = model.PAY_CLASS_CODE_TEXT + "must be unique.";
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
        public DataTable GetClass(ClassModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ClassID", objModel.CID),
                    new SqlParameter("@ClassCode",objModel.PAY_CLASS_CODE ),
                     new SqlParameter("@EntityID",objModel.EID ),
                    new SqlParameter("@ClassName", objModel.CLASS_NAME),
                    new SqlParameter("@ISAct", objModel.IsActive),
                    new SqlParameter("@UID", objModel.CreatedBy),

                };
                dt = DataLib.ExecuteDataTable("GetClass", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetClassHis(ClassModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@CID", objModel.CID),
                     new SqlParameter("@EntityID", objModel.EID),
                };
                dt = DataLib.ExecuteDataTable("GetClassHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public string UploadClassDetails(string FilePath, int CreatedBy, int EID, ref int SuccessCount, ref int FailCount)
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

                if (!ExcellUtils.CheckColumnFormat(FilePath, EID, "IVAP_MST_CLASS", "ViewClass"))
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
                ClassModel Model = new ClassModel();
                Model.EID = EID;
                Model.SetDisplayName();
                string strerr = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        string TID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        Model.CID = Convert.ToInt32(TID);
                        Model.EID = EID;
                        Model.PAY_CLASS_CODE = Convert.ToString((dt.Rows[i][Model.PAY_CLASS_CODE_TEXT]).ToString().Trim());
                        Model.ERP_CLASS_CODE = Convert.ToString((dt.Rows[i][Model.ERP_CLASS_CODE_TEXT]).ToString().Trim());
                        Model.CLASS_NAME = Convert.ToString((dt.Rows[i][Model.CLASS_NAME_TEXT]).ToString().Trim());
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
                            ret = AddUpdateClass(Model);

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