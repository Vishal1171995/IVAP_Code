using Ivap.Areas.Configuration.Models;
using Ivap.Areas.Configuration.Repository;
using Ivap.Areas.InputProcessing.Repository;
using Ivap.Models;
using Ivap.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivap.Areas.PayrollOutput.Repository
{
    public class PayrollOutPutFileMovementRepo
    {
        public Response SetOutputWorkFlow(int UID, int RoleID, int FileID, int EID, string Remarks, DateTime PayDate)
        {
            Response Res = new Response();
            SqlTransaction trans = null;
            SqlConnection conn = null;
            string formattedDate = PayDate.ToString("yyyy/MM/dd");
            try
            {
                //Now Check file Rights
                FileSetupModel objModel = new FileSetupModel();
                FileSetupRepo objRepo = new FileSetupRepo();
                objModel.EID = EID;
                objModel.FILE_TYPE = "PMS Output File";
                DataTable dtFile = objRepo.GetFileType(objModel);
                DataView dvFileType = new DataView(dtFile);
                dvFileType.RowFilter = "TID=" + FileID;
                DataTable dtFilterdFile = dvFileType.ToTable();
                //If User Dont have rights then deny
                if (dtFilterdFile.Rows.Count == 0)
                {
                    Res.IsSuccess = false;
                    Res.Message = "Sorry!!! You don't have rights to approve this file.";
                    return Res;
                }

                //Get All File Of Current User
                string TempTableName = "Ivap_TEMP_HIS_" + EID;
                string StrTidQry = "select substring((select ','+ cast(TID as varchar) from " + TempTableName + " where cast(PayDate as date)=cast('" + formattedDate + "' as date) and Curr_User=" + UID + " and file_id=" + FileID + " for xml path('')),0,1000000) As Data";
                DataTable dtTids = DataLib.ExecuteDataTable(StrTidQry, CommandType.Text, null);
                //If No records found Do not proceed further
                string Tids = dtTids.Rows[0]["Data"].ToString().Trim().TrimEnd(',').TrimStart(',');
                if (Tids == "")
                {
                    Res.IsSuccess = false;
                    Res.Message = "Sorry!!! We are unable to process your request.No item found for approval.";
                    return Res;
                }



                //Now Get Next User From Data Table

                int Ordering = 1;
                if (RoleID == 2)
                    Ordering = 1;
                if (RoleID == 3)
                    Ordering = 2;
                if (RoleID == 9)
                    Ordering = 3;
                if (RoleID == 10)
                    Ordering = 4;
                if (RoleID == 8)
                    Ordering = 5;

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FileID",FileID),
                    new SqlParameter("@Ordering",Ordering),
                    new SqlParameter("@Role",RoleID),
                };


                DataTable dtWorkFlowSetting = DataLib.ExecuteDataTable("GetWfSetting", CommandType.StoredProcedure, parameters);
                int NextApprovalRole = 0;
                //If dt returns no Data it means no more approval required for that document Type
                if (dtWorkFlowSetting.Rows.Count == 0)
                {
                    //This means No More approval Found.... Archive This document
                    Tids = Tids.Trim().TrimStart(',').TrimEnd(',');
                    string StrWfQuery = "Update " + TempTableName + " Set CURR_STATUS='APPROVED',CURR_USER=98  WHERE TID IN (" + Tids + ")";
                    int No_Of_Records = Tids.Split(',').Length;
                    SqlParameter[] PNextApproverMov = new SqlParameter[]{
                            new SqlParameter("@UID", 98),
                            new SqlParameter("@FileID", FileID),
                            new SqlParameter("@No_Of_Records", No_Of_Records),
                            new SqlParameter("@Tids", Tids),
                            new SqlParameter("@Status", "APPROVED"),
                            new SqlParameter("@Remarks", Remarks)
                    };

                    conn = new SqlConnection(DataLib.GetConnectionString());
                    conn.Open();
                    trans = conn.BeginTransaction();

                    var TempHisRes = DataLib.ExecuteScaler(StrWfQuery, CommandType.Text, null, conn, trans);
                    var WFRes = DataLib.ExecuteScaler("SetOutputFileWorkFlow", CommandType.StoredProcedure, PNextApproverMov, conn, trans);
                    MoveArchiveToOutputFile(Table_Name: TempTableName, Tids: Tids, File_ID: FileID, Entity_ID: EID,Pay_Date:formattedDate ,conn: conn, trans: trans);
                    trans.Commit();
                    Res.IsSuccess = true;
                    Res.Message = "Approved successfully.";
                    return Res;

                }

                NextApprovalRole = Convert.ToInt32(dtWorkFlowSetting.Rows[0]["USER_ROLE"]);
                //Now Get Next Approval...
                SqlParameter[] PNextApprovernew = new SqlParameter[]{
                            new SqlParameter("@EID", EID),
                            new SqlParameter("@FileID", FileID),
                            new SqlParameter("@RoleID", NextApprovalRole)
                    };

                DataTable dtNextApprover = DataLib.ExecuteDataTable("GetNextApproval", CommandType.StoredProcedure, PNextApprovernew);
                if (dtNextApprover.Rows.Count == 0)
                {
                    Res.IsSuccess = false;
                    Res.Message = "Sorry!!! We are unable to process your request.Next Approver not found.";
                    return Res;
                }

                //Now Get Schema of Files for checking user data rights
                UploadInputRepo ObjIURepo = new UploadInputRepo();
                DataSet dsSchema = ObjIURepo.GetSchemaAndUserRIghtsOfInputFile(EID, FileID);
                DataView dvMSchema = new DataView(dsSchema.Tables[0]);
                dvMSchema.RowFilter = "COMPONENT_DATATYPE='MASTER' AND MANDATORY=1";
                DataTable dtSchema = dvMSchema.ToTable();

                int TotalUpdatedRecords = 0;
                conn = new SqlConnection(DataLib.GetConnectionString());
                conn.Open();
                trans = conn.BeginTransaction();
                for (int i = 0; i < dtNextApprover.Rows.Count; i++)
                {
                    int NextUID = Convert.ToInt32(dtNextApprover.Rows[0]["UID"]);
                    string ColumnToUpdate = "";

                    if (NextApprovalRole == 9)
                        ColumnToUpdate = "CLIENT_MAKER";
                    if (NextApprovalRole == 10)
                        ColumnToUpdate = "CLIENT_CHECKER";
                    if (NextApprovalRole == 8)
                        ColumnToUpdate = "CLIENT_ADMIN";
                    if (NextApprovalRole == 2)
                        ColumnToUpdate = "MYND_MAKER";
                    if (NextApprovalRole == 3)
                        ColumnToUpdate = "MYND_CHECKER";

                    string DataRIghtCond = "";
                    for (int j = 0; j < dtSchema.Rows.Count; j++)
                    {
                        //COMPONENT_TABLE_NAME
                        DataView dvRights = new DataView(dsSchema.Tables[1]);
                        dvRights.RowFilter = "TABLE_NAME='" + dtSchema.Rows[j]["COMPONENT_TABLE_NAME"].ToString() + "' AND UID=" + NextUID;
                        DataTable dtRights = dvRights.ToTable();
                        if (dtRights.Rows.Count > 0)
                        {
                            string RightsTids = dtRights.Rows[0]["TIDS"].ToString().Trim().TrimEnd(',');
                            DataRIghtCond = DataRIghtCond + " AND " + dtSchema.Rows[j]["COMPONENT_NAME"].ToString()
                                .Trim() + " IN (" + RightsTids + ")";
                        }
                    }

                    //Now Create Query For Updating Temp_His_Table

                    Tids = Tids.Trim().TrimStart(',').TrimEnd(',');
                    string CurrSelect = "select substring((select ','+ cast(TID as varchar) from " + TempTableName + " where Curr_User=" + UID + " AND TID IN (" + Tids + ")" + DataRIghtCond + " for xml path('')),0,1000000) As Data";

                    DataTable dtApprTids = DataLib.ExecuteDataTable(CurrSelect, CommandType.Text, null, conn, trans);
                    if (dtApprTids.Rows.Count > 0)
                    {
                        string CurrTids = dtApprTids.Rows[0]["Data"].ToString();
                        string StrWfQuery = "Update " + TempTableName + " Set " + ColumnToUpdate + "=" + NextUID + ",CURR_STATUS='PENDING',CURR_USER=" + NextUID + "  WHERE CURR_USER=" + UID + " AND TID IN (" + Tids + ")" + DataRIghtCond;
                        int No_Of_Records = CurrTids.Split(',').Length;
                        SqlParameter[] PNextApproverMov = new SqlParameter[]{
                            new SqlParameter("@UID", NextUID),
                            new SqlParameter("@FileID", FileID),
                            new SqlParameter("@No_Of_Records", No_Of_Records),
                            new SqlParameter("@Tids", CurrTids),
                            new SqlParameter("@Status", "PENDING"),
                            new SqlParameter("@Remarks", Remarks)
                        };

                        var TempHisRes = DataLib.ExecuteNonQuery(StrWfQuery, CommandType.Text, null, conn, trans);
                        TotalUpdatedRecords = TotalUpdatedRecords + TempHisRes;
                        var WFRes = DataLib.ExecuteScaler("SetOutputFileWorkFlow", CommandType.StoredProcedure, PNextApproverMov, conn, trans);
                    }
                }
                int Total_Of_Records = Tids.Split(',').Length;
                if (Total_Of_Records == TotalUpdatedRecords)
                {
                    trans.Commit();
                    Res.IsSuccess = true;
                    Res.Message = "Records Approved successfully.";
                    return Res;
                }
                trans.Rollback();
                Res.IsSuccess = false;
                Res.Message = "Sorry!!! We are unable to process your request.Next Approver not found.";
                return Res;

            }
            catch
            {
                if (conn != null)
                {
                    trans.Rollback();
                }
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    trans.Dispose();

                }
            }
        }

        public void MoveArchiveToOutputFile(string Table_Name, string Tids, int File_ID, int Entity_ID,string Pay_Date, SqlConnection conn, SqlTransaction trans)
        {
            try
            {
                DataTable dtData;
                string colString = DataLib.ExecuteScaler("SELECT STUFF((SELECT ', ' + dtl.Component_Name [text()]          FROM IVAP_MST_COMPONENT_ENTITY E inner join  IVAP_MST_FILE_COMP_DETAIL Dtl ON E.TID = dtl.COMPONENT_ID  		 where E.ENTITY_ID=" + Entity_ID + " AND dtl.FILE_ID=" + File_ID + " ORDER BY dtl.Display_Order  FOR XML PATH(''), TYPE)  .value('.','NVARCHAR(MAX)'),1,2,' ') List_Output", CommandType.Text, null);
                colString = colString + ",CREATED_BY,FILE_ID,CURR_STATUS ";
                dtData = DataLib.ExecuteDataTable("select " + colString + " from " + Table_Name + " with(nolock) WHERE TID IN (" + Tids + ") ", CommandType.Text, null, conn, trans);
                //First of all Get Schema of all table
                DataTable dtSchema = GetSchemaOfInputFile(EID: Entity_ID, FILE_ID: File_ID);

                //Filtering Companent of HRDMASTER
                DataView dvSchema = new DataView(dtSchema);
                dvSchema.RowFilter = "COMPONENT_FILE_TYPE='HRDMAST'";
                DataTable dtSchemaHRD = dvSchema.ToTable();

                //Now add check Emp_Code exist then Update else create new string
                


                SqlBulkCopy SqlHRDHIStable = null;
                string HRDHISMastTableName = "Ivap_HRD_HIS_" + Entity_ID;

                SqlHRDHIStable = new SqlBulkCopy(connection:conn,copyOptions:SqlBulkCopyOptions.KeepIdentity,externalTransaction:trans);
                SqlHRDHIStable.DestinationTableName = HRDHISMastTableName;
                DataTable dtHRDSheemaInsert = new DataTable();
               //Insert into history table
                if (dtSchemaHRD.Rows.Count > 0)
                {
                    for (int j = 0; j < dtSchemaHRD.Rows.Count; j++)
                    {
                        //str = str + "," +"'"+ SourceTable.Columns[j].ColumnName.ToUpper()+"'";
                        SqlHRDHIStable.ColumnMappings.Add(dtSchemaHRD.Rows[j]["COMPONENT_NAME"].ToString().ToUpper(), dtSchemaHRD.Rows[j]["COMPONENT_NAME"].ToString().ToUpper());
                        dtHRDSheemaInsert.Columns.Add(dtSchemaHRD.Rows[j]["COMPONENT_NAME"].ToString().ToUpper());
                    }
                    dtHRDSheemaInsert.Columns.Add("CREATED_BY");
                    dtHRDSheemaInsert.Columns.Add("FILE_ID");
                    dtHRDSheemaInsert.Columns.Add("CURR_STATUS");
                    SqlHRDHIStable.ColumnMappings.Add("CREATED_BY", "CREATED_BY");
                    SqlHRDHIStable.ColumnMappings.Add("FILE_ID", "FILE_ID");
                    SqlHRDHIStable.ColumnMappings.Add("CURR_STATUS", "WF_STATUS");
                    SqlHRDHIStable.ColumnMappings.Add("CREATED_BY", "UPDATED_BY");
                    SqlHRDHIStable.WriteToServer(dtData);
                }
                //insert or update into main table

                foreach (DataRow drHRD in dtData.Rows)
                {
                    string EMP_CODE = Convert.ToString(drHRD["EMP_CODE"]);
                    if (CheckHRMaster(EID: Entity_ID, EMP_CODE: EMP_CODE))
                    {//Writre Update query
                        StringBuilder updateHRDMAST = new StringBuilder();

                        ArrayList objHRDList = new ArrayList();
                        updateHRDMAST.Append("Update Ivap_HRD_MAST_" + Convert.ToString(Entity_ID) + " set ");
                        foreach (DataColumn dcHRD in dtData.Columns)
                        {
                            foreach (DataRow drScheemaHRD in dtSchemaHRD.Rows)
                            {
                                if (Convert.ToString(drScheemaHRD["COMPONENT_NAME"]).Trim().ToUpper() == dcHRD.ColumnName.ToString().Trim().ToUpper())
                                {
                                    if (drScheemaHRD["Component_DataType"].ToString().Trim().ToUpper() == "DATETIME")
                                    {
                                        if (Convert.ToString(drHRD[dcHRD]) != "")
                                        {
                                            objHRDList.Add("" + dcHRD.ColumnName + " = convert(datetime,'" + drHRD[dcHRD] + "',103) ");
                                        }
                                        else
                                        {
                                            objHRDList.Add("" + dcHRD.ColumnName + " = Null ");
                                        }
                                    }
                                    else if (drScheemaHRD["Component_DataType"].ToString().Trim().ToUpper() == "NUMBER" || drScheemaHRD["Component_DataType"].ToString().Trim().ToUpper() == "AMOUNT" || drScheemaHRD["Component_DataType"].ToString().Trim().ToUpper() == "MASTER")
                                    {
                                        if (Convert.ToString(drHRD[dcHRD]) == "")
                                        {
                                            objHRDList.Add("" + dcHRD.ColumnName + " = Null ");
                                        }
                                    }
                                    else
                                    {
                                        objHRDList.Add("" + dcHRD.ColumnName + " = '" + drHRD[dcHRD] + "'");
                                    }
                                }
                            }
                        }
                        updateHRDMAST.Append(string.Join(",", objHRDList.ToArray()));
                        updateHRDMAST.Append("  Where EMP_CODE='" + EMP_CODE + "';");
                        DataLib.ExecuteScaler(updateHRDMAST.ToString(), CommandType.Text, null,con: conn, trans: trans);
                    }
                    else
                    {
                        //Make data table to insert bulk copy
                        dtHRDSheemaInsert.ImportRow(drHRD);
                    }
                }
                if (dtHRDSheemaInsert.Rows.Count > 0)
                {
                    SqlBulkCopy SqlMainHRtable = null;
                    string HRDMastTableName = "Ivap_HRD_MAST_" + Entity_ID;

                    SqlMainHRtable = new SqlBulkCopy(connection: conn, copyOptions: SqlBulkCopyOptions.KeepIdentity, externalTransaction: trans);
                    SqlMainHRtable.DestinationTableName = HRDMastTableName;


                    //Insert into history table
                    if (dtSchemaHRD.Rows.Count > 0)
                    {
                        for (int j = 0; j < dtSchemaHRD.Rows.Count; j++)
                        {
                            //str = str + "," +"'"+ SourceTable.Columns[j].ColumnName.ToUpper()+"'";
                            SqlMainHRtable.ColumnMappings.Add(dtSchemaHRD.Rows[j]["COMPONENT_NAME"].ToString().ToUpper(), dtSchemaHRD.Rows[j]["COMPONENT_NAME"].ToString().ToUpper());

                        }

                        SqlMainHRtable.ColumnMappings.Add("CREATED_BY", "CREATED_BY");
                        SqlMainHRtable.ColumnMappings.Add("FILE_ID", "FILE_ID");
                        SqlMainHRtable.ColumnMappings.Add("CURR_STATUS", "WF_STATUS");
                        SqlMainHRtable.WriteToServer(dtHRDSheemaInsert);
                    }
                }
                SqlBulkCopy Sqltable = null;
                //Filtering Companent of PAYMASTER
                dvSchema = new DataView(dtSchema);
                dvSchema.RowFilter = "COMPONENT_FILE_TYPE='PAYMAST'";
                DataTable dtSchemaPAY = dvSchema.ToTable();
                DataTable dtPAYSheemaInsert = new DataTable();

                Sqltable = null;
                string PayHISMastTableName = "Ivap_PAY_HIS_" + Entity_ID;
                Sqltable = new SqlBulkCopy(connection:conn,copyOptions:SqlBulkCopyOptions.KeepIdentity,externalTransaction:trans);
                Sqltable.DestinationTableName = PayHISMastTableName;
                if (dtSchemaPAY.Rows.Count > 0)
                {
                    for (int j = 0; j < dtSchemaPAY.Rows.Count; j++)
                    {
                        dtPAYSheemaInsert.Columns.Add(dtSchemaPAY.Rows[j]["COMPONENT_NAME"].ToString().ToUpper());
                        //str = str + "," +"'"+ SourceTable.Columns[j].ColumnName.ToUpper()+"'";
                        if (dtSchemaPAY.Rows[j]["COMPONENT_NAME"].ToString().ToUpper() == "EMP_CODE")
                        {
                            Sqltable.ColumnMappings.Add("EMP_CODE", "PAY_EMP_CODE");
                        }
                        else
                        {
                            Sqltable.ColumnMappings.Add(dtSchemaPAY.Rows[j]["COMPONENT_NAME"].ToString().ToUpper(), dtSchemaPAY.Rows[j]["COMPONENT_NAME"].ToString().ToUpper());
                            
                        }
                    }
                    dtPAYSheemaInsert.Columns.Add("CREATED_BY");
                    dtPAYSheemaInsert.Columns.Add("FILE_ID");
                    dtPAYSheemaInsert.Columns.Add("CURR_STATUS");
                    Sqltable.ColumnMappings.Add("CREATED_BY", "CREATED_BY");
                    Sqltable.ColumnMappings.Add("FILE_ID", "FILE_ID");
                    Sqltable.ColumnMappings.Add("CURR_STATUS", "WF_STATUS");
                    Sqltable.ColumnMappings.Add("CREATED_BY", "UPDATED_BY");
                    
                    //Sqltable.ColumnMappings.Add("CURR_STATUS", "WF_STATUS");
                    Sqltable.WriteToServer(dtData);
                }


                foreach (DataRow drPAY in dtData.Rows)
                {
                    string EMP_CODE = Convert.ToString(drPAY["EMP_CODE"]);
                    if (CheckPAYMaster(EID: Entity_ID, EMP_CODE: EMP_CODE, Pay_Date:Pay_Date))
                    {//Writre Update query
                        StringBuilder updatePAYMAST = new StringBuilder();

                        ArrayList objPAYList = new ArrayList();
                        updatePAYMAST.Append("Update Ivap_PAY_MAST_" + Convert.ToString(Entity_ID) + " set ");
                        foreach (DataColumn dcPAY in dtData.Columns)
                        {
                            foreach (DataRow drScheemaPAY in dtSchemaPAY.Rows)
                            {
                                if (Convert.ToString(drScheemaPAY["COMPONENT_NAME"]).Trim().ToUpper() == dcPAY.ColumnName.ToString().Trim().ToUpper())
                                {
                                    if (drScheemaPAY["COMPONENT_DATATYPE"].ToString().ToUpper() == "DATETIME")
                                    {
                                        if (Convert.ToString(drPAY[dcPAY]) != "")
                                        {
                                            objPAYList.Add("" + dcPAY.ColumnName + " = convert(datetime,'" + drPAY[dcPAY] + "',103) ");
                                        }
                                        else
                                        {
                                            objPAYList.Add("" + dcPAY.ColumnName + " = Null ");
                                        }
                                    }
                                    else if (drScheemaPAY["COMPONENT_DATATYPE"].ToString().Trim().ToUpper() == "NUMBER" || drScheemaPAY["COMPONENT_DATATYPE"].ToString().Trim().ToUpper() == "AMOUNT" || drScheemaPAY["COMPONENT_DATATYPE"].ToString().Trim().ToUpper() == "MASTER")
                                    {
                                        if (Convert.ToString(drPAY[dcPAY]) == "")
                                        {
                                            objPAYList.Add("" + dcPAY.ColumnName + " = Null ");
                                        }
                                    }
                                    else
                                    {
                                        objPAYList.Add("" + dcPAY.ColumnName + " = '" + drPAY[dcPAY] + "'");
                                    }
                                }
                            }

                        }
                        updatePAYMAST.Append(string.Join(",", objPAYList.ToArray()));
                        updatePAYMAST.Append(" Where PAY_EMP_CODE='" + EMP_CODE + "';");
                        DataLib.ExecuteScaler(updatePAYMAST.ToString(), CommandType.Text, null, con: conn, trans: trans);
                    }
                    else
                    {
                        //Make data table to insert bulk copy
                        dtPAYSheemaInsert.ImportRow(drPAY);
                    }
                }
                if (dtPAYSheemaInsert.Rows.Count > 0)
                {
                    Sqltable = null;
                    string PayMastTableName = "Ivap_PAY_MAST_" + Entity_ID;
                    Sqltable = new SqlBulkCopy(connection: conn, copyOptions: SqlBulkCopyOptions.KeepIdentity, externalTransaction: trans);
                    Sqltable.DestinationTableName = PayMastTableName;
                    if (dtSchemaPAY.Rows.Count > 0)
                    {
                        for (int j = 0; j < dtSchemaPAY.Rows.Count; j++)
                        {
                            //str = str + "," +"'"+ SourceTable.Columns[j].ColumnName.ToUpper()+"'";
                            if (dtSchemaPAY.Rows[j]["COMPONENT_NAME"].ToString().ToUpper() == "EMP_CODE")
                            {
                                Sqltable.ColumnMappings.Add("EMP_CODE", "PAY_EMP_CODE");
                            }
                            else
                            {
                                Sqltable.ColumnMappings.Add(dtSchemaPAY.Rows[j]["COMPONENT_NAME"].ToString().ToUpper(), dtSchemaPAY.Rows[j]["COMPONENT_NAME"].ToString().ToUpper());
                            }
                        }
                        Sqltable.ColumnMappings.Add("CREATED_BY", "CREATED_BY");
                        Sqltable.ColumnMappings.Add("FILE_ID", "FILE_ID");
                        Sqltable.ColumnMappings.Add("CURR_STATUS", "WF_STATUS");
                        Sqltable.WriteToServer(dtData);
                    }
                }

            }
            catch (Exception ex)
            {   
                throw;
            }
        }
        public bool CheckHRMaster(int EID, string EMP_CODE)
        {
            bool result = false;
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@EID",EID),
                            new SqlParameter("@EMP_Code",EMP_CODE),
                        };
                int outresult = Convert.ToInt32(DataLib.ExecuteScaler("CheckHRMasterUser", CommandType.StoredProcedure, parameters));
                if (outresult == 1) { result = true; }

                return result;
            }
            catch
            {
                throw;
            }
        }
        public bool CheckPAYMaster(int EID, string EMP_CODE,string Pay_Date)
        {
            bool result = false;
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                        {
                            new SqlParameter("@EID",EID),
                            new SqlParameter("@EMP_Code",EMP_CODE),
                            new SqlParameter("@PayDate",Pay_Date),
                        };
                int outresult = Convert.ToInt32(DataLib.ExecuteScaler("CheckPayMasterForMonth", CommandType.StoredProcedure, parameters));
                if (outresult == 1) { result = true; }

                return result;
            }
            catch
            {
                throw;
            }
        }
        public Response Reconsider(int UID, int RoleID, int FileID, int EID, string Remarks, DateTime PayDate)
        {
            Response Res = new Response();
            SqlTransaction trans = null;
            SqlConnection conn = null;
            string formattedDate = PayDate.ToString("yyyy/MM/dd");

            try
            {
                //Now Check file Rights
                FileSetupModel objModel = new FileSetupModel();
                FileSetupRepo objRepo = new FileSetupRepo();
                objModel.EID = EID;
                objModel.FILE_TYPE = "PMS Output File";
                DataTable dtFile = objRepo.GetFileType(objModel);
                DataView dvFileType = new DataView(dtFile);
                dvFileType.RowFilter = "TID=" + FileID;
                DataTable dtFilterdFile = dvFileType.ToTable();
                //If User Dont have rights then deny
                if (dtFilterdFile.Rows.Count == 0)
                {
                    Res.IsSuccess = false;
                    Res.Message = "Sorry!!! You don't have rights to approve this file.";
                    return Res;
                }

                //Get All File Of Current User
                string TempTableName = "Ivap_TEMP_HIS_" + EID;
                string StrTidQry = "select substring((select ','+ cast(TID as varchar) from " + TempTableName + " where cast(PayDate as date)=cast('" + formattedDate + "' as date)  and File_ID=" + FileID + " and Curr_User=" + UID + " for xml path('')),0,1000000) As Data";
                DataTable dtTids = DataLib.ExecuteDataTable(StrTidQry, CommandType.Text, null);
                //If No records found Do not proceed further

                string Tids = dtTids.Rows[0]["Data"].ToString().Trim().TrimEnd(',').TrimStart(',');
                if (Tids == "")
                {
                    Res.IsSuccess = false;
                    Res.Message = "Sorry!!! We are unable to process your request.No item found for approval.";
                    return Res;
                }


                int TotalUpdatedRecords = 0;
                conn = new SqlConnection(DataLib.GetConnectionString());
                conn.Open();
                trans = conn.BeginTransaction();

                Tids = Tids.Trim().TrimStart(',').TrimEnd(',');

                string StrWfQuery = "Update " + TempTableName + " Set CLIENT_CHECKER=null,       CLIENT_ADMIN=null,   CLIENT_MAKER=null,       MYND_CHECKER=null,CURR_STATUS='PENDING',CURR_USER=CREATED_BY  WHERE CURR_USER=" + UID + " AND TID IN (" + Tids + ")";
                int No_Of_Records = Tids.Split(',').Length;
                SqlParameter[] PNextApproverMov = new SqlParameter[]{
                            new SqlParameter("@UID", UID),
                            new SqlParameter("@FileID", FileID),
                            new SqlParameter("@No_Of_Records", No_Of_Records),
                            new SqlParameter("@Tids", Tids),
                            new SqlParameter("@Status", "Reconsider"),
                            new SqlParameter("@Remarks", Remarks),
                            new SqlParameter("@PayDate", formattedDate)
                        };

                var TempHisRes = DataLib.ExecuteNonQuery(StrWfQuery, CommandType.Text, null, conn, trans);
                TotalUpdatedRecords = TotalUpdatedRecords + TempHisRes;
                var WFRes = DataLib.ExecuteScaler("SetOutputFileWorkFlow0", CommandType.StoredProcedure, PNextApproverMov, conn, trans);
                trans.Commit();
                Res.IsSuccess = true;
                Res.Message = "Records Reconsidered successfully.";
                return Res;
            }
            catch
            {
                if (conn != null)
                {
                    trans.Rollback();
                }
                throw;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    trans.Dispose();

                }
            }
        }

        public DataTable Download(AppUser ObjU, int File_ID)
        {
            PayrollOutPutFileSetUpRepo ObjObRepo = new PayrollOutPutFileSetUpRepo();

            DataTable dtSchema = ObjObRepo.GetSchemaOfInputFile(ObjU.EID, File_ID);
            string formattedDate = ObjU.PayDate.ToString("yyyy/MM/dd");

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
                    sqlQuery.Append(" convert(varchar,ivap_temp_his_" + ObjU.EID + "." + drSchema["COMPONENT_NAME"] + ", 103)").Append(" as ").Append("[" + drSchema["COMPONENT_DISPLAY_NAME"] + "]").Append(",");
                }
                else
                {
                    sqlQuery.Append(" ivap_temp_his_" + ObjU.EID).Append(".").Append(drSchema["COMPONENT_NAME"]).Append(" as ").Append("[" + drSchema["COMPONENT_DISPLAY_NAME"] + "]").Append(",");
                }

            }
            string StrMainQry = sqlQuery.ToString().Trim().TrimEnd(',');
            StrMainQry = StrMainQry + " from ivap_temp_his_" + ObjU.EID;

            StringBuilder SbMasterDataJoin = new StringBuilder();
            DataView dvMaster = new DataView(dtSchema);
            dvMaster.RowFilter = "COMPONENT_DATATYPE='MASTER'";
            DataTable dtMasterComponent = dvMaster.ToTable();
            StringBuilder SbTableName = new StringBuilder();

            for (int i = 0; i < dtMasterComponent.Rows.Count; i++)
            {
                MasterComponentAvailable = true;
                SbTableName.Append(" left join ").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"])
                    .Append(" on ivap_temp_his_" + ObjU.EID).Append(".").Append(dtMasterComponent.Rows[i]["COMPONENT_NAME"])
                    .Append(" = ").Append(dtMasterComponent.Rows[i]["COMPONENT_TABLE_NAME"]).Append(".TID");
            }
            // we will check access rights for master data for specific user and entity
            DataSet dsAllMaster = new DataSet();
            if (MasterComponentAvailable == true)
            {
                StrMainQry += SbTableName.ToString();
            }
            StrMainQry = StrMainQry + " where cast(ivap_temp_his_" + ObjU.EID + ".PAYDATE As Date) =cast('" + formattedDate + "' As Date) and ivap_temp_his_" + ObjU.EID + ".File_ID=" + File_ID;

            string ColumnToFilter = "";

            if (ObjU.Role == 9)
                ColumnToFilter = "CLIENT_MAKER";
            if (ObjU.Role == 10)
                ColumnToFilter = "CLIENT_CHECKER";
            if (ObjU.Role == 8)
                ColumnToFilter = "CLIENT_ADMIN";
            if (ObjU.Role == 2)
                ColumnToFilter = "MYND_MAKER";
            if (ObjU.Role == 3)
                ColumnToFilter = "MYND_CHECKER";
            StrMainQry = StrMainQry + " AND (CURR_USER=" + ObjU.UID + " OR " + ColumnToFilter + "=" + ObjU.UID + ")";
            DataTable dtData = DataLib.ExecuteDataTable(StrMainQry, CommandType.Text, null);
            return dtData;
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

    }
}