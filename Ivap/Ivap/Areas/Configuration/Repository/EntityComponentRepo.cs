using Ivap.Areas.Configuration.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Ivap.Areas.Configuration.Repository
{
    public class EntityComponentRepo
    {
        public DataTable SearchEntityComponent(int EID, string COMPONENT_FILE_TYPE, string Search_Text, string Globle_Component_ID)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@COMPONENT_FILE_TYPE",COMPONENT_FILE_TYPE),
                    new SqlParameter("@Search_Text",Search_Text),
                    new SqlParameter("@EID",EID),
                    new SqlParameter("@Globle_Component_ID",Globle_Component_ID),
                };
                return DataLib.ExecuteDataTable("GetGlobleComponent", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }

        #region EntityComp

        public Response SetEntityComponent(EntityComponentModel model)
        {
            SqlTransaction trans = null;
            SqlConnection con = null;
            Response Res = new Response();
            try
            {
                con = new SqlConnection(Ivap.Utils.DataLib.GetConnectionString());
                con.Open();
                trans = con.BeginTransaction();
                Res = AddUpdateEntityComponent(model, con, trans);
                if (Res.IsSuccess == true)
                    trans.Commit();
                else
                    trans.Rollback();
                return Res;
            }
            catch (Exception ex)
            {
                trans.Rollback();

                throw;
            }
            finally
            {
                if (con != null)
                {
                    con.Dispose();
                    trans.Dispose();
                }

            }
        }
        public Response AddUpdateEntityComponent(EntityComponentModel model, SqlConnection Con, SqlTransaction Trans)
        {
            Response Res = new Response();
           
            try
            {

                //Now check 
                bool IsPublished = false;
                DataSet dsPublishStatus = CheckPublistStatus(model.EID,trans:Trans,conn:Con);
                if (dsPublishStatus.Tables[2].Rows.Count <= 0 && dsPublishStatus.Tables[0].Rows.Count > 0)
                    PublishTempMaster(model.EID, "HRDMAST",tran:Trans,con:Con);
                if (model.File_Type.Trim().ToUpper() == "HRDMAST")
                {
                    if (dsPublishStatus.Tables[0].Rows.Count > 0)
                        IsPublished = true;
                }
                else
                {
                    if (dsPublishStatus.Tables[1].Rows.Count > 0)
                        IsPublished = true;
                }

                foreach (var list in model.Globle_Component_ID)
                {
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@EntityCompID",model.EntityCOMPID),
                        new SqlParameter("@Globle_Component_ID",list),
                        new SqlParameter("@ENTITY_ID",model.EID),
                        new SqlParameter("@COMPONENT_DISPLAY_NAME",model.COMPONENT_DISPLAY_NAME),
                        new SqlParameter("@COMPONENT_DATATYPE",model.COMPONENT_DATATYPE),
                        new SqlParameter("@Component_TableName",model.Component_TableName),
                        new SqlParameter("@Component_FieldName",model.Component_FieldName),
                        new SqlParameter("@MIN_LENGTH",model.MIN_LENGTH),
                        new SqlParameter("@MAX_LENGTH",model.MAX_LENGTH),
                        new SqlParameter("@MANDATORY",model.MANDATORY),
                        new SqlParameter("@CREATED_BY",model.CreatedBy),
                        new SqlParameter("@ISACT",model.IsActive),
                        new SqlParameter("@GL_CODE",model.GL_Code),
                        new SqlParameter("@PMS_CODE",model.PMS_Code),
                        new SqlParameter("@EXTRA_INPUT_VALIDATION",model.Extra_Validation),
                        new SqlParameter("@EXTRA_RG_EXPRESSION",model.Expression),
                    };
                    int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateEntityComponentNEW20", CommandType.StoredProcedure, parameters, Con, Trans));
                    //If Datatable already created run alter statement
                    Response AlterRes = new Response();
                    if (IsPublished && result >= 0)
                    {
                        AlterRes = AlterObject(model.EID, model.EntityCOMPID == 0 ? result : model.EntityCOMPID, Con, Trans);

                        AlterRes = AlterTempObject(model.EID, model.EntityCOMPID == 0 ? result : model.EntityCOMPID, Con, Trans);
                        if (AlterRes.IsSuccess == false)
                        {
                            //trans.Rollback();
                            Res.IsSuccess = false;
                            Res.Message = "Sorry!!!Unable to process your request.Something went wrong.";
                            return Res;
                        }
                    }

                    if (result == -4)
                    {
                        //trans.Rollback();
                        Res.IsSuccess = false;
                        Res.Message = "Failed!!! Component Display Name must be unique.";
                        return Res;
                    }
                }
                //trans.Commit();
                Res.IsSuccess = true;
                Res.Data = IsPublished.ToString();
                Res.Message = "Congratulation!!!. Component Submited successfully.";
                return Res;
            }
            catch (Exception ex)
            {
                Trans.Rollback();

                throw;
            }
        }
        public DataTable GetEntityComponent(EntityComponentModel model, SqlConnection Conn = null, SqlTransaction Trans = null)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@EntityCompID",model.EntityCOMPID),
                    new SqlParameter("@EntityID",model.EID),
                    new SqlParameter("@ISAct",model.IsActive),
                    new SqlParameter("@COMPONENT_FILE_TYPE",model.COMPONENT_FILE_TYPE)
                };
                if (Trans == null)
                    return DataLib.ExecuteDataTable("GetComponentEntity17", CommandType.StoredProcedure, parameters);
                else
                    return DataLib.ExecuteDataTable("GetComponentEntity17", CommandType.StoredProcedure, parameters, Conn, Trans);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataSet CheckPublistStatus(int EntityID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@EntityID", EntityID),
                };

                return DataLib.ExecuteDataSet("CheckPublistStatus", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
        public DataSet CheckPublistStatus(int EntityID,SqlTransaction trans,SqlConnection conn)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@EntityID", EntityID),
                };

                return DataLib.ExecuteDataSet("CheckPublistStatus", CommandType.StoredProcedure, parameters,con:conn,trans:trans);
            }
            catch
            {
                throw;
            }
        }

        public Response DeleteEntityComponent(EntityComponentModel model)
        {
            SqlTransaction trans = null;
            SqlConnection con = null;
            Response res = new Response();
            try
            {
                con = new SqlConnection(Ivap.Utils.DataLib.GetConnectionString());
                con.Open();
                trans = con.BeginTransaction();
                //Now check 
                bool IsPublished = false;
                DataSet dsPublishStatus = CheckPublistStatus(model.EID,trans:trans,conn:con);
                if (dsPublishStatus.Tables[2].Rows.Count <= 0)
                    PublishTempMaster(model.EID, "HRDMAST",tran:trans,con:con);
                if (model.File_Type.Trim().ToUpper() == "HRDMAST")
                {
                    if (dsPublishStatus.Tables[0].Rows.Count > 0)
                        IsPublished = true;
                }
                else
                {
                    if (dsPublishStatus.Tables[1].Rows.Count > 0)
                        IsPublished = true;
                }

                //If Datatable already created run delete statement
                Response AlterRes = new Response();
                if (IsPublished)
                {
                    AlterRes = DropColumn(model.EID, model.EntityCOMPID, con, trans);
                    //Drop Temp table Method
                    AlterRes = DropTempColumn(model.EID, model.EntityCOMPID, con, trans);
                }
                int result = 0;

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EntityCompID",model.EntityCOMPID),
                     new SqlParameter("@EntityID",model.EID)
                };
                result = Convert.ToInt32(DataLib.ExecuteScaler("DeleteEntityComponent", CommandType.StoredProcedure, parameters, con, trans));
                if (IsPublished && AlterRes.IsSuccess == false)
                {
                    trans.Rollback();
                    res.IsSuccess = false;
                    res.Message = "Sorry!!!Unable to process your request.Something went wrong.";
                    return res;
                }

                if (result < 0)
                {
                    trans.Rollback();
                    res.IsSuccess = false;
                    res.Message = "File Component Details Delete First..";
                    return res;
                }
                if (result > 1)
                {
                    trans.Commit();
                    res.IsSuccess = true;
                    res.Message = "Component Deleted Successfully.";
                }
                return res;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw;
            }
            finally
            {
                if (con != null)
                {
                    con.Dispose();
                    trans.Dispose();
                }

            }
        }
        public DataTable GetEntityComponentHistory(EntityComponentModel model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@CompEntityID",model.EntityCOMPID ),
                };
                return DataLib.ExecuteDataTable("GetComponentEntityHis", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }


        public Response DropColumn(int EID, int EntityCOMPID, SqlConnection Conn, SqlTransaction Trans)
        {
            Response Res = new Response();
            try
            {

                EntityComponentModel ObjComponent = new EntityComponentModel();
                ObjComponent.EID = EID;
                ObjComponent.EntityCOMPID = EntityCOMPID;

                //Now Generate DDL Statement Ivap_HRD_MAST_32
                string HRD_Table_Name = "Ivap_PAY_MAST_" + EID;
                //Generating string for History Table
                string His_Table_Name = "Ivap_PAY_HIS_" + EID;
                DataTable dtComponent = GetEntityComponent(ObjComponent, Conn, Trans);

                if (dtComponent.Rows[0]["COMPONENT_FILE_TYPE"].ToString().Trim().ToUpper() == "HRDMAST")
                {
                    HRD_Table_Name = "Ivap_HRD_MAST_" + EID;
                    His_Table_Name = "Ivap_HRD_HIS_" + EID;
                }
                string ColumnText = dtComponent.Rows[0]["COMPONENT_NAME"].ToString().Trim().ToUpper();
                StringBuilder SBQry = new StringBuilder();
                SBQry.Append("if  exists(select Name from sys.columns where object_ID = object_ID('" + HRD_Table_Name + "') AND Name = '" + ColumnText + "')");
                //if not exists(select Name from sys.columns where object_ID = object_ID('Ivap_HRD_MAST_1') AND Name = '')
                SBQry.Append(" BEGIN ");
                SBQry.Append(" alter table ").Append(HRD_Table_Name).Append(" drop column ").Append(ColumnText);
                SBQry.Append(" alter table ").Append(His_Table_Name).Append(" drop column ").Append(ColumnText);
                SBQry.Append(" END ");
                SBQry.Append("; select 1");
                int DbRes = Convert.ToInt32(DataLib.ExecuteScaler(SBQry.ToString(), CommandType.Text, null, Conn, Trans));
                Res.IsSuccess = true;
                Res.Message = "Column droped successfully.";
                return Res;
            }
            catch
            {
                throw;
            }
        }

        #endregion
        #region PublishObject

        public Response setPublishHRDMaster(int EID, SqlTransaction trans, SqlConnection con)
        {
            Response res = new Response();
            try
            {
                EntityComponentModel ObjComponent = new EntityComponentModel();
                ObjComponent.EID = EID;
                ObjComponent.EntityCOMPID = 0;
                ObjComponent.COMPONENT_FILE_TYPE = "HRDMAST";
                DataTable dtComponent = GetEntityComponent(ObjComponent,Conn:con,Trans:trans);
                DataView DataComp = dtComponent.DefaultView;
                DataComp.RowFilter = "COMPONENT_NAME='EMP_CODE'";
                DataTable CheckEmpcode = DataComp.ToTable();
                DataView DataCompPayDate = dtComponent.DefaultView;
                DataComp.RowFilter = "COMPONENT_NAME='HR_PAYDATE'";
                DataTable CheckPayDate = DataCompPayDate.ToTable();
                //Now Generate DDL Statement Ivap_HRD_MAST_32
                if (CheckEmpcode.Rows.Count > 0)
                {
                    PublishTempMaster(EID, "HRDMAST",tran:trans,con:con);
                    string HRD_Table_Name = "Ivap_HRD_MAST_" + EID;
                    StringBuilder SBHRDStatic = new StringBuilder();
                    //Now initialise HRD Static Column
                    SBHRDStatic.Append("TID int IDENTITY(1, 1) PRIMARY KEY,FILE_ID int,WF_STATUS varchar(2000),CREATED_ON datetime default(getdate()),CREATED_BY int,IS_VALID INT, ");
                    //Generating string for History Table
                    string His_Table_Name = "Ivap_HRD_HIS_" + EID;
                    StringBuilder SBHisStatic = new StringBuilder();
                    //Now initialise HRD Static Column
                    SBHisStatic.Append("TID int IDENTITY(1, 1) PRIMARY KEY,FILE_ID int,WF_STATUS varchar(2000),HRDID INT,UPDATED_ON datetime default(getdate()),UPDATED_BY int,ACTION varchar(200) ,CREATED_ON datetime default(getdate()),CREATED_BY int,IS_VALID INT, ");
                    //Now create dynamic column
                    StringBuilder SBDynColumn = new StringBuilder();
                    for (int i = 0; i < dtComponent.Rows.Count; i++)
                    {
                        string DataType = dtComponent.Rows[i]["COMPONENT_DATATYPE"].ToString().Trim().ToUpper();
                        string Max = dtComponent.Rows[i]["Max_Length"].ToString().Trim().ToUpper();
                        string ColumnText = dtComponent.Rows[i]["COMPONENT_NAME"].ToString().Trim().ToUpper();
                        switch (DataType)
                        {
                            case "TEXT":
                                {
                                    SBDynColumn.Append(ColumnText).Append(" nvarchar(").Append(Max).Append(")");
                                    break;
                                }
                            case "MASTER":
                                {
                                    SBDynColumn.Append(ColumnText).Append(" int");
                                    break;
                                }
                            case "DATETIME":
                                {
                                    SBDynColumn.Append(ColumnText).Append(" datetime");
                                    break;
                                }
                            case "AMOUNT":
                                {
                                    SBDynColumn.Append(ColumnText).Append(" numeric(16,2)");
                                    break;
                                }
                            case "NUMBER":
                                {
                                    SBDynColumn.Append(ColumnText).Append(" int");
                                    break;
                                }
                            default:
                                {
                                    SBDynColumn.Append(ColumnText).Append(" nvarchar(").Append(Max).Append(")");
                                    break;
                                }
                        }
                        SBDynColumn.Append(",");
                    }
                    string StrHRDCreateStatement = " Create table " + HRD_Table_Name + "(" + SBHRDStatic.ToString() + SBDynColumn.ToString().TrimEnd(',') + ");";
                    string StrHisCreateStatement = " Create table " + His_Table_Name + "(" + SBHisStatic.ToString() + SBDynColumn.ToString().TrimEnd(',') + ");";
                    string StrQry = "BEGIN TRAN BEGIN TRY if NOT exists(select Name from Sys.objects where Type='u' and name ='" + HRD_Table_Name + "') BEGIN ";
                    StrQry = StrQry + StrHRDCreateStatement + ";" + StrHisCreateStatement + "; COMMIT; select 1 ";
                    StrQry = StrQry + "END else select -1; END TRY BEGIN CATCH ROLLBACK;END CATCH";
                    int DbRes = Convert.ToInt32(DataLib.ExecuteScaler(StrQry, CommandType.Text,null,con:con,trans:trans));
                    res.IsSuccess = true;
                    res.Message = "Table published successfully.";
                    if (DbRes == -1)
                        res.Message = "This table is already published.";
                }
                else
                {
                    res.IsSuccess = false;
                    res.Message = "Please Add EMP_CODE in HRDMAST Component.";
                }
            }
            catch {
                res.IsSuccess = false;
                res.Message = "There are some error while publish table";
            }
            return res;
        }
        public Response PublishHRDMaster(int EID)
        {
            Response Res = new Response();
            SqlTransaction trans = null;
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(Ivap.Utils.DataLib.GetConnectionString());
                con.Open();
                trans = con.BeginTransaction();
                Res = setPublishHRDMaster(EID: EID, trans: trans, con: con);
                if (Res.IsSuccess)
                {
                    trans.Commit();
                }
                else {
                    trans.Rollback();
                }
                con.Dispose();
                return Res;
            }
            catch
            {
                throw;
            }
        }

        public Response setPublishPayMaster(int EID,SqlConnection con,SqlTransaction tran) {
            Response Res = new Response();
            try
            {
                EntityComponentModel ObjComponent = new EntityComponentModel();
                ObjComponent.EID = EID;
                ObjComponent.EntityCOMPID = 0;
                ObjComponent.COMPONENT_FILE_TYPE = "PAYMAST";
                DataTable dtComponent = GetEntityComponent(ObjComponent,Conn:con,Trans:tran);
                DataView DataComp = dtComponent.DefaultView;
                DataComp.RowFilter = "COMPONENT_NAME='PAY_EMP_CODE'";
                DataTable CheckPayEmpcode = DataComp.ToTable();
                DataView DataCompPayDate = dtComponent.DefaultView;
                DataComp.RowFilter = "COMPONENT_NAME='PAYDATE'";
                DataTable CheckPayDate = DataCompPayDate.ToTable();
                if (CheckPayEmpcode.Rows.Count > 0 && CheckPayDate.Rows.Count > 0)
                {
                    PublishTempMaster(EID, "PAYMAST",tran:tran,con:con);
                    //Now Generate DDL Statement Ivap_HRD_MAST_32
                    string HRD_Table_Name = "Ivap_PAY_MAST_" + EID;
                    StringBuilder SBHRDStatic = new StringBuilder();
                    //Now initialise HRD Static Column
                    SBHRDStatic.Append("TID int IDENTITY(1, 1) PRIMARY KEY,WF_STATUS varchar(2000),FILE_ID int,CREATED_ON datetime default(getdate()),CREATED_BY int,IS_VALID INT, ");
                    //Generating string for History Table
                    string His_Table_Name = "Ivap_PAY_HIS_" + EID;
                    StringBuilder SBHisStatic = new StringBuilder();
                    //Now initialise HRD Static Column
                    SBHisStatic.Append("TID int IDENTITY(1, 1) PRIMARY KEY,FILE_ID int,WF_STATUS varchar(2000),HRDID INT,UPDATED_ON datetime default(getdate()),UPDATED_BY int,ACTION varchar(200) ,CREATED_ON datetime default(getdate()),CREATED_BY int,IS_VALID INT, ");
                    //Now create dynamic column
                    StringBuilder SBDynColumn = new StringBuilder();
                    for (int i = 0; i < dtComponent.Rows.Count; i++)
                    {
                        string DataType = dtComponent.Rows[i]["COMPONENT_DATATYPE"].ToString().Trim().ToUpper();
                        string Max = dtComponent.Rows[i]["Max_Length"].ToString().Trim().ToUpper();
                        string ColumnText = dtComponent.Rows[i]["COMPONENT_NAME"].ToString().Trim().ToUpper();
                        switch (DataType)
                        {
                            case "TEXT":
                                {
                                    SBDynColumn.Append(ColumnText).Append(" nvarchar(").Append(Max).Append(")");
                                    break;
                                }
                            case "MASTER":
                                {
                                    SBDynColumn.Append(ColumnText).Append(" int");
                                    break;
                                }
                            case "DATETIME":
                                {
                                    SBDynColumn.Append(ColumnText).Append(" datetime");
                                    break;
                                }
                            case "AMOUNT":
                                {
                                    SBDynColumn.Append(ColumnText).Append(" numeric(16,2)");
                                    break;
                                }
                            case "NUMBER":
                                {
                                    SBDynColumn.Append(ColumnText).Append(" int");
                                    break;
                                }
                            default:
                                {
                                    SBDynColumn.Append(ColumnText).Append(" nvarchar(").Append(Max).Append(")");
                                    break;
                                }
                        }
                        SBDynColumn.Append(",");
                    }
                    string StrHRDCreateStatement = " Create table " + HRD_Table_Name + "(" + SBHRDStatic.ToString() + SBDynColumn.ToString().TrimEnd(',') + ");";
                    string StrHisCreateStatement = " Create table " + His_Table_Name + "(" + SBHisStatic.ToString() + SBDynColumn.ToString().TrimEnd(',') + ");";
                    string StrQry = "BEGIN TRAN BEGIN TRY if NOT exists(select Name from Sys.objects where Type='u' and name ='" + HRD_Table_Name + "') BEGIN ";
                    StrQry = StrQry + StrHRDCreateStatement + ";" + StrHisCreateStatement + "; COMMIT; select 1 ";
                    StrQry = StrQry + "END else select -1; END TRY BEGIN CATCH ROLLBACK;END CATCH";
                    int DbRes = Convert.ToInt32(DataLib.ExecuteScaler(StrQry, CommandType.Text, null,con:con,trans:tran));
                    Res.IsSuccess = true;
                    Res.Message = "Table published successfully.";
                    if (DbRes == -1)
                        Res.Message = "This table is already published.";
                }
                else
                {
                    Res.IsSuccess = false;
                    Res.Message = "Please Add PAY_EMP_CODE,PAYDATE in PAYMAST Component.";
                }
                return Res;
            }
            catch {
                Res.IsSuccess = false;
                Res.Message = "There are some error while publish table";
                return Res;
            }
        }
        public Response PublishPayMaster(int EID)
        {
            Response Res = new Response();
            SqlConnection con = null;
            SqlTransaction tran = null;
            try
            {
                con = new SqlConnection(Ivap.Utils.DataLib.GetConnectionString());
                tran = con.BeginTransaction();
                Res= setPublishPayMaster(EID:EID,con:con,tran:tran);
                if (Res.IsSuccess)
                {
                    tran.Commit();
                }
                else {
                    tran.Rollback();
                }
                con.Dispose();
                return Res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public Response AlterObject(int EID, int EntityCOMPID, SqlConnection Con, SqlTransaction Trans)
        {
            Response Res = new Response();
            try
            {

                EntityComponentModel ObjComponent = new EntityComponentModel();
                ObjComponent.EID = EID;
                ObjComponent.EntityCOMPID = EntityCOMPID;

                //Now Generate DDL Statement Ivap_HRD_MAST_32
                string HRD_Table_Name = "Ivap_PAY_MAST_" + EID;
                //Generating string for History Table
                string His_Table_Name = "Ivap_PAY_HIS_" + EID;
                DataTable dtComponent = GetEntityComponent(ObjComponent, Con, Trans);

                if (dtComponent.Rows[0]["COMPONENT_FILE_TYPE"].ToString().Trim().ToUpper() == "HRDMAST")
                {
                    HRD_Table_Name = "Ivap_HRD_MAST_" + EID;
                    His_Table_Name = "Ivap_HRD_HIS_" + EID;
                }

                string DataType = dtComponent.Rows[0]["COMPONENT_DATATYPE"].ToString().Trim().ToUpper();
                string Max = "MAX";//dtComponent.Rows[0]["Max_Length"].ToString().Trim().ToUpper();
                string ColumnText = dtComponent.Rows[0]["COMPONENT_NAME"].ToString().Trim().ToUpper();
                StringBuilder SBAlterColumn = new StringBuilder();
                switch (DataType)
                {
                    case "TEXT":
                        {
                            SBAlterColumn.Append(ColumnText).Append(" nvarchar(").Append(Max).Append(")");
                            break;
                        }
                    case "MASTER":
                        {
                            SBAlterColumn.Append(ColumnText).Append(" int");
                            break;
                        }
                    case "DATETIME":
                        {
                            SBAlterColumn.Append(ColumnText).Append(" datetime");
                            break;
                        }
                    case "AMOUNT":
                        {
                            SBAlterColumn.Append(ColumnText).Append(" numeric(16,2)");
                            break;
                        }
                    case "NUMBER":
                        {
                            SBAlterColumn.Append(ColumnText).Append(" numeric(16,2)");
                            break;
                        }
                    default:
                        {
                            SBAlterColumn.Append(ColumnText).Append(" numeric(16,2)");
                            break;
                        }
                }
                StringBuilder SBQry = new StringBuilder();
                SBQry.Append("if not exists(select Name from sys.columns where object_ID = object_ID('" + HRD_Table_Name + "') AND Name = '" + ColumnText + "')");
                //if not exists(select Name from sys.columns where object_ID = object_ID('Ivap_HRD_MAST_1') AND Name = '')
                SBQry.Append(" BEGIN ");
                SBQry.Append(" alter table ").Append(HRD_Table_Name).Append(" ADD ").Append(SBAlterColumn);
                SBQry.Append(" alter table ").Append(His_Table_Name).Append(" ADD ").Append(SBAlterColumn);
                SBQry.Append(" END ");
                SBQry.Append(" ELSE BEGIN ");
                SBQry.Append(" ALTER TABLE  ").Append(HRD_Table_Name).Append(" alter column ").Append(SBAlterColumn);
                SBQry.Append(" ALTER TABLE  ").Append(His_Table_Name).Append(" alter column ").Append(SBAlterColumn);
                SBQry.Append("; END select 1");
                int DbRes = Convert.ToInt32(DataLib.ExecuteScaler(SBQry.ToString(), CommandType.Text, null, Con, Trans));
                Res.IsSuccess = true;
                Res.Message = "Table altered successfully.";
                Res.Data = Convert.ToString(DbRes);
                return Res;
            }
            catch
            {
                throw;
            }
        }

        #endregion PublishObject


        #region PublishTempObject
        public Response PublishTempMaster(int EID, string Filetype,SqlTransaction tran, SqlConnection con)
        {
            Response Res = new Response();
            try
            {
                PublishTempHisTable(EID, Filetype,tran:tran,con:con);
            }
            catch
            { }
            try
            {
                EntityComponentModel ObjComponent = new EntityComponentModel();
                ObjComponent.EID = EID;
                ObjComponent.EntityCOMPID = 0;
                DataSet dsPublishStatus = CheckPublistStatus(EID,trans:tran,conn:con);
                if (Filetype != "HRDMAST" && dsPublishStatus.Tables[2].Rows.Count > 0)
                    ObjComponent.COMPONENT_FILE_TYPE = Filetype;
                DataTable dtComponent = GetEntityComponent(ObjComponent,Conn:con,Trans:tran);
                //Now Generate DDL Statement Ivap_HRD_MAST_32
                string TEMP_Table_Name = "Ivap_MAST_TEMP_" + EID;
                StringBuilder SBHRDStatic = new StringBuilder();
                //Now initialise HRD Static Column
                SBHRDStatic.Append("TID int IDENTITY(1, 1) PRIMARY KEY,FILE_ID int,TEMP_BATCH_ID int,TEMP_BATCH_NO varchar(100),TEMP_STATUS VARCHAR(100),CREATED_ON datetime default(getdate()),CREATED_BY int,IS_VALID INT, ");
                int DbRes = 0;
                //Now create dynamic column
                StringBuilder SBDynColumn = new StringBuilder();
                for (int i = 0; i < dtComponent.Rows.Count; i++)
                {
                    string DataType = dtComponent.Rows[i]["COMPONENT_DATATYPE"].ToString().Trim().ToUpper();
                    string Max = "MAX";////dtComponent.Rows[i]["Max_Length"].ToString().Trim().ToUpper();
                    string ColumnText = dtComponent.Rows[i]["COMPONENT_NAME"].ToString().Trim().ToUpper();
                    if (ColumnText != "PAY_EMP_CODE")
                    {
                        if (Filetype == "PAYMAST" && dsPublishStatus.Tables[2].Rows.Count > 0)
                        {
                            StringBuilder SBQry = new StringBuilder();
                            SBQry.Append("if not exists(select Name from sys.columns where object_ID = object_ID('" + TEMP_Table_Name + "')AND Name = '" + ColumnText + "')");
                            SBQry.Append(" BEGIN ");
                            SBQry.Append(" alter table ").Append(TEMP_Table_Name).Append(" ADD ").Append(ColumnText.ToString()).Append(" varchar(").Append(Max).Append(")");
                            SBQry.Append(" END ");
                            SBQry.Append(" ELSE BEGIN ");
                            SBQry.Append(" ALTER TABLE  ").Append(TEMP_Table_Name).Append(" alter column ").Append(ColumnText.ToString()).Append(" varchar(").Append(Max).Append(")");
                            SBQry.Append("; END select 1");
                            DbRes = Convert.ToInt32(DataLib.ExecuteScaler(SBQry.ToString(), CommandType.Text, null,con:con,trans:tran));
                        }
                        SBDynColumn.Append(ColumnText).Append(" varchar(").Append(Max).Append("),");
                    }
                    //SBDynColumn.Append(",");
                }

                if (Filetype == "HRDMAST")
                {
                    string StrHRDCreateStatement = " Create table " + TEMP_Table_Name + "(" + SBHRDStatic.ToString() + SBDynColumn.ToString().TrimEnd(',') + ");";
                    string StrQry = "BEGIN TRAN BEGIN TRY if NOT exists(select Name from Sys.objects where Type='u' and name ='" + TEMP_Table_Name + "') BEGIN ";
                    StrQry = StrQry + StrHRDCreateStatement + "; COMMIT; select 1 ";
                    StrQry = StrQry + "END else select -1; END TRY BEGIN CATCH ROLLBACK;END CATCH";
                    DbRes = Convert.ToInt32(DataLib.ExecuteScaler(StrQry, CommandType.Text, null,con:con,trans:tran));
                }
                /////
                //if (Filetype == "PAYMAST" && dsPublishStatus.Tables[2].Rows.Count>0)
                //{
                //    StringBuilder SBQry = new StringBuilder();
                //    SBQry.Append("if exists(select Name from sys.columns where object_ID = object_ID('" + TEMP_Table_Name + "'))");
                //    SBQry.Append(" BEGIN ");
                //    SBQry.Append(" alter table ").Append(TEMP_Table_Name).Append(" ADD ").Append(SBDynColumn.ToString().TrimEnd(',').ToUpper());
                //    SBQry.Append("; END select 1");
                //    DbRes = Convert.ToInt32(DataLib.ExecuteScaler(SBQry.ToString(), CommandType.Text, null));
                //}
                //
                Res.IsSuccess = true;
                Res.Message = "Table published successfully.";
                if (DbRes == -1)
                    Res.Message = "This table is already published.";
                if (DbRes == 0)
                    Res.Message = "This table is already published.";
                return Res;
            }
            catch
            {
                throw;
            }
        }

        public Response PublishTempHisTable(int EID, string Filetype,SqlTransaction tran, SqlConnection con)
        {
            Response Res = new Response();
            try
            {
                EntityComponentModel ObjComponent = new EntityComponentModel();
                ObjComponent.EID = EID;
                ObjComponent.EntityCOMPID = 0;
                DataSet dsPublishStatus = CheckPublistStatus(EID,trans:tran,conn:con);
                if (Filetype != "HRDMAST" && dsPublishStatus.Tables[2].Rows.Count > 0)
                    ObjComponent.COMPONENT_FILE_TYPE = Filetype;

                DataTable dtComponent = GetEntityComponent(ObjComponent,Conn:con,Trans:tran);
                //Now Generate DDL Statement Ivap_HRD_MAST_32
                string TEMP_Table_Name = "Ivap_TEMP_HIS_" + EID;
                StringBuilder SBHRDStatic = new StringBuilder();
                //Now initialise HRD Static Column
                SBHRDStatic.Append("TID int IDENTITY(1, 1) PRIMARY KEY,FILE_ID int,TEMP_BATCH_ID int,TEMP_BATCH_NO varchar(100),WF_STATUS VARCHAR(200),CURR_STATUS VARCHAR(200),CURR_USER INT,CREATED_ON datetime default(getdate()),CREATED_BY int, ");
                SBHRDStatic.Append("CLIENT_MAKER INT,CLIENT_MAKER_DATE datetime,CLIENT_CHECKER INT,CLIENT_CHECKER_DATE datetime,CLIENT_ADMIN INT,CLIENT_ADMIN_DATE datetime,MYND_MAKER INT,MYND_MAKER_DATE datetime,MYND_CHECKER INT,MYND_CHECKER_DATE datetime,IS_VALID INT, ");
                int DbRes = 0;
                //Now create dynamic column
                StringBuilder SBDynColumn = new StringBuilder();
                StringBuilder SBColumnDef = new StringBuilder();
                for (int i = 0; i < dtComponent.Rows.Count; i++)
                {
                    string ColumnText = dtComponent.Rows[i]["COMPONENT_NAME"].ToString().Trim().ToUpper();
                    SBColumnDef = new StringBuilder();
                    if (ColumnText != "PAY_EMP_CODE")
                    {
                        string DataType = dtComponent.Rows[i]["COMPONENT_DATATYPE"].ToString().Trim().ToUpper();
                        string Max = dtComponent.Rows[i]["Max_Length"].ToString().Trim().ToUpper();

                        switch (DataType)
                        {
                            case "TEXT":
                                {
                                    SBColumnDef.Append(ColumnText).Append(" nvarchar(").Append(Max).Append(")");
                                    break;
                                }
                            case "MASTER":
                                {
                                    SBColumnDef.Append(ColumnText).Append(" int");
                                    break;
                                }
                            case "DATETIME":
                                {
                                    SBColumnDef.Append(ColumnText).Append(" datetime");
                                    break;
                                }
                            case "AMOUNT":
                                {
                                    SBColumnDef.Append(ColumnText).Append(" numeric(16,2)");
                                    break;
                                }
                            case "NUMBER":
                                {
                                    SBColumnDef.Append(ColumnText).Append(" int");
                                    break;
                                }
                            default:
                                {
                                    SBColumnDef.Append(ColumnText).Append(" nvarchar(").Append(Max).Append(")");
                                    break;
                                }
                        }

                        if (Filetype == "PAYMAST" && dsPublishStatus.Tables[2].Rows.Count > 0)
                        {
                            StringBuilder SBQry = new StringBuilder();
                            SBQry.Append("if not exists(select Name from sys.columns where object_ID = object_ID('" + TEMP_Table_Name + "')AND Name = '" + ColumnText + "')");
                            SBQry.Append(" BEGIN ");
                            SBQry.Append(" alter table ").Append(TEMP_Table_Name).Append(" ADD ").Append(SBColumnDef);
                            SBQry.Append(" END ");
                            SBQry.Append(" ELSE BEGIN ");
                            SBQry.Append(" ALTER TABLE  ").Append(TEMP_Table_Name).Append(" alter column ").Append(SBColumnDef);
                            SBQry.Append("; END select 1");
                            DbRes = Convert.ToInt32(DataLib.ExecuteScaler(SBQry.ToString(), CommandType.Text, null));
                        }
                        //SBDynColumn.Append(ColumnText).Append(SBDynColumn).Append("),");
                        SBDynColumn.Append(SBColumnDef).Append(",");
                    }
                }

                if (Filetype == "HRDMAST")
                {
                    string StrHRDCreateStatement = " Create table " + TEMP_Table_Name + "(" + SBHRDStatic.ToString() + SBDynColumn.ToString().TrimEnd(',') + ");";
                    string StrQry = "BEGIN TRAN BEGIN TRY if NOT exists(select Name from Sys.objects where Type='u' and name ='" + TEMP_Table_Name + "') BEGIN ";
                    StrQry = StrQry + StrHRDCreateStatement + "; COMMIT; select 1 ";
                    StrQry = StrQry + "END else select -1; END TRY BEGIN CATCH ROLLBACK;END CATCH";
                    DbRes = Convert.ToInt32(DataLib.ExecuteScaler(StrQry, CommandType.Text, null));
                }
                Res.IsSuccess = true;
                Res.Message = "Table published successfully.";
                if (DbRes == -1)
                    Res.Message = "This table is already published.";
                if (DbRes == 0)
                    Res.Message = "This table is already published.";
                return Res;
            }
            catch (Exception Ex)
            {
                string ExMsg = Ex.Message;
                throw;
            }
        }
        public Response DropTempColumn(int EID, int EntityCOMPID, SqlConnection Conn, SqlTransaction Trans)
        {
            Response Res = new Response();
            try
            {

                EntityComponentModel ObjComponent = new EntityComponentModel();
                ObjComponent.EID = EID;
                ObjComponent.EntityCOMPID = EntityCOMPID;

                //Now Generate DDL Statement Ivap_MAST_TEMP_32
                string TEMP_Table_Name = "Ivap_MAST_TEMP_" + EID;
                DataTable dtComponent = GetEntityComponent(ObjComponent, Conn, Trans);


                string ColumnText = dtComponent.Rows[0]["COMPONENT_NAME"].ToString().Trim().ToUpper();
                StringBuilder SBQry = new StringBuilder();
                SBQry.Append("if  exists(select Name from sys.columns where object_ID = object_ID('" + TEMP_Table_Name + "') AND Name = '" + ColumnText + "')");
                SBQry.Append(" BEGIN ");
                SBQry.Append(" alter table ").Append(TEMP_Table_Name).Append(" drop column ").Append(ColumnText);
                SBQry.Append(" END ");
                SBQry.Append("; select 1");
                int DbRes = Convert.ToInt32(DataLib.ExecuteScaler(SBQry.ToString(), CommandType.Text, null, Conn, Trans));
                Res.IsSuccess = true;
                Res.Message = "Column droped successfully.";
                return Res;
            }
            catch
            {
                throw;
            }
        }


        public Response AlterTempObject(int EID, int EntityCOMPID, SqlConnection Con, SqlTransaction Trans)
        {
            Response Res = new Response();
            try
            {

                EntityComponentModel ObjComponent = new EntityComponentModel();
                ObjComponent.EID = EID;
                ObjComponent.EntityCOMPID = EntityCOMPID;

                //Now Generate DDL Statement Ivap_TEMP_MAST_32
                string TEMP_Table_Name = "Ivap_MAST_TEMP_" + EID;

                DataTable dtComponent = GetEntityComponent(ObjComponent, Con, Trans);
                AlterTempHisObject(EID, Con, Trans, dtComponent);

                string DataType = dtComponent.Rows[0]["COMPONENT_DATATYPE"].ToString().Trim().ToUpper();
                string Max = "MAX";//dtComponent.Rows[0]["Max_Length"].ToString().Trim().ToUpper();
                string ColumnText = dtComponent.Rows[0]["COMPONENT_NAME"].ToString().Trim().ToUpper();
                StringBuilder SBAlterColumn = new StringBuilder();

                SBAlterColumn.Append(ColumnText).Append(" nvarchar(").Append(Max).Append(")");

                StringBuilder SBQry = new StringBuilder();
                SBQry.Append("if not exists(select Name from sys.columns where object_ID = object_ID('" + TEMP_Table_Name + "') AND Name = '" + ColumnText + "')");
                //if not exists(select Name from sys.columns where object_ID = object_ID('Ivap_HRD_MAST_1') AND Name = '')
                SBQry.Append(" BEGIN ");
                SBQry.Append(" alter table ").Append(TEMP_Table_Name).Append(" ADD ").Append(SBAlterColumn);
                SBQry.Append(" END ");
                SBQry.Append(" ELSE BEGIN ");
                SBQry.Append(" ALTER TABLE  ").Append(TEMP_Table_Name).Append(" alter column ").Append(SBAlterColumn);
                SBQry.Append("; END select 1");
                int DbRes = Convert.ToInt32(DataLib.ExecuteScaler(SBQry.ToString(), CommandType.Text, null, Con, Trans));
                Res.IsSuccess = true;
                Res.Message = "Table altered successfully.";
                Res.Data = Convert.ToString(DbRes);
                return Res;
            }
            catch
            {
                throw;
            }
        }
        public Response AlterTempHisObject(int EID, SqlConnection Con, SqlTransaction Trans, DataTable dt)
        {
            Response Res = new Response();
            try
            {
                //Now Generate DDL Statement Ivap_TEMP_MAST_32
                string His_Table_Name = "Ivap_TEMP_HIS_" + EID;
                DataTable dtComponent = dt;

                string DataType = dtComponent.Rows[0]["COMPONENT_DATATYPE"].ToString().Trim().ToUpper();
                string DataLength = dtComponent.Rows[0]["Max_Length"].ToString().Trim().ToUpper();
                string ColumnText = dtComponent.Rows[0]["COMPONENT_NAME"].ToString().Trim().ToUpper();
                //alter temp History table column name and datatype 
                StringBuilder SBHisAlterColumn = new StringBuilder();
                switch (DataType)
                {
                    case "TEXT":
                        {
                            SBHisAlterColumn.Append(ColumnText).Append(" varchar(").Append(DataLength).Append(")");
                            break;
                        }
                    case "MASTER":
                        {
                            SBHisAlterColumn.Append(ColumnText).Append(" int");
                            break;
                        }
                    case "DATETIME":
                        {
                            SBHisAlterColumn.Append(ColumnText).Append(" datetime");
                            break;
                        }
                    case "AMOUNT":
                        {
                            SBHisAlterColumn.Append(ColumnText).Append(" numeric(16,2)");
                            break;
                        }
                    case "NUMBER":
                        {
                            SBHisAlterColumn.Append(ColumnText).Append(" numeric(16,2)");
                            break;
                        }
                    default:
                        {
                            SBHisAlterColumn.Append(ColumnText).Append(" numeric(16,2)");
                            break;
                        }
                }


                StringBuilder SBQry = new StringBuilder();
                SBQry.Append("if not exists(select Name from sys.columns where object_ID = object_ID('" + His_Table_Name + "') AND Name = '" + ColumnText + "')");
                //if not exists(select Name from sys.columns where object_ID = object_ID('Ivap_HRD_MAST_1') AND Name = '')
                SBQry.Append(" BEGIN ");
                SBQry.Append(" alter table ").Append(His_Table_Name).Append(" ADD ").Append(SBHisAlterColumn);
                SBQry.Append(" END ");
                SBQry.Append(" ELSE BEGIN ");
                SBQry.Append(" ALTER TABLE  ").Append(His_Table_Name).Append(" alter column ").Append(SBHisAlterColumn);
                SBQry.Append("; END select 1");
                int DbRes = Convert.ToInt32(DataLib.ExecuteScaler(SBQry.ToString(), CommandType.Text, null, Con, Trans));
                Res.IsSuccess = true;
                Res.Message = "Table altered successfully.";
                Res.Data = Convert.ToString(DbRes);
                return Res;
            }
            catch
            {
                throw;
            }
        }

        #endregion PublishTempObject
        #region FileDetailCashCade
        public Response ResetEntityComponent(EntityComponentModel model)
        {
            Response Res = new Response();
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EntityCompID",model.EntityCOMPID),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("ResetEntityComponent", CommandType.StoredProcedure, parameters));
                if (result > 0)
                {
                    Res.Message = "Entity Component Reset successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }

                return Res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response UpdateFile_ComponentDetails(EntityComponentModel model, String TID)
        {
            Response Res = new Response();
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",TID),
                    new SqlParameter("@UID",model.CreatedBy),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("UpdateFileComponentDTLCashcade", CommandType.StoredProcedure, parameters));
                if (result > 0)
                {
                    Res.Message = "File Component Updated successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }

                return Res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion End FileDetailCashCade
    }
}