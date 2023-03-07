using Ivap.Areas.Configuration.Repository;
using Ivap.Areas.InputProcessing.Models;
using Ivap.Areas.InputProcessing.Validator;
using Ivap.Areas.Master.Models;
using Ivap.Areas.Master.Repository;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Ivap.Areas.InputProcessing.Repository
{
    public class UploadInputRepo
    {
        public InputFileResponseModel UploadInputDetails(InputFileUploadModel ObjInputRequest)
        {
           
             InputFileResponseModel Res = new InputFileResponseModel();
            Res.IsSuccess = false;
            Res.Message = "Sorry!!! Unable to process your request. Please try again later.";
            try
            {
                // Validating File Extention 
                string[] ArrFileExt = ObjInputRequest.File_Name.Split('.');
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
                    dt = ExcellUtils.ExlToDataTableNew(ObjInputRequest.FilePath,ObjInputRequest.FileExtention);
                }
                catch (Exception Ex)
                {
                    return Res;
                }
                Response ResCheck = CheckColumnFormat(dt, ObjInputRequest.EID, ObjInputRequest.FileID);
                if (ResCheck.IsSuccess==false)
                {
                    Res.IsSuccess = false;
                    Res.SuccessCount = 0;
                    Res.Message = ResCheck.Message +"Column missing in excel sheet";
                    return Res;
                   
                }
              //  DataTable dtInputFile = ExcellUtils.ExlToDataTable(ObjInputRequest.FilePath, ObjInputRequest.FileExtention);
                DataTable dtFileUpload = GetSchemaOfInputFile(ObjInputRequest.EID, ObjInputRequest.FileID);
                DataTable dtPublish = new DataTable();
                string TempTableName = "Ivap_MAST_TEMP_" + ObjInputRequest.EID;
                dtPublish = CheckUploadTable(TempTableName); 
                if (dt.Rows.Count > 0)
                {
                    DataTable dtBatch = GETBATCHNO(ObjInputRequest.EID, ObjInputRequest.FileID);


                    string ObjDateTime = DateTime.Now.ToString("dd/MM/yyyy");
                    string ObjReplaceTime = ObjDateTime.Replace("-", "/");
                    string val;
                    if (dtBatch.Rows[0]["BATCHID"].ToString() == null || dtBatch.Rows[0]["BATCHID"].ToString() =="")
                    {
                        val = "1";
                    }
                    else
                    {
                        int val1 = Convert.ToInt32(dtBatch.Rows[0]["BATCHID"].ToString()) + 1;
                        val = val1.ToString();
                    }
                    ObjReplaceTime += "/" + val;
                    Res.BatchNumber = ObjReplaceTime;

                    if (dtPublish.Columns.Count > 0)
                    {
                        Stopwatch sw;
                        sw = Stopwatch.StartNew();
                        int Success=UploadFileData(dt, dtPublish, dtFileUpload, ObjInputRequest.EID,ObjInputRequest.FileID, ObjReplaceTime, val,ObjInputRequest.CreatedBy);
                        sw.Stop();
                        string TimeCount = Convert.ToString(sw.ElapsedMilliseconds);
                        Res.IsSuccess = true;
                        Res.SuccessCount = Success;
                        Res.Message = "Successfully Uploaded  ";// + TimeCount;

                    }
                    else
                    {
                        Res.IsSuccess = false;
                        Res.Message = "Failed!!!";
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



        private Response CheckColumnFormat(DataTable dtInputFile, int EID,int FileID)
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
                for(int i=0;i< dtInputFile.Columns.Count;i++)
                {
                    dtInputFile.Columns[dtInputFile.Columns[i].ToString()].ColumnName = dtInputFile.Columns[i].ToString().Trim();
                }

                for (int i = 0; i < DtSchema.Rows.Count; i++)
                {

                    string ColumnName = DtSchema.Rows[i]["COMPONENT_DISPLAY_NAME"].ToString().Trim().Replace(".", "#");
                    if (!(dtInputFile.Columns.Contains(ColumnName)))
                    {
                       
                        ret.Message += DtSchema.Rows[i]["COMPONENT_DISPLAY_NAME"].ToString()  +  ", ";
                        ret.IsSuccess = false;
                    }
                   
                }
               
            }
            catch (Exception ex) { throw; }
           
            return ret;
        }

       

        private int UploadFileData(DataTable SourceTable, DataTable DestinationTable,DataTable Display_Table, int EntityID,int FileID,string Rtime,string val,int Created_By)
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
                    for(int i=0;i<arr.Length;i++)
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
            catch(Exception ex)
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
        public  DataTable GetSchemaOfInputFile(int EID, int FILE_ID)
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

        public DataSet GetSchemaAndUserRIghtsOfInputFile(int EID, int FILE_ID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FILE_ID",FILE_ID),

                };
                return DataLib.ExecuteDataSet("GetSchemaAndUserRIghtsOfInputFile", CommandType.StoredProcedure, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public  DataTable GETBATCHNO(int EID, int FILE_ID)
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

        #region ValidationArea

        

        public InputValidationVM ValidateTempDataTable(int EntityID, int FileID, string PayDate,int UID)
        {
            InputValidationModel inputValidation = new InputValidationModel();
            InputValidationVM objModel = new InputValidationVM();
            string TempTableName = "Ivap_MAST_TEMP_" + EntityID;
            // Get Data Formate From Entity Table
            DataTable DataFormateDt = GetEntity(EntityID);
            string DateFormate = DataFormateDt.Rows[0]["DATE_FORMAT"].ToString();

            //Now Get Schema of Current File
            DataTable SchemaTable = GetSchemaOfInputFile(EntityID, FileID);
            
            

            //Now Get List of all master companents
            StringBuilder SbMasterData = new StringBuilder();
            DataView dvMaster = new DataView(SchemaTable);
            dvMaster.RowFilter = "COMPONENT_DATATYPE='MASTER'";
            DataTable dtMasterComponent = dvMaster.ToTable();
            StringBuilder SbTableName = new StringBuilder();
            bool MasterComponentAvailable = false;
            for(int i=0;i<dtMasterComponent.Rows.Count;i++)
            {
                SbMasterData.Append("select '").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"]).Append("' As Master_Name");
                SbMasterData.Append(", ").Append(dtMasterComponent.Rows[i]["COMPONENT_COLUMN_NAME"]).Append(" As VALUE");
                SbMasterData.Append(", ").Append(" TID").Append(" As TID");
                SbMasterData.Append(" from ").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"]);
                string Table_Name = dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"].ToString().Trim().ToUpper();
                if (Table_Name == "IVAP_MST_STATE")
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
                if(i+1!=dtMasterComponent.Rows.Count)
                    SbMasterData.Append(" UNION ALL ");
            }
            //Now Get access control from Data base for checking Data
            SbMasterData.Append(";select UID,TABLE_NAME,TIDS from IVAP_DATA_ACCESS_DTL where ISACTIVE=1 AND UID="+UID+" and  TABLE_NAME in (")
                .Append(SbTableName.ToString().TrimEnd(',')).Append(")");
            DataSet dsAllMaster = new DataSet();
            //If Any Master component available only then Hit database to retrive all masters in one take
            if (MasterComponentAvailable == true)
            {
                 dsAllMaster = DataLib.ExecuteDataSet(SbMasterData.ToString(), CommandType.Text, null);
            }

            //Now Create Query For getting Data from Temp Table 
            StringBuilder Sb = new StringBuilder("select TID,");
            for (int i = 0; i < SchemaTable.Rows.Count; i++)
            {
                Sb.Append(SchemaTable.Rows[i]["COMPONENT_NAME"]).Append(",");
            }

            string TempSelectQry = Sb.ToString().TrimEnd(',') + " from " + TempTableName + " WHERE Created_BY=" + UID + " AND File_ID=" + FileID;
            if (PayDate.Trim().ToUpper() != "ALL" && PayDate.Trim().Length > 0)
                TempSelectQry += " AND PAYDATE='" + PayDate + "'";
            if (string.IsNullOrEmpty(PayDate))
                TempSelectQry += " AND (PAYDATE=''" + " OR PAYDATE is null)";

            DataTable TempTable = DataLib.ExecuteDataTable(TempSelectQry, CommandType.Text, null);

            //Now Adding two different columns for validation type and message

            TempTable.Columns.Add("ValidationType");
            TempTable.Columns.Add("Message");
            
            
            BasicValidationBase ObjVal;
            var ValRes=new ValidationResponse();
            for (int Temp = 0; Temp < TempTable.Rows.Count; Temp++)
            {
                bool IsValidRow = true;
                for (int Schema = 0; Schema < SchemaTable.Rows.Count; Schema++)
                {

                    try
                    {
                        
                        ObjVal = new BasicValidationBase();
                        ObjVal.IsRequired= SchemaTable.Rows[Schema]["MANDATORY"].ToString();
                        ObjVal.Value= Convert.ToString(TempTable.Rows[Temp][SchemaTable.Rows[Schema]["COMPONENT_NAME"].ToString()]).Trim();
                        ObjVal.MinLength= Convert.ToInt32( SchemaTable.Rows[Schema]["MIN_LENGTH"].ToString());
                        ObjVal.MaxLength = Convert.ToInt32(SchemaTable.Rows[Schema]["MAX_LENGTH"].ToString());
                        ObjVal.DateFormat = DateFormate;
                        ObjVal.dsMasterData = dsAllMaster;
                        ObjVal.DataType= SchemaTable.Rows[Schema]["COMPONENT_DATATYPE"].ToString();
                        ObjVal.ColumnName= SchemaTable.Rows[Schema]["COMPONENT_DISPLAY_NAME"].ToString();
                        ObjVal.DisplayName = SchemaTable.Rows[Schema]["COMPONENT_DISPLAY_NAME"].ToString();
                        ObjVal.UID = UID;
                        ObjVal.Table_Name= SchemaTable.Rows[Schema]["COMPONENT_TABLE_NAME"].ToString();

                        ObjVal.Reg_Exp = SchemaTable.Rows[Schema]["EXTRA_RG_EXPRESSION"].ToString();
                        ObjVal.Extra_ValidationType = SchemaTable.Rows[Schema]["EXTRA_INPUT_VALIDATION"].ToString();

                        //ObjVal.IsRequired = SchemaTable.Rows[Schema]["MANDATORY"].ToString(); Extra_ValidationType
                        ValRes = ObjVal.ExecuteBasicValidation(ObjVal);
                        if (ValRes.IsSuccess == false)
                        {
                            IsValidRow = false;
                            break;
                        }
                        
                    }

                    catch
                    {

                    }

                }
                //TempTable.Rows[Temp]["CkStatus"] = ValRes.ValidationErrorType;
                if (IsValidRow == true)
                    TempTable.Rows[Temp]["ValidationType"] = "SUCCESS";
                else
                    TempTable.Rows[Temp]["ValidationType"] = ValRes.ValidationErrorType;
                TempTable.Rows[Temp]["Message"] = ValRes.Message ;
            }
            //Now Find Duplicate based of Paydate and Emp Code 
            StringBuilder SbDupQuery = new StringBuilder();
            String result = TempTable.AsEnumerable()
                    .Where(s => s.Field<String>("ValidationType") == "SUCCESS")
                     .Select(row => row["TID"].ToString())
                     .Aggregate((s1, s2) => String.Concat(s1, "," + s2));


            SbDupQuery.Append("select PAYDATE,EMP_CODE,count(*) As TotalCount from Ivap_MAST_TEMP_"+EntityID+ " where TID IN(" + result + ") group by PAYDATE,EMP_CODE")
                            .Append(" having count(*) > 1" );


            HttpContext.Current.Session.Add("TempTable", TempTable);
            objModel.TotalCount = TempTable.Rows.Count;
            objModel.SuccessCount = TempTable.Select("ValidationType = 'SUCCESS'").Length;
            objModel.ReqValFailCount = TempTable.Select("ValidationType = 'Required Validation'").Length;
            objModel.DataFormateValFailCount = TempTable.Select("ValidationType = 'Data Format Validation'").Length;
            objModel.DateFormateValFailCount = TempTable.Select("ValidationType = 'Date Format Validation'").Length;
            objModel.MasterValFailCount = TempTable.Select("ValidationType = 'Master Valued Validation'").Length;
            objModel.MasterAccessVoilationCount = TempTable.Select("ValidationType = 'Master Data Access Voilation'").Length;
            return objModel;
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
        public DataTable GetTempTable(int FileID, string PayDate, int EID, int UID)
        {

            //Now Get Schema of Current File
            DataTable SchemaTable = GetSchemaOfInputFile(EID, FileID);
            //Now Create Query For getting Data from Temp Table 
            //COMPONENT_NAME
            string TempTable = "Ivap_MAST_TEMP_" + EID;
            StringBuilder Sb = new StringBuilder("select ");
            for (int i = 0; i < SchemaTable.Rows.Count; i++)
            {
                Sb.Append(SchemaTable.Rows[i]["COMPONENT_NAME"]).Append(" As [").Append( SchemaTable.Rows[i]["COMPONENT_DISPLAY_NAME"]).Append("],");
            }
            string TempSelectQry = Sb.ToString().TrimEnd(',') + " from " + TempTable + " WHERE Created_BY=" + UID + " AND File_ID=" + FileID;
            if ((PayDate.Trim().ToUpper() != "ALL") && PayDate.Length>0)
                TempSelectQry += " AND PAYDATE='" + PayDate + "'";

            if (string.IsNullOrEmpty( PayDate))
                TempSelectQry += " AND (PAYDATE=''"+ " OR PAYDATE is null)";

            DataTable dt = DataLib.ExecuteDataTable(TempSelectQry, CommandType.Text, null);
            return dt;
        }
        public int DeleteTempTableData(int FileID, string EffectiveDate, string PayDate, string TempTable,int UserID)
        {
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@File_ID", FileID),
                    new SqlParameter("@Effe_Date", EffectiveDate),
                    new SqlParameter("@Pay_Date", PayDate),
                    new SqlParameter("@TableName", TempTable),
                    new SqlParameter("@UserID",UserID)
                };

                int Result = Convert.ToInt32(DataLib.ExecuteScaler("DeleteTempTableData", CommandType.StoredProcedure, parameters));
                return Result;
            }
            catch
            {
                throw;
            }
        }

        public Response DeleteValidatedData(DataTable dt,int EntityID)
        {
            Response Res = new Response();

            //Now Delete Record from Temp Table
            //string output = string.Empty;
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    output = output + dt.Rows[i]["TID"].ToString();
            //    output += (i < dt.Rows.Count) ? "," : string.Empty;
            //}
            try
            {
                StringBuilder SBTids = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SBTids.Append(dt.Rows[i]["TID"]).Append(",");
                }
                string TempTableName = "Ivap_MAST_TEMP_" + EntityID;

                string StrTids = SBTids.ToString().Trim().TrimEnd(',');
                string StrQry = "Delete from " + TempTableName + " WHERE TID IN (" + StrTids + ");select 1;";
                var DbRes = Convert.ToInt32(DataLib.ExecuteScaler(StrQry, CommandType.Text, null));
                Res.Message = "Data Deleted successfully.";
                Res.IsSuccess = true;
                return Res;
            }
            catch
            {
                throw;
            }
        }

        #endregion Validation Area
        #region Approval
        public int SendToApproval(DataTable dt, int EntityID)
        {
            int result = 0;
            string TempTableName = "Ivap_MAST_TEMP_" + EntityID;
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SqlParameter[] parameters = new SqlParameter[]
                            {
                         new SqlParameter("@TempTableName",TempTableName),
                        new SqlParameter("@TempTID",Convert.ToInt32(dt.Rows[i]["TID"])),
                            };
                    result = Convert.ToInt32(DataLib.ExecuteScaler("ApprovalTempTableData", CommandType.StoredProcedure, parameters));
                }
                return result;
            }
            catch
            {
                throw;
            }
        }
        #endregion Approval


        #region Dueand Effective date
        public DataTable GetAllTempData(int File_ID, int EntityID=0)
        {
            DataTable dt = new DataTable();
             
            try
            {
                    SqlParameter[] parameters = new SqlParameter[]
                            {
                        new SqlParameter("@File_ID",File_ID),
                            };
                    dt = DataLib.ExecuteDataTable("GetEffDatecheck1", CommandType.StoredProcedure, parameters);
                
                return dt;
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetEffectiveDueDateData(int File_ID,int UID,int Entity_ID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@File_ID",File_ID),
                            new SqlParameter("@UID",UID),
                            new SqlParameter("@EID",Entity_ID)
                        };
                dt = DataLib.ExecuteDataTable("GetEffDateTempTableData0", CommandType.StoredProcedure, parameters);

                return dt;
            }
            catch
            {
                throw;
            }
        }
        #endregion Dueand Effective date

        #region TempToTable
        public Response ProceesClearance(DataTable dtData,int FileID,int EID,int uid)
        {
            Response Res = new Response();
            SqlConnection con = null;
            SqlBulkCopy Sqltable = null;
            try
            {
                dtData.Columns.Add("CREATED_BY");
                dtData.Columns.Add("FILE_ID");
                dtData.Columns.Add("WF_STATUS");
                dtData.Columns.Add("CURR_USER");
                dtData.Columns.Add("CURR_STATUS");
                dtData.Columns.Add("CLIENT_MAKER");
                //CLIENT_MAKER
                foreach (DataRow dr in dtData.Rows)
                {
                    dr["FILE_ID"] = FileID;
                    dr["CREATED_BY"] = uid;
                    dr["WF_STATUS"] = "UPLOADED";
                    dr["CURR_STATUS"] = "UPLOADED";
                    dr["CURR_USER"] = uid;
                    dr["CLIENT_MAKER"] = uid;
                }

                //First of all Get Schema of all table
                DataTable dtSchema = GetSchemaOfInputFile(EID, FileID);
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
                    SbMasterData.Append(" WHERE ENTITY_ID= ").Append(EID).Append(" AND ISACTIVE=1");
                    SbTableName.Append("'").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"]).Append("'").Append(",");
                    MasterComponentAvailable = true;
                    //For avaoiding union all in last select
                    if (i + 1 != dtMasterComponent.Rows.Count)
                        SbMasterData.Append(" UNION ALL ");
                }

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
                //Filtering Companent of HRDMASTER
               
                string HRDMastTableName = "Ivap_TEMP_HIS_" + EID;
                con = new SqlConnection(Ivap.Utils.DataLib.GetConnectionString());
                Sqltable = new SqlBulkCopy(con);
                Sqltable.DestinationTableName = HRDMastTableName;
                string str = "";
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
                    Sqltable.ColumnMappings.Add("CURR_USER", "CURR_USER");
                    Sqltable.ColumnMappings.Add("CURR_STATUS", "CURR_STATUS");
                    Sqltable.ColumnMappings.Add("CLIENT_MAKER", "CLIENT_MAKER");
                    //CURR_STATUS
                    Sqltable.WriteToServer(dtData);
                }

                //Now Delete Record from Temp Table
                StringBuilder SBTids = new StringBuilder();
                for(int i=0;i<dtData.Rows.Count;i++)
                {
                    SBTids.Append(dtData.Rows[i]["TID"]).Append(",");
                }
                string TempTableName = "Ivap_MAST_TEMP_" + EID;

                string StrTids = SBTids.ToString().Trim().TrimEnd(',');
                string StrQry = "Delete from " + TempTableName + " WHERE TID IN (" + StrTids + ");select 1;";
                var DbRes = Convert.ToInt32(DataLib.ExecuteScaler(StrQry, CommandType.Text, null));
                Res.Message = "Data Processed successfully.";
                Res.IsSuccess = true;
                return Res;
            }
            catch
            {
                throw;
            }
        }
        #endregion TempToTable


    }
}