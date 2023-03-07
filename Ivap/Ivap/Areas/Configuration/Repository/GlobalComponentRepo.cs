using Ivap.Areas.Configuration.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace Ivap.Areas.Configuration.Repository
{
    public class GlobalComponentRepo
    {
        public Response AddUpdateComponent(GlobalComponentModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ComponentID",model.COMPONENTID),
                    new SqlParameter("@COMPONENT_FILE_TYPE",model.COMPONENT_FILE_TYPE),
                    new SqlParameter("@COMPONENT_TYPE",model.COMPONENT_TYPE),
                    new SqlParameter("@COMPONENT_SUB_TYPE",model.COMPONENT_SUB_TYPE),
                    new SqlParameter("@COMPONENT_NAME",model.COMPONENT_NAME),
                    new SqlParameter("@COMPONENT_DATATYPE",model.COMPONENT_DATATYPE),
                    new SqlParameter("@COMPONENT_DISPLAY_NAME",model.COMPONENT_DISPLAY_NAME),
                    new SqlParameter("@COMPONENT_DESCRIPTION",model.COMPONENT_DESCRIPTION),
                     new SqlParameter("@Component_TableName",model.Component_TableName),
                    new SqlParameter("@Component_FieldName",model.Component_FieldName),
                    new SqlParameter("@MIN_LENGTH",model.MIN_LENGTH),
                     new SqlParameter("@MAX_LENGTH",model.MAX_LENGTH),
                    new SqlParameter("@MANDATORY",model.MANDATORY),
                    new SqlParameter("@CREATED_BY",model.CreatedBy),
                    new SqlParameter("@ISACT",model.IsActive),
                    new SqlParameter("@EXTRA_INPUT_VALIDATION",model.Extra_Validation),
                    new SqlParameter("@EXTRA_RG_EXPRESSION",model.Expression)
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateComponent2612New", CommandType.StoredProcedure, parameters));
                Response.operation opration = Response.operation.ADD;
                if (model.COMPONENTID > 0)
                    opration = Response.operation.Update;
                res = res.GetResponse(opration, "Component", result, "Component Name");
                if (res.IsSuccess && model.COMPONENTID == 0)
                {
                  res.Message=  "Congratulation!!!. Component Submited successfully.";
                }
                if (result == -1)
                {
                    res.Message = "Failed!!! Component Name must be unique.";
                    res.IsSuccess = false;
                }
                else if (result == -2)
                {
                    res.Message = "Failed!!! Table Name ,Column Name must be unique.";
                    res.IsSuccess = false;
                }
                else if (result == -3)
                {
                    res.Message = "Component Name is predefined keyword you can't ADD or Update.";
                    res.IsSuccess = false;
                }
                else if (result == -4)
                {
                    res.Message = "Failed!!! Component Display Name must be unique.";
                    res.IsSuccess = false;
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response AddComponent(GlobalComponentModel Model)
        {
            string ComponentName = Model.COMPONENT_NAME;
            string ComponentDisPlayName = Model.COMPONENT_DISPLAY_NAME;
            Response res = new Response();
            try
            {
                List<string> ComponentListName = new List<string>();

                if (Model.COMPONENT_FILE_TYPE.ToUpper() == "PAYMAST" && Model.COMPONENT_TYPE.ToUpper() == "EARNING" && Model.COMPONENT_SUB_TYPE.ToUpper() == "RECURRING")
                {
                    ComponentListName.Add("ERN_");
                    ComponentListName.Add("ARR_");
                    ComponentListName.Add("PRJ_");
                    ComponentListName.Add("TAX_");
                    ComponentListName.Add("YTD_");
                }
                else if (Model.COMPONENT_FILE_TYPE.ToUpper() == "PAYMAST" && Model.COMPONENT_TYPE.ToUpper() == "EARNING" && Model.COMPONENT_SUB_TYPE.ToUpper() == "ONETIME")
                {
                    ComponentListName.Add("YTD_");
                    ComponentListName.Add("TAX_");
                }
                else
                {
                    ComponentListName.Add(Model.COMPONENT_NAME);
                }
                
                foreach (var item in ComponentListName)
                {
                    if (ComponentListName.Count > 1)
                    {
                        Model.COMPONENT_NAME = item + ComponentName;
                        Model.COMPONENT_DISPLAY_NAME = item + ComponentDisPlayName;
                    }
                    res = AddUpdateComponent(Model);
                }
                return res;
            }
            catch
            {
                throw;
            }
        }
        
        public Response DeleteGlobalComponent(int GlobalCompID)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@GlobalCompID",GlobalCompID),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("DeleteGlobalComponent", CommandType.StoredProcedure, parameters));
                if (result == -1)
                {
                    res.IsSuccess = false;
                    res.Message = "Global Component Delete First.";
                }
                if (result > 0)
                {
                    res.IsSuccess = true;
                    res.Message = "Component Deleted Successfully.";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetComponent(GlobalComponentModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ComponentID",model.COMPONENTID),
                    new SqlParameter("@CompFileType",model.COMPONENT_FILE_TYPE),
                    new SqlParameter("@ISAct",model.IsActive)
                };

                return DataLib.ExecuteDataTable("GetComponent", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }


        public DataTable GetComponentFileType(GlobalComponentModel model,string DataType)
        {
            DataTable dt = new DataTable();
            try
            {
                string COMPONENT_FILE_TYPE = model.COMPONENT_FILE_TYPE == "0" ? null : model.COMPONENT_FILE_TYPE;
               string COMPONENT_TYPE = model.COMPONENT_TYPE == "0" ? null : model.COMPONENT_TYPE;
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Component_File_Type",COMPONENT_FILE_TYPE),
                    new SqlParameter("@Component_Type",COMPONENT_TYPE),
                    new SqlParameter("@COMPONENT_DATATYPE",DataType),
                };

                return DataLib.ExecuteDataTable("GetComponentFileType", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetComponentFileType(string COMPONENT_FILE_TYPE, string COMPONENT_TYPE, string COMPONENT_SUB_TYPE, string COMPONENT_DATATYPE,string TableName)
        {
            DataTable dt = new DataTable();
            try
            {
                //string COMPONENT_FILE_TYPE = model.COMPONENT_FILE_TYPE == "0" ? null : model.COMPONENT_FILE_TYPE;
                //string COMPONENT_TYPE = model.COMPONENT_TYPE == "0" ? null : model.COMPONENT_TYPE;
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Component_File_Type",COMPONENT_FILE_TYPE),
                    new SqlParameter("@Component_Type",COMPONENT_TYPE),
                    new SqlParameter("@COMPONENT_SUB_TYPE",COMPONENT_SUB_TYPE),
                    new SqlParameter("@COMPONENT_DATATYPE",COMPONENT_DATATYPE),
                    new SqlParameter("@UTableName",TableName),
                };

                return DataLib.ExecuteDataTable("GetComponentFileTypeupload", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetComponentTableName( string TableName,string FieldName = "")
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@TableName",TableName),
                     new SqlParameter("@FieldName",FieldName),
                };

                return DataLib.ExecuteDataTable("GetGComponentTableName", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }


        public DataTable GetComponentHistory(GlobalComponentModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@COMPONENT_ID",model.COMPONENTID ),
                };
                return DataLib.ExecuteDataTable("GetComponentHis", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }





        public string UploadGlobalComponentDetails(string FilePath, string FileExtention, string SampleFilePath, int CreatedBy, ref int SuccessCount, ref int FailCount)
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

                if (!CheckColumnFormatGComponent(FilePath, FileExtention, SampleFilePath))
                {
                    return "Invalid File Format";
                }


                if (dt.Rows.Count < 1)
                {
                    return "No Data found for upload.";
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
                GlobalComponentModel Model = new GlobalComponentModel();
                GlobalComponentVM GlobalVM;
                FileComponentModel FileM = new FileComponentModel();
                FileSetupRepo FRepo = new FileSetupRepo();
                string strerr = "";

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //Only checking Required validation using View Model
                    try
                    {
                        GlobalVM = new GlobalComponentVM();
                        string Max = dt.Rows[i]["MAX_LENGTH"] == null || Convert.ToString(dt.Rows[i]["MAX_LENGTH"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["MAX_LENGTH"]);
                        string Min = dt.Rows[i]["MIN_LENGTH"] == null || Convert.ToString(dt.Rows[i]["MIN_LENGTH"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["MIN_LENGTH"]);
                        //string COMPONENTID = dt.Rows[i]["TID"] == null || Convert.ToString(dt.Rows[i]["TID"]) == "" ? "0" : Convert.ToString(dt.Rows[i]["TID"]);
                        GlobalVM.COMPONENTID = 0;//Convert.ToInt32(COMPONENTID.Replace("\"", ""));
                        GlobalVM.COMPONENT_FILE_TYPE = Convert.ToString(dt.Rows[i]["COMPONENT_FILE_TYPE"]).Replace("\"", "").Trim();
                        GlobalVM.COMPONENT_TYPE = Convert.ToString(dt.Rows[i]["COMPONENT_TYPE"]).Replace("\"", "").Trim();
                        GlobalVM.COMPONENT_SUB_TYPE = Convert.ToString(dt.Rows[i]["COMPONENT_SUB_TYPE"]).Replace("\"", "").Trim();
                        GlobalVM.COMPONENT_NAME = Convert.ToString(dt.Rows[i]["COMPONENT_NAME"]).Replace("\"", "").Trim();
                        GlobalVM.COMPONENT_DATATYPE = Convert.ToString(dt.Rows[i]["COMPONENT_DATATYPE"]).Replace("\"", "").Trim();
                        GlobalVM.COMPONENT_DISPLAY_NAME = Convert.ToString(dt.Rows[i]["COMPONENT_DISPLAY_NAME"]).Replace("\"", "").Trim();
                        GlobalVM.COMPONENT_DESCRIPTION = Convert.ToString(dt.Rows[i]["COMPONENT_DESCRIPTION"]).Replace("\"", "").Trim();
                        GlobalVM.MIN_LENGTH = Min.Replace("\"", "").Trim();
                        GlobalVM.MAX_LENGTH = Max.Replace("\"", "").Trim();
                        GlobalVM.MANDATORY = Convert.ToBoolean(dt.Rows[i]["MANDATORY"].ToString().Replace("\"", "") == "1" ? true : false);
                        GlobalVM.IsActive = Convert.ToBoolean(dt.Rows[i]["STATUS"].ToString().Replace("\"", "") == "1" ? true : false);
                        GlobalVM.Expression = Convert.ToString(dt.Rows[i]["EXTRA_RG_EXPRESSION"]).Replace("\"", "").Trim();
                        GlobalVM.Extra_Validation = Convert.ToString(dt.Rows[i]["EXTRA_INPUT_VALIDATION"]).Replace("\"", "").Trim();
                        GlobalVM.Component_FieldName = Convert.ToString(dt.Rows[i]["COMPONENT_FIELD_NAME"]).Replace("\"", "").Trim();
                        GlobalVM.Component_TableName = Convert.ToString(dt.Rows[i]["COMPONENT_TABLE_NAME"]).Replace("\"", "").Trim();
                        GlobalVM.CreatedBy = CreatedBy;
                        var results = new List<ValidationResult>();
                        var vc = new ValidationContext(GlobalVM, null, null);
                        var isValid = Validator.TryValidateObject(GlobalVM, vc, results, true);
                        var errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                        strerr = string.Join(" ", errors);
                        if (isValid)
                        {
                            string s = GlobalVM.COMPONENT_FILE_TYPE.ToUpper();
                            //Checking File Type 
                            if (!string.IsNullOrEmpty(GlobalVM.COMPONENT_FILE_TYPE))
                            {

                                if (GlobalVM.COMPONENT_FILE_TYPE.ToUpper() == "HRDMAST" || GlobalVM.COMPONENT_FILE_TYPE.ToUpper() == "PAYMAST")
                                {
                                    Model.COMPONENT_FILE_TYPE = GlobalVM.COMPONENT_FILE_TYPE.ToUpper();
                                }
                                else
                                {
                                    Model.COMPONENT_FILE_TYPE = null;
                                }
                            }
                            else
                            {
                                Model.COMPONENT_FILE_TYPE = null;
                            }

                            //Getting Component Datatype from DataBase
                            if (!string.IsNullOrEmpty(GlobalVM.COMPONENT_DATATYPE))
                            {
                                DataTable dtDType = (GetComponentFileType("", "", "", GlobalVM.COMPONENT_DATATYPE, ""));
                                if (dtDType.Rows.Count > 0)
                                {
                                    Model.COMPONENT_DATATYPE = Convert.ToString(dtDType.Rows[0]["COMPONENT_DATATYPE"].ToString().Replace("\"", ""));
                                }
                                else
                                {
                                    Model.COMPONENT_DATATYPE = null;
                                }
                            }
                            else
                            {
                                Model.COMPONENT_DATATYPE = null;
                            }
                            //Getting Table Name from database
                            if (!string.IsNullOrEmpty(GlobalVM.Component_TableName))
                            {
                                DataTable dtTableName = GetComponentFileType("", "", "", "", GlobalVM.Component_TableName);
                                if (dtTableName.Rows.Count > 0)
                                {
                                    Model.Component_TableName = Convert.ToString(dtTableName.Rows[0]["TABLE_NAME"].ToString().Replace("\"", ""));
                                }
                                else
                                {
                                    Model.Component_TableName = null;
                                }
                            }
                            else
                            {
                                Model.Component_TableName = null;
                            }


                            //Getting Extra Validation from database
                            if (!string.IsNullOrEmpty(GlobalVM.Extra_Validation))
                            {

                                FileM.Extra_Validation = GlobalVM.Extra_Validation;

                                DataTable dtValidation = (FRepo.GetValidation(FileM));
                                if (dtValidation.Rows.Count > 0)
                                {
                                    Model.Extra_Validation = Convert.ToString(dtValidation.Rows[0]["VALIDATION_FIELD"].ToString().Replace("\"", ""));
                                }
                                else
                                {
                                    Model.Extra_Validation = null;
                                }
                            }
                            else
                            {
                                Model.Extra_Validation = null;
                            }

                            //Getting Component Type from database
                            if (!string.IsNullOrEmpty(GlobalVM.COMPONENT_TYPE))
                            {
                                DataTable objDTCtype = new DataTable();
                                if (!string.IsNullOrEmpty(GlobalVM.COMPONENT_FILE_TYPE))
                                {
                                    objDTCtype = GetComponentFileType(GlobalVM.COMPONENT_FILE_TYPE, GlobalVM.COMPONENT_TYPE, "", "", "") ;
                                    Model.COMPONENT_TYPE = objDTCtype.Rows.Count > 0 ? Convert.ToString(objDTCtype.Rows[0]["COMPONENT_TYPE"]) : null;
                                }
                                else
                                {
                                    Model.COMPONENT_TYPE = null;
                                }

                            }
                            else
                            {
                                Model.COMPONENT_TYPE = null;
                            }

                            //Getting Component Sub Type from database
                            if (!string.IsNullOrEmpty(GlobalVM.COMPONENT_SUB_TYPE))
                            {
                                DataTable objDTCStype = new DataTable();
                                if (!string.IsNullOrEmpty(GlobalVM.COMPONENT_FILE_TYPE) && !string.IsNullOrEmpty(GlobalVM.COMPONENT_TYPE))
                                {
                                    objDTCStype = GetComponentFileType(GlobalVM.COMPONENT_FILE_TYPE, GlobalVM.COMPONENT_TYPE, GlobalVM.COMPONENT_SUB_TYPE, "", "");
                                    Model.COMPONENT_SUB_TYPE = objDTCStype.Rows.Count > 0 ? Convert.ToString(objDTCStype.Rows[0]["COMPONENT_SUB_TYPE"]) : null;
                                }
                                else
                                {
                                    Model.COMPONENT_SUB_TYPE = null;
                                }

                            }
                            else
                            {
                                Model.COMPONENT_SUB_TYPE = null;
                            }


                            //Getting Component Field Name from database
                            if (!string.IsNullOrEmpty(GlobalVM.Component_FieldName))
                            {
                                DataTable FNamedt = new DataTable();
                                if (!string.IsNullOrEmpty(GlobalVM.Component_TableName))
                                {
                                    FNamedt = GetComponentTableName(GlobalVM.Component_TableName, GlobalVM.Component_FieldName);
                                    Model.Component_FieldName = FNamedt.Rows.Count > 0 ? Convert.ToString(FNamedt.Rows[0]["FIELD_NAME"]) : null;
                                }
                                else
                                {
                                    Model.Component_FieldName = null;
                                }

                            }
                            else
                            {
                                Model.Component_FieldName = null;
                            }

                               Model.COMPONENTID= GlobalVM.COMPONENTID;
                            Model.COMPONENT_NAME = GlobalVM.COMPONENT_NAME.Trim();
                            Model.COMPONENT_DISPLAY_NAME = GlobalVM.COMPONENT_DISPLAY_NAME.Trim();
                            Model.COMPONENT_DESCRIPTION = GlobalVM.COMPONENT_DESCRIPTION.Trim();
                            Model.MIN_LENGTH = Convert.ToInt32(GlobalVM.MIN_LENGTH);
                            Model.MAX_LENGTH = Convert.ToInt32(GlobalVM.MAX_LENGTH);
                            Model.MANDATORY = GlobalVM.MANDATORY;
                            Model.IsActive = GlobalVM.IsActive;
                            Model.Expression = GlobalVM.Expression.Trim();
                            Model.CreatedBy = CreatedBy;
                             
                            results = new List<ValidationResult>();
                            vc = new ValidationContext(Model, null, null);
                            isValid = Validator.TryValidateObject(Model, vc, results, true);
                            errors = Array.ConvertAll(results.ToArray(), o => o.ErrorMessage);
                            strerr = string.Join(" ", errors);
                            if (isValid)
                            {
                                ret = AddComponent(Model);

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
                                        dt.Rows[i]["Message"] = ret.Message;
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



        public bool CheckColumnFormatGComponent(string FilePath, string FileExtention, string SampleFilePath)
        {
            bool ret = true;
            DataTable dt = ExcellUtils.ExlToDataTable(SampleFilePath, "XLSX");//(SampleFilePath, ',');
            DataTable dt2 = ExcellUtils.ExlToDataTable(FilePath, FileExtention);

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (!(dt2.Columns.Contains(dt.Columns[i].ColumnName)))
                {
                    ret = false;
                    return ret;
                }
            }
            return ret;
        }

    }
}