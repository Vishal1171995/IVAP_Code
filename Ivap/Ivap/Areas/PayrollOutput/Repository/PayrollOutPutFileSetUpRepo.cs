using Ivap.Areas.Configuration.Repository;
using Ivap.Areas.InputProcessing.Models;
using Ivap.Areas.InputProcessing.Validator;
using Ivap.Areas.PayrollOutput.Models;
using Ivap.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Globalization;
using Ivap.DateConverter;

namespace Ivap.Areas.PayrollOutput.Repository
{
    public class PayrollOutPutFileSetUpRepo
    {
        public DataTable GetFileType(PayrollOutPutFileSetupModel objModel, string WFS_String = "")
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@FileID", objModel.FileID),
                    new SqlParameter("@ENTITY_ID", objModel.EID),
                    new SqlParameter("@FILE_TYPE", objModel.FILE_TYPE),
                    new SqlParameter("@FILE_NAME", objModel.FILE_NAME),
                    new SqlParameter("@IsAct", objModel.IsActive),
                    new SqlParameter("@UID", objModel.CreatedBy),
                    new SqlParameter("@WFS_STRING", WFS_String),
                };
                dt = DataLib.ExecuteDataTable("GetFileType", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetEntity(int EntityID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EntityID", EntityID),
                    new SqlParameter("@IsAct", "1"),
                };
                dt = DataLib.ExecuteDataTable("GetEntity", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch
            {
                throw;
            }
        }
        public PayrollOutputFileResponseModel UploadOutputDetails(PayrollOutputFileUploadModel ObjOutputRequest)
        {

            PayrollOutputFileResponseModel Res = new PayrollOutputFileResponseModel();
            Res.IsSuccess = false;
            Res.Message = "Sorry!!! Unable to process your request. Please try again later.";
            try
            {
                
                // Validating File Extention 
                string[] ArrFileExt = ObjOutputRequest.File_Name.Split('.');
                //At last index of array file extention resides
                if (!(ArrFileExt[ArrFileExt.Length - 1].ToString().ToUpper() == "XLSX" || ArrFileExt[ArrFileExt.Length - 1].ToString().ToUpper() == "XLS"))
                {
                    Res.IsSuccess = false;
                    Res.Message = "Invalid file type!!! File type must be XLS OR XLSX only.";
                    return Res;
                }

                DataTable dt = null;
                try
                {
                    dt =  ExcellUtils.ExlToDataTableNew(ObjOutputRequest.FilePath, ObjOutputRequest.FileExtention);
                }
                catch (Exception Ex)
                {
                    return Res;
                }
                Response ResCheck = CheckColumnFormat(dt, ObjOutputRequest.EID, ObjOutputRequest.FileID);
                if (ResCheck.IsSuccess == false)
                {
                    Res.IsSuccess = false;
                    Res.SuccessCount = 0;
                    Res.Message = ResCheck.Message + "Column missing in excel sheet";
                    return Res;

                }
                InputValidationVM VMValidation = new InputValidationVM();
                InputValidationModel MValidation = new InputValidationModel();
                int EID = ObjOutputRequest.EID;
                VMValidation = ValidateTempDataTable(EID, ObjOutputRequest.FileID, ObjOutputRequest.CreatedBy, dt,PayDate:ObjOutputRequest.PayDate);
                //Now validate data for upload
                if (dt.Rows.Count > 0)
                {
                    if (VMValidation.SuccessCount == VMValidation.TotalCount)
                    {
                        Res = ProceesClearance(dtData: dt, FileID: ObjOutputRequest.FileID, EID: ObjOutputRequest.EID, uid: ObjOutputRequest.CreatedBy);
                    }
                    else
                    {
                        Res.IsSuccess = false;
                        Res.Message = "Error in data validation";
                    }
                }
                else
                {
                    Res.IsSuccess = false;
                    Res.Message = "Excel file does not contain data";
                }

                return Res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private int UploadFileData(DataTable SourceTable, DataTable DestinationTable, DataTable Display_Table, int EntityID, int FileID, string Rtime, string val, int Created_By)
        {
            int SuccessCount = 0;
            SqlConnection con = null;
            SqlBulkCopy Sqltable = null;
            try
            {
                DataView dvSchema = new DataView(Display_Table);
                string ToBeRemovedColumn = "";
                for (int i = 0; i < SourceTable.Columns.Count; i++)
                {
                    // string ColumnName = SourceTable.Rows[i]["COMPONENT_DISPLAY_NAME"].ToString().Trim().Replace(".", "#");

                    try
                    {
                        dvSchema.RowFilter = "COMPONENT_DISPLAY_NAME='" + SourceTable.Columns[i].ColumnName.Trim() + "'";
                        DataTable dtColumns = dvSchema.ToTable();
                        SourceTable.Columns[i].ColumnName = dtColumns.Rows[0]["COMPONENT_NAME"].ToString().Trim().ToUpper();
                        if (SourceTable.Columns[i].ColumnName == "PAY_EMP_CODE")
                            SourceTable.Columns[i].ColumnName = "EMP_CODE";
                    }
                    catch
                    {
                        //Now Collect all Extra columns that is not part of schema but supplyed by the user.
                        ToBeRemovedColumn = ToBeRemovedColumn + "," + SourceTable.Columns[i].ColumnName;
                        //SourceTable.Columns.RemoveAt(i);

                    }
                }
                ToBeRemovedColumn = ToBeRemovedColumn.TrimEnd(',');
                ToBeRemovedColumn = ToBeRemovedColumn.TrimStart(',');
                //Now Remove all Extra columns that is not part of schema but supplyed by the user.
                if (ToBeRemovedColumn != "")
                {
                    var arr = ToBeRemovedColumn.Split(',');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        //int index = Convert.ToInt32(arr[i]);
                        SourceTable.Columns.Remove(arr[i]);
                    }
                }
                con = new SqlConnection(Ivap.Utils.DataLib.GetConnectionString());
                new SqlBulkCopy(con);
                if (SourceTable.Rows.Count > 0)
                {
                    SourceTable.Columns.Add("TEMP_BATCH_ID");
                    SourceTable.Columns.Add("TEMP_BATCH_NO");
                    SourceTable.Columns.Add("FILE_ID");
                    SourceTable.Columns.Add("CREATED_BY");
                    SourceTable.Columns.Add("TEMP_STATUS");
                    foreach (DataRow dr in SourceTable.Rows)
                    {
                        dr["TEMP_BATCH_ID"] = val;
                        dr["TEMP_BATCH_NO"] = Rtime;
                        dr["FILE_ID"] = FileID;
                        dr["CREATED_BY"] = Created_By;
                        dr["TEMP_STATUS"] = "UPLOADED";
                        SuccessCount += 1;
                    }

                    Sqltable = new SqlBulkCopy(con);
                    Sqltable.DestinationTableName = DestinationTable.Rows[1]["TABLE_NAME"].ToString();
                    string str = "";
                    for (int j = 0; j < SourceTable.Columns.Count; j++)
                    {
                        //str = str + "," +"'"+ SourceTable.Columns[j].ColumnName.ToUpper()+"'";
                        Sqltable.ColumnMappings.Add(SourceTable.Columns[j].ColumnName.ToUpper(), SourceTable.Columns[j].ColumnName.ToUpper());

                    }
                    con.Open();
                    Sqltable.WriteToServer(SourceTable);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Sqltable.Close();
                con.Close();
                con.Dispose();
            }
            return SuccessCount;

        }
        public InputValidationVM ValidateTempDataTable(int EntityID, int FileID, int UID, DataTable outPutDataDt,DateTime PayDate)
        {
            InputValidationModel inputValidation = new InputValidationModel();
            InputValidationVM objModel = new InputValidationVM();
            // Get Data Formate From Entity Table
            DataTable DataFormateDt = GetEntity(EntityID);
            string DateFormate = DataFormateDt.Rows[0]["DATE_FORMAT"].ToString();

            //Now Get Schema of Current File
            DataTable SchemaTable = GetSchemaOfInputFile(EntityID, FileID);
            //Now Create Query For getting Data from Temp Table 
            //COMPONENT_NAME
            DataView dvSchema = new DataView(SchemaTable);
            for (int i = 0; i < outPutDataDt.Columns.Count; i++)
            {
                dvSchema.RowFilter = "COMPONENT_DISPLAY_NAME='" + outPutDataDt.Columns[i].ColumnName.Trim() + "'";
                DataTable dtColumns = dvSchema.ToTable();
                outPutDataDt.Columns[i].ColumnName = dtColumns.Rows[0]["COMPONENT_NAME"].ToString().Trim().ToUpper();

            }

            //Now Get List of all master companents
            StringBuilder SbMasterData = new StringBuilder();
            DataView dvMaster = new DataView(SchemaTable);
            dvMaster.RowFilter = "COMPONENT_DATATYPE='MASTER'";
            DataTable dtMasterComponent = dvMaster.ToTable();
            StringBuilder SbTableName = new StringBuilder();
            bool MasterComponentAvailable = false;
            for (int i = 0; i < dtMasterComponent.Rows.Count; i++)
            {
                SbMasterData.Append("select '").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"]).Append("' As Master_Name");
                SbMasterData.Append(", ").Append(dtMasterComponent.Rows[i]["COMPONENT_COLUMN_NAME"]).Append(" As VALUE");
                SbMasterData.Append(", ").Append(" TID").Append(" As TID");
                SbMasterData.Append(" from ").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"]);
                if (dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"].ToString().ToUpper() == "IVAP_MST_STATE")
                {
                    SbMasterData.Append(" WHERE  ISACTIVE=1");
                }
                else
                {
                    SbMasterData.Append(" WHERE ENTITY_ID= ").Append(EntityID).Append(" AND ISACTIVE=1");
                }

                SbTableName.Append("'").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"]).Append("'").Append(",");
                MasterComponentAvailable = true;
                //For avaoiding union all in last select
                if (i + 1 != dtMasterComponent.Rows.Count)
                    SbMasterData.Append(" UNION ALL ");
            }
            SbMasterData.Append(";select UID,TABLE_NAME,TIDS from IVAP_DATA_ACCESS_DTL where ISACTIVE=1 AND UID=" + UID + " and  TABLE_NAME in (")
                .Append(SbTableName.ToString().TrimEnd(',')).Append(")");
            DataSet dsAllMaster = new DataSet();
            if (MasterComponentAvailable == true)
            {
                dsAllMaster = DataLib.ExecuteDataSet(SbMasterData.ToString(), CommandType.Text, null);
            }


            outPutDataDt.Columns.Add("ValidationType");
            outPutDataDt.Columns.Add("Message");

            //Get column Name for and Schema of Table 

            BasicValidationBase ObjVal;
            var ValRes = new ValidationResponse();
            for (int Temp = 0; Temp < outPutDataDt.Rows.Count; Temp++)
            {
                bool IsValidRow = true;
                for (int Schema = 0; Schema < SchemaTable.Rows.Count; Schema++)
                {

                    try
                    {

                        ObjVal = new BasicValidationBase();
                        ObjVal.IsRequired = SchemaTable.Rows[Schema]["MANDATORY"].ToString();
                        ObjVal.Value = Convert.ToString(outPutDataDt.Rows[Temp][SchemaTable.Rows[Schema]["COMPONENT_NAME"].ToString()]).Trim();
                        ObjVal.MinLength = Convert.ToInt32(SchemaTable.Rows[Schema]["MIN_LENGTH"].ToString());
                        ObjVal.MaxLength = Convert.ToInt32(SchemaTable.Rows[Schema]["MAX_LENGTH"].ToString());
                        ObjVal.DateFormat = DateFormate;
                        ObjVal.dsMasterData = dsAllMaster;
                        ObjVal.DataType = SchemaTable.Rows[Schema]["COMPONENT_DATATYPE"].ToString();
                        ObjVal.ColumnName = SchemaTable.Rows[Schema]["COMPONENT_DISPLAY_NAME"].ToString();
                        ObjVal.DisplayName = SchemaTable.Rows[Schema]["COMPONENT_DISPLAY_NAME"].ToString();
                        ObjVal.UID = UID;
                        ObjVal.Table_Name = SchemaTable.Rows[Schema]["COMPONENT_TABLE_NAME"].ToString();

                        //ObjVal.IsRequired = SchemaTable.Rows[Schema]["MANDATORY"].ToString();
                        ValRes = ObjVal.ExecuteBasicValidation(ObjVal);
                        if (ValRes.IsSuccess == false)
                        {
                            IsValidRow = false;
                            break;
                        }
                        if (SchemaTable.Rows[Schema]["COMPONENT_NAME"].ToString().ToUpper() == "PAYDATE") {
                            var currFormatPayDate= DateFormatConverterFactory.Create(ObjVal.DateFormat);
                            //string payDateFormat = DateFormate;
                            //if (payDateFormat.ToString().ToUpper() == "DD/MM/YYYY") {
                            //    payDateFormat = "dd/MM/yyyy";
                            //}
                            if (DateTime.Compare(currFormatPayDate.ConvertDate(ObjVal.Value, ObjVal.DateFormat), PayDate)!=0) {
                                ValRes.IsSuccess = false;
                                ValRes.ValidationErrorType = "PAYDATE";
                                ValRes.Message = "Pay Date should be according to  active month";
                                IsValidRow = false;
                                break;
                            }
                        }

                    }

                    catch
                    {

                    }

                }
                //TempTable.Rows[Temp]["CkStatus"] = ValRes.ValidationErrorType;
                if (IsValidRow == true)
                    outPutDataDt.Rows[Temp]["ValidationType"] = "SUCCESS";
                else
                    outPutDataDt.Rows[Temp]["ValidationType"] = ValRes.ValidationErrorType;
                outPutDataDt.Rows[Temp]["Message"] = ValRes.Message;


            }

            //DataTable SchemaTableConvert = SchemaTable;
            DataView dvSchema1 = new DataView(SchemaTable);
            for (int i = 0; i < outPutDataDt.Columns.Count; i++)
            {
                if (outPutDataDt.Columns[i].ColumnName.Trim() == "ValidationType" || outPutDataDt.Columns[i].ColumnName.Trim() == "Message")
                {

                }
                else
                {
                    dvSchema1.RowFilter = "COMPONENT_NAME='" + outPutDataDt.Columns[i].ColumnName.Trim() + "'";
                    DataTable dtColumns = dvSchema1.ToTable();
                    outPutDataDt.Columns[i].ColumnName = dtColumns.Rows[0]["COMPONENT_DISPLAY_NAME"].ToString().Trim().ToUpper();
                }

            }
            HttpContext.Current.Session.Add("OutpuTempTable", outPutDataDt);
            objModel.TotalCount = outPutDataDt.Rows.Count;
            objModel.SuccessCount = outPutDataDt.Select("ValidationType = 'SUCCESS'").Length;
            objModel.ReqValFailCount = outPutDataDt.Select("ValidationType = 'Required Validation'").Length;
            objModel.DataFormateValFailCount = outPutDataDt.Select("ValidationType = 'Data Format Validation'").Length;
            objModel.DateFormateValFailCount = outPutDataDt.Select("ValidationType = 'Date Format Validation'").Length;
            objModel.MasterValFailCount = outPutDataDt.Select("ValidationType = 'Master Valued Validation'").Length;
            objModel.MasterAccessVoilationCount = outPutDataDt.Select("ValidationType = 'Master Data Access Voilation'").Length;
            return objModel;
        }
        public DataTable GETBATCHNO(int EID, int FILE_ID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FILEID",FILE_ID),

                };
                dt = DataLib.ExecuteDataTable("GETBATCHNO", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static DataTable CheckUploadTable(string Temp_Table, string ColumnName = "")
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@TEMP_TABLE", Temp_Table),
                    new SqlParameter("@Column_Name", ColumnName),
                };

                dt = DataLib.ExecuteDataTable("UPLOADTEMPTABLE_NEW", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetSchemaOfInputFile(int EID, int FILE_ID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FILE_ID",FILE_ID),

                };
                dt = DataLib.ExecuteDataTable("GET_FILE_SETUP_UPLOAD", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private Response CheckColumnFormat(DataTable dtInputFile, int EID, int FileID)
        {
            Response ret = new Response();
            DataSet ds = new DataSet();
            try
            {
                DataTable DtSchema = new DataTable();
                // DataTable DtUpload = new DataTable();              
                FileSetupRepo objFileSetupRepo = new FileSetupRepo();
                DtSchema = objFileSetupRepo.FileSetupSample(EID, FileID);

                //DataTable dtInputFile = ExcellUtils.ExlToDataTable(FilePath, "XLSX");
                ret.IsSuccess = true;
                for (int i = 0; i < dtInputFile.Columns.Count; i++)
                {
                    dtInputFile.Columns[dtInputFile.Columns[i].ToString()].ColumnName = dtInputFile.Columns[i].ToString().Trim();
                }

                for (int i = 0; i < DtSchema.Rows.Count; i++)
                {

                    string ColumnName = DtSchema.Rows[i]["COMPONENT_DISPLAY_NAME"].ToString().Trim().Replace(".", "#");
                    if (!(dtInputFile.Columns.Contains(ColumnName)))
                    {

                        ret.Message += DtSchema.Rows[i]["COMPONENT_DISPLAY_NAME"].ToString() + ", ";
                        ret.IsSuccess = false;
                    }

                }

            }
            catch (Exception ex) { throw; }

            return ret;
        }

        public PayrollOutputFileResponseModel ProceesClearance(DataTable dtData, int FileID, int EID, int uid)
        {
            PayrollOutputFileResponseModel Res = new PayrollOutputFileResponseModel();
            SqlConnection con = null;
            SqlBulkCopy Sqltable = null;
            try
            {
                dtData.Columns.Add("CREATED_BY");
                dtData.Columns.Add("FILE_ID");
                dtData.Columns.Add("WF_STATUS");
                dtData.Columns.Add("CURR_USER");
                dtData.Columns.Add("CURR_STATUS");
                foreach (DataRow dr in dtData.Rows)
                {
                    dr["FILE_ID"] = FileID;
                    dr["CREATED_BY"] = uid;
                    dr["CURR_USER"] = uid;
                    dr["CURR_STATUS"] = "UPLOADED";
                    dr["WF_STATUS"] = "UPLOADED";
                }

                //First of all Get Schema of all table
                DataTable dtSchema = GetSchemaOfInputFile(EID, FileID);

                //Now replace Content Display Name to Content Name
                DataView dvColumns;
                for (int i = 0; i < dtData.Columns.Count; i++)
                {
                    try
                    {
                        dvColumns = new DataView(dtSchema);
                        dvColumns.RowFilter = "COMPONENT_DISPLAY_NAME='" + dtData.Columns[i].ColumnName.Trim() + "'";
                        DataTable dtColumns = dvColumns.ToTable();
                        dtData.Columns[i].ColumnName = dtColumns.Rows[0]["COMPONENT_NAME"].ToString().Trim();
                    }
                    catch
                    { }
                }
                //Ivap_PAY_MAST_30,Ivap_HRD_MAST_30
                //Now Replace Master valued with TIDs

                StringBuilder SbMasterData = new StringBuilder();
                DataView dvMaster = new DataView(dtSchema);
                dvMaster.RowFilter = "COMPONENT_DATATYPE='MASTER'";
                DataTable dtMasterComponent = dvMaster.ToTable();
                StringBuilder SbTableName = new StringBuilder();
                bool MasterComponentAvailable = false;
                for (int i = 0; i < dtMasterComponent.Rows.Count; i++)
                {
                    SbMasterData.Append("select '").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"]).Append("' As Master_Name");
                    SbMasterData.Append(", ").Append(dtMasterComponent.Rows[i]["COMPONENT_COLUMN_NAME"]).Append(" As VALUE");
                    SbMasterData.Append(", ").Append(" TID").Append(" As TID");
                    SbMasterData.Append(" from ").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"]);
                    if (dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"].ToString().Trim().ToUpper() == "IVAP_MST_STATE")
                    {
                        SbMasterData.Append(" WHERE  ISACTIVE=1");
                    }
                    else
                    {
                        SbMasterData.Append(" WHERE ENTITY_ID= ").Append(EID).Append(" AND ISACTIVE=1");
                    }
                    SbTableName.Append("'").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"]).Append("'").Append(",");
                    MasterComponentAvailable = true;
                    //For avaoiding union all in last select
                    if (i + 1 != dtMasterComponent.Rows.Count)
                        SbMasterData.Append(" UNION ALL ");
                }
                // we will check access rights for master data for specific user and entity
                DataSet dsAllMaster = new DataSet();
                if (MasterComponentAvailable == true)
                {
                    dsAllMaster = DataLib.ExecuteDataSet(SbMasterData.ToString(), CommandType.Text, null);
                }

                for (int Temp = 0; Temp < dtData.Rows.Count; Temp++)
                {
                    for (int Schema = 0; Schema < dtSchema.Rows.Count; Schema++)
                    {

                        try
                        {

                            var Value = Convert.ToString(dtData.Rows[Temp][dtSchema.Rows[Schema]["COMPONENT_NAME"].ToString()]).Trim();
                            var DataType = dtSchema.Rows[Schema]["COMPONENT_DATATYPE"].ToString().ToUpper();
                            var ColumnName = dtSchema.Rows[Schema]["COMPONENT_DISPLAY_NAME"].ToString();
                            var DisplayName = dtSchema.Rows[Schema]["COMPONENT_DISPLAY_NAME"].ToString();
                            var Table_Name = dtSchema.Rows[Schema]["COMPONENT_TABLE_NAME"].ToString();

                            if (Value.Trim() != "" && DataType == "MASTER")
                            {
                                DataView dv = new DataView(dsAllMaster.Tables[0]);
                                dv.RowFilter = "Master_Name='" + Table_Name + "' AND Value='" + Value + "'";
                                DataTable dt = dv.ToTable();
                                dtData.Rows[Temp][dtSchema.Rows[Schema]["COMPONENT_NAME"].ToString()] = dt.Rows[0]["TID"].ToString();
                            }

                        }

                        catch
                        {

                        }

                    }
                }
                //Now Find Distinct EMPCODE,PAYDATE,FILEID
                DataView dvFilteredRow = new DataView(dtData);
                DataTable dtDistinct = dvFilteredRow.ToTable(true, "EMP_CODE", "PayDate");

                ArrayList arEMP = new ArrayList();
                ArrayList arPayDate = new ArrayList();
                foreach (DataRow dr in dtDistinct.Rows) {
                    arEMP.Add("'"+dr["EMP_CODE"]+"'");
                    arPayDate.Add("'"+dr["PayDate"]+"'");
                }
                DataLib.ExecuteNonQuery("delete from  Ivap_TEMP_HIS_"+EID+" where FILE_ID=" + FileID + " and EMP_CODE in ("+string.Join(",",arEMP.ToArray())+ ") and convert(varchar,paydate,103) in(" + string.Join(",",arPayDate.ToArray())+")" ,CommandType.Text,null);
                //String result = dtDistinct.AsEnumerable()
                //     .Select(row => row["EMP_CODE"].ToString())
                //     .Aggregate((s1, s2) => String.Concat("EMP_CODE='"+s1+"'", " And PAYdate=convert(varchar,'" + s2+"',106)"));
                //if (result.ToString() != "") { 
                //DataTable dtDistinctDataBase = DataLib.ExecuteDataTable(" delete from Ivap_TEMP_HIS_31 where FILE_ID=" + FileID+ " and ", CommandType.Text,null);
                //}
                //Now insert all data

                string HRDMastTableName = "Ivap_TEMP_HIS_" + EID;
                con = new SqlConnection(Ivap.Utils.DataLib.GetConnectionString());
                Sqltable = new SqlBulkCopy(con);
                Sqltable.DestinationTableName = HRDMastTableName;
               
                con.Open();
                if (dtSchema.Rows.Count > 0)
                {
                    for (int j = 0; j < dtSchema.Rows.Count; j++)
                    {
                        //str = str + "," +"'"+ SourceTable.Columns[j].ColumnName.ToUpper()+"'";
                        Sqltable.ColumnMappings.Add(dtSchema.Rows[j]["COMPONENT_NAME"].ToString().ToUpper(), dtSchema.Rows[j]["COMPONENT_NAME"].ToString().ToUpper());

                    }
                    Sqltable.ColumnMappings.Add("CREATED_BY", "CREATED_BY");
                    Sqltable.ColumnMappings.Add("FILE_ID", "FILE_ID");
                    Sqltable.ColumnMappings.Add("WF_STATUS", "WF_STATUS");
                    Sqltable.ColumnMappings.Add("CREATED_BY", "MYND_MAKER");
                    Sqltable.ColumnMappings.Add("CURR_USER", "CURR_USER");
                    Sqltable.ColumnMappings.Add("CURR_STATUS", "CURR_STATUS");
                    Sqltable.WriteToServer(dtData);
                }

                //Filtering Companent of HRDMASTER



                //DataView dvSchema = new DataView(dtSchema);
                //dvSchema.RowFilter = "COMPONENT_FILE_TYPE='HRDMAST'";
                //DataTable dtSchemaHRD = dvSchema.ToTable();

                ////Now add check Emp_Code exist then Update else create new string
                //DataTable dtHRDInsert = dtData.Clone();


                //foreach (DataRow drHRD in dtData.Rows)
                //{
                //    string EMP_CODE = Convert.ToString(drHRD["EMP_CODE"]);
                //    if (CheckHRDEMP(EID: EID, EMP_CODE: EMP_CODE, File_ID: FileID))
                //    {//Writre Update query
                //        StringBuilder updateHRDMAST = new StringBuilder();

                //        ArrayList objHRDList = new ArrayList();
                //        updateHRDMAST.Append("Update Ivap_TEMP_HIS_" + Convert.ToString(EID) + " set ");
                //        foreach (DataColumn dcHRD in dtData.Columns)
                //        {
                //            foreach (DataRow drScheemaHRD in dtSchemaHRD.Rows)
                //            {
                //                if (Convert.ToString(drScheemaHRD["COMPONENT_NAME"]).Trim().ToUpper() == dcHRD.ColumnName.ToString().Trim().ToUpper())
                //                {
                //                    if (drScheemaHRD["Component_DataType"].ToString().Trim().ToUpper() == "DATETIME")
                //                    {
                //                        if (Convert.ToString(drHRD[dcHRD]) != "")
                //                        {
                //                            objHRDList.Add("" + dcHRD.ColumnName + " = convert(datetime,'" + drHRD[dcHRD] + "',103) ");
                //                        }
                //                        else
                //                        {
                //                            objHRDList.Add("" + dcHRD.ColumnName + " = Null ");
                //                        }
                //                    }
                //                    else if (drScheemaHRD["Component_DataType"].ToString().Trim().ToUpper() == "NUMBER" || drScheemaHRD["Component_DataType"].ToString().Trim().ToUpper() == "AMOUNT" || drScheemaHRD["Component_DataType"].ToString().Trim().ToUpper() == "MASTER")
                //                    {
                //                        if (Convert.ToString(drHRD[dcHRD]) == "")
                //                        {
                //                            objHRDList.Add("" + dcHRD.ColumnName + " = Null ");
                //                        }
                //                    }
                //                    else
                //                    {
                //                        objHRDList.Add("" + dcHRD.ColumnName + " = '" + drHRD[dcHRD] + "'");
                //                    }
                //                }
                //            }
                //        }
                //        updateHRDMAST.Append(string.Join(",", objHRDList.ToArray()));
                //        updateHRDMAST.Append(" ,MYND_MAKER=" + uid + " , CURR_USER=" + uid + " ,CURR_STATUS='UPLOADED' Where EMP_CODE='" + EMP_CODE + "' and file_id=" + FileID + ";");
                //        ExecuteQuery(updateHRDMAST.ToString());
                //    }
                //    else
                //    {
                //        //Make data table to insert bulk copy
                //        dtHRDInsert.ImportRow(drHRD);
                //    }
                //}
                //string HRDMastTableName = "Ivap_TEMP_HIS_" + EID;
                //con = new SqlConnection(Ivap.Utils.DataLib.GetConnectionString());
                //Sqltable = new SqlBulkCopy(con);
                //Sqltable.DestinationTableName = HRDMastTableName;
                //string str = "";
                //con.Open();
                //if (dtSchemaHRD.Rows.Count > 0)
                //{
                //    for (int j = 0; j < dtSchemaHRD.Rows.Count; j++)
                //    {
                //        //str = str + "," +"'"+ SourceTable.Columns[j].ColumnName.ToUpper()+"'";
                //        Sqltable.ColumnMappings.Add(dtSchemaHRD.Rows[j]["COMPONENT_NAME"].ToString().ToUpper(), dtSchemaHRD.Rows[j]["COMPONENT_NAME"].ToString().ToUpper());

                //    }
                //    Sqltable.ColumnMappings.Add("CREATED_BY", "CREATED_BY");
                //    Sqltable.ColumnMappings.Add("FILE_ID", "FILE_ID");
                //    Sqltable.ColumnMappings.Add("WF_STATUS", "WF_STATUS");
                //    Sqltable.ColumnMappings.Add("CREATED_BY", "MYND_MAKER");
                //    Sqltable.ColumnMappings.Add("CURR_USER", "CURR_USER");
                //    Sqltable.ColumnMappings.Add("CURR_STATUS", "CURR_STATUS");
                //    Sqltable.WriteToServer(dtHRDInsert);
                //}
                ////Now add check Emp_Code exist then Update else create new string



                ////Filtering Companent of PAYMASTER
                //dvSchema = new DataView(dtSchema);
                //dvSchema.RowFilter = "COMPONENT_FILE_TYPE='PAYMAST'";
                //DataTable dtSchemaPAY = dvSchema.ToTable();

                ////Now add check Emp_Code exist then Update else create new string
                //DataTable dtPAYInsert = dtData.Clone();


                //foreach (DataRow drPAY in dtData.Rows)
                //{
                //    string EMP_CODE = Convert.ToString(drPAY["EMP_CODE"]);
                //    //if (CheckPAYEMP(EID: EID, EMP_CODE: EMP_CODE))
                //    if (CheckHRDEMP(EID: EID, EMP_CODE: EMP_CODE, File_ID: FileID))
                //    {//Writre Update query
                //        StringBuilder updatePAYMAST = new StringBuilder();

                //        ArrayList objPAYList = new ArrayList();
                //        updatePAYMAST.Append("Update Ivap_TEMP_HIS_" + Convert.ToString(EID) + " set ");
                //        foreach (DataColumn dcPAY in dtData.Columns)
                //        {
                //            foreach (DataRow drScheemaPAY in dtSchemaPAY.Rows)
                //            {
                //                if (Convert.ToString(drScheemaPAY["COMPONENT_NAME"]).Trim().ToUpper() == dcPAY.ColumnName.ToString().Trim().ToUpper())
                //                {
                //                    if (drScheemaPAY["COMPONENT_DATATYPE"].ToString().ToUpper() == "DATETIME")
                //                    {
                //                        if (Convert.ToString(drPAY[dcPAY]) != "")
                //                        {
                //                            objPAYList.Add("" + dcPAY.ColumnName + " = convert(datetime,'" + drPAY[dcPAY] + "',103) ");
                //                        }
                //                        else {
                //                            objPAYList.Add("" + dcPAY.ColumnName + " = Null ");
                //                        }
                //                    }
                //                    else if (drScheemaPAY["COMPONENT_DATATYPE"].ToString().Trim().ToUpper() == "NUMBER" || drScheemaPAY["COMPONENT_DATATYPE"].ToString().Trim().ToUpper() == "AMOUNT" || drScheemaPAY["COMPONENT_DATATYPE"].ToString().Trim().ToUpper() == "MASTER")
                //                    {
                //                        if (Convert.ToString(drPAY[dcPAY]) == "")
                //                        {
                //                            objPAYList.Add("" + dcPAY.ColumnName + " = Null ");
                //                        }
                //                    }
                //                    else
                //                    {
                //                        objPAYList.Add("" + dcPAY.ColumnName + " = '" + drPAY[dcPAY] + "'");
                //                    }
                //                }
                //            }

                //        }
                //        updatePAYMAST.Append(string.Join(",", objPAYList.ToArray()));
                //        updatePAYMAST.Append(" ,MYND_MAKER=" + uid + " , CURR_USER=" + uid + " ,CURR_STATUS='UPLOADED' Where EMP_CODE='" + EMP_CODE + "' and File_ID=" + FileID + ";");
                //        ExecuteQuery(updatePAYMAST.ToString());
                //    }
                //    else
                //    {
                //        //Make data table to insert bulk copy
                //        dtPAYInsert.ImportRow(drPAY);
                //    }
                //}

                //string PayMastTableName = "Ivap_TEMP_HIS_" + EID;
                //Sqltable = new SqlBulkCopy(con);
                //Sqltable.DestinationTableName = PayMastTableName;
                //if (dtSchemaPAY.Rows.Count > 0)
                //{
                //    for (int j = 0; j < dtSchemaPAY.Rows.Count; j++)
                //    {
                //        //str = str + "," +"'"+ SourceTable.Columns[j].ColumnName.ToUpper()+"'";
                //        Sqltable.ColumnMappings.Add(dtSchemaPAY.Rows[j]["COMPONENT_NAME"].ToString().ToUpper(), dtSchemaPAY.Rows[j]["COMPONENT_NAME"].ToString().ToUpper());

                //    }
                //    //if (!dtPAYInsert.Columns.Contains("PAYEMP_CODE"))
                //    //{
                //    //    Sqltable.ColumnMappings.Add("EMP_CODE", "PAY_EMP_CODE");
                //    //}
                //    Sqltable.ColumnMappings.Add("CREATED_BY", "CREATED_BY");
                //    Sqltable.ColumnMappings.Add("FILE_ID", "FILE_ID");
                //    Sqltable.ColumnMappings.Add("WF_STATUS", "WF_STATUS");
                //    Sqltable.ColumnMappings.Add("CREATED_BY", "MYND_MAKER");
                //    Sqltable.ColumnMappings.Add("CURR_USER", "CURR_USER");
                //    Sqltable.ColumnMappings.Add("CURR_STATUS", "CURR_STATUS");
                //    Sqltable.WriteToServer(dtPAYInsert);
                //}
                Res.IsSuccess = true;
                Res.SuccessCount = dtData.Rows.Count;
                return Res;
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetEffectiveDueDateOutputData(string File_IDs, int UID, int Entity_ID,DateTime PayDate)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@File_IDs",File_IDs),
                            new SqlParameter("@UID",UID),
                            new SqlParameter("@EID",Entity_ID),
                            new SqlParameter("@PayDate",PayDate.ToString()),
                        };
                //dt = DataLib.ExecuteDataTable("GetPayrollOutputData_1", CommandType.StoredProcedure, parameters);
                dt = DataLib.ExecuteDataTable("GetEffDateTempTableData_OutPut0", CommandType.StoredProcedure, parameters);

                return dt;
            }
            catch
            {
                throw;
            }
        }


        public bool CheckPAYEMP(int EID, string EMP_CODE)
        {
            bool result = false;
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@EID",EID),
                            new SqlParameter("@EMP_Code",EMP_CODE)
                        };
                int outresult = Convert.ToInt32(DataLib.ExecuteScaler("CheckPAYEMP", CommandType.StoredProcedure, parameters));
                if (outresult == 1) { result = true; }

                return result;
            }
            catch
            {
                throw;
            }
        }
        public bool CheckHRDEMP(int EID, string EMP_CODE, int File_ID)
        {
            bool result = false;
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@EID",EID),
                            new SqlParameter("@EMP_Code",EMP_CODE),
                            new SqlParameter("@File_ID",File_ID),
                        };
                int outresult = Convert.ToInt32(DataLib.ExecuteScaler("CheckHRDEMP_2", CommandType.StoredProcedure, parameters));
                if (outresult == 1) { result = true; }

                return result;
            }
            catch
            {
                throw;
            }
        }

        public void ExecuteQuery(string query)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@qry",query)

                        };
                DataLib.ExecuteScaler("ExecuteQuery", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetPayrollOutPutTable(string PayDate, int File_ID, int UID, int EID)
        {
            DataTable dt;
            dt = GetOutputFileReportDownload(EID: EID, File_ID: File_ID, PayDate: PayDate);
            return dt;
        }

        public Response DeletePayrollOutput(string PayDate, int File_ID,int UID, int EID) {
            Response res= new Response();
            try
            {
                res.IsSuccess = false;
                DataLib.ExecuteNonQuery("delete from ivap_temp_his_"+EID+ " where Upper(Replace(convert(varchar,PAYDATE,106),' ','-'))  ='" + PayDate.ToString() + "' and file_id="+File_ID, CommandType.Text, null);
                res.IsSuccess = true;
                res.Message = "Payroll Output file sucessfully deleted !";
                return res;
            }
            catch {
                throw;
            }
        }
        public DataTable GetOutputFileReportDownload(int EID, int File_ID, string PayDate)
        {
            DataTable dtSchema = GetSchemaOfInputFile(EID, File_ID);

            //Now Replace Master valued with TIDs
            bool MasterComponentAvailable = false;
            StringBuilder sqlQuery = new StringBuilder();
            sqlQuery.Append("select ");
            foreach (DataRow drSchema in dtSchema.Rows)
            {
                if (drSchema["COMPONENT_DATATYPE"].ToString().ToUpper() == "MASTER")
                {
                    sqlQuery.Append(drSchema["COMPONENT_TABLE_NAME"]).Append(".").Append(drSchema["COMPONENT_COLUMN_NAME"])
                                    .Append(" as ").Append("[" + drSchema["COMPONENT_DISPLAY_NAME"] + "]").Append(",");
                }
                else if (drSchema["COMPONENT_DATATYPE"].ToString().ToUpper() == "DATETIME")
                {
                    sqlQuery.Append(" convert(varchar,ivap_temp_his_" + EID + "." + drSchema["COMPONENT_NAME"] + ", 103)").Append(" as ").Append("[" + drSchema["COMPONENT_DISPLAY_NAME"] + "]").Append(",");
                }
                else
                {
                    sqlQuery.Append(" ivap_temp_his_" + EID).Append(".").Append(drSchema["COMPONENT_NAME"]).Append(" as ").Append("[" + drSchema["COMPONENT_DISPLAY_NAME"] + "]").Append(",");
                }

            }
            string StrMainQry = sqlQuery.ToString().Trim().TrimEnd(',');
            StrMainQry = StrMainQry + " from ivap_temp_his_" + EID;

            StringBuilder SbMasterDataJoin = new StringBuilder();
            DataView dvMaster = new DataView(dtSchema);
            dvMaster.RowFilter = "COMPONENT_DATATYPE='MASTER'";
            DataTable dtMasterComponent = dvMaster.ToTable();
            StringBuilder SbTableName = new StringBuilder();

            for (int i = 0; i < dtMasterComponent.Rows.Count; i++)
            {
                MasterComponentAvailable = true;
                SbTableName.Append(" left join ").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"])
                    .Append(" on ivap_temp_his_" + EID).Append(".").Append(dtMasterComponent.Rows[i]["COMPONENT_NAME"])
                    .Append(" = ").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"]).Append(".TID");
            }
            // we will check access rights for master data for specific user and entity
            DataSet dsAllMaster = new DataSet();
            if (MasterComponentAvailable == true)
            {
                StrMainQry += SbTableName.ToString();
            }
            StrMainQry = StrMainQry + " where Upper(Replace(convert(varchar,ivap_temp_his_" + EID + ".PAYDATE,106),' ','-')) ='" + PayDate.ToString() + "' and ivap_temp_his_" + EID + ".File_ID=" + File_ID;
            DataTable dtData = DataLib.ExecuteDataTable(StrMainQry, CommandType.Text, null);
            return dtData;
        }



    }
}