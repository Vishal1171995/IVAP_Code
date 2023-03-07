using ClosedXML.Excel;
using Ivap.Areas.Configuration.Models;
using Ivap.Repository;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Ivap.Areas.Configuration.Repository
{
    public class FileSetupRepo
    {
        public Response SetUpAddUpdateFileType(FileSetupModel model, SqlConnection conn, SqlTransaction trans)
        {
            Response Res = new Response();
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@FileID",model.FileID),
                    new SqlParameter("@ENTITY_ID",model.EID),
                    new SqlParameter("@FILE_TYPE",model.FILE_TYPE),
                    new SqlParameter("@FILE_NAME",model.FILE_NAME),
                    new SqlParameter("@FILE_DESC",model.FILE_DESC),
                    new SqlParameter("@ISACTIVE",model.IsActive),
                    new SqlParameter("@UID",model.CreatedBy),
                    new SqlParameter("@CATEGORY",model.CATEGORY),
                    new SqlParameter("@Transpose",model.Transpose),
                     new SqlParameter("@PayRollInputFile",model.PayRollInputFile),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateFileType_1", CommandType.StoredProcedure, parameters, con: conn, trans: trans));
                if (result > 0)
                {
                    Res.Message = "File type created successfully.";
                    Res.IsSuccess = true;
                    Res.Data = result.ToString();
                    return Res;
                }
                if (result == 0)
                {
                    Res.Message = "File type updated successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                else if (result == -1)
                {
                    Res.Message = "Failed!!! file name must be unique.";
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
        public Response AddUpdateFileType(FileSetupModel model)
        {
            SqlTransaction trans = null;
            SqlConnection con = null;
            Response Res = new Response();
            try
            {
                con = new SqlConnection(Ivap.Utils.DataLib.GetConnectionString());
                con.Open();
                trans = con.BeginTransaction();
                Res = SetUpAddUpdateFileType(model: model, trans: trans, conn: con);
                if (Res.IsSuccess)
                {
                    trans.Commit();
                }
                else
                {
                    trans.Rollback();
                }

                return Res;
            }
            catch
            {
                trans.Rollback();
                con.Dispose();
                throw;
            }
            finally
            {
                con.Dispose();
                trans.Dispose();
            }
        }


        public Response AddUpdateTranspose(TransposeModel model)
        {
            Response Res = new Response();

            if (model.Transpose_Field_Type == "DEFAULTVALUE")
            {
                model.Transpose_Component_Name = model.Transpose_File_Display_Name;
            }

            if (model.Transpose_Field_Type == "TRANSPOSEFIELD" || model.Transpose_Field_Type == "TRANSPOSEVALUE")
            {
                model.Transpose_Component_Name = model.Transpose_File_Display_Name;
                model.Transpose_Default_Value = " ";
            }
            if (model.Transpose_Field_Type == "COMPONENT")
            {
                model.Transpose_Default_Value = " ";
            }
            int testInt = model.Transpose_ISACTIVE ? 1 : 0;
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",model.TID),
                    new SqlParameter("@ENTITY_ID",model.EID),
                    new SqlParameter("@File_Id",model.Transpose_FileID),
                    new SqlParameter("@Field_Type",model.Transpose_Field_Type),
                    new SqlParameter("@Component_Name",model.Transpose_Component_Name),
                    new SqlParameter("@Display_Name",model.Transpose_File_Display_Name),
                    new SqlParameter("@Display_Order",model.Display_order),
                    new SqlParameter("@Default_Value",model.Transpose_Default_Value),
                    new SqlParameter("@UID",model.CreatedBy),
                     new SqlParameter("@IsActive",testInt),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateTranspose", CommandType.StoredProcedure, parameters));
                if (result > 0)
                {
                    Res.Message = "Transpose created successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                if (result == 0)
                {
                    Res.Message = "Transpose updated successfully.";
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

        public Response UpdateFileComponentMASTER(FileComponentModel model)
        {
            Response Res = new Response();
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",model.COMPONENTID),
                    new SqlParameter("@UID",model.CreatedBy),
                    new SqlParameter("@COMPONENT_COLUMN_NAME",model.Component_FieldName),
                    new SqlParameter("@COMPONENT_DISPLAY_NAME",model.COMPONENT_DISPLAY_NAME),
                    new SqlParameter("@ISACTIVE",model.IsActive),
                    new SqlParameter("@MANDATORY",model.MANDATORY),
                    new SqlParameter("@GL_Code",model.GL_Code ?? Convert.DBNull),
                    new SqlParameter("@PMS_CODE",model.PMS_CODE ?? Convert.DBNull),
                    new SqlParameter("@Extra_Input_Validation",model.Extra_Validation ?? Convert.DBNull),
                    new SqlParameter("@Regular_Expression",model.Expression ?? Convert.DBNull),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("UpdateFileComponent_MASTER_20_FEB", CommandType.StoredProcedure, parameters));
                if (result > 0)
                {
                    Res.Message = "File Component Updated successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                else
                {
                    Res.Message = "Display Name exists. Try another display name!!!";
                    Res.IsSuccess = true;
                    return Res;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response UpdateFile_Component(FileComponentModel model)
        {
            Response Res = new Response();
            if (model.COMPONENT_NAME == "EMP_CODE" || model.COMPONENT_NAME == "PAYDATE")
            {
                model.MANDATORY = true;
                model.MIN_LENGTH = 1;
                model.MAX_LENGTH = 50;
                model.IsActive = true;

            }
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",model.COMPONENTID),
                    new SqlParameter("@UID",model.CreatedBy),
                    new SqlParameter("@COMPONENT_DISPLAY_NAME",model.COMPONENT_DISPLAY_NAME),
                    new SqlParameter("@ISACTIVE",model.IsActive),
                    new SqlParameter("@MIN_LENGTH",model.MIN_LENGTH),
                    new SqlParameter("@MAX_LENGTH",model.MAX_LENGTH),
                    new SqlParameter("@MANDATORY",model.MANDATORY),
                     new SqlParameter("@GL_Code",model.GL_Code ?? Convert.DBNull),
                    new SqlParameter("@PMS_CODE",model.PMS_CODE ?? Convert.DBNull),
                     new SqlParameter("@Extra_Input_Validation",model.Extra_Validation ?? Convert.DBNull),
                    new SqlParameter("@Regular_Expression",model.Expression ?? Convert.DBNull),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("UpdateFileComponent_New_20_FEB", CommandType.StoredProcedure, parameters));
                if (result > 0)
                {
                    Res.Message = "File Component Updated successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                else
                {
                    Res.Message = "Display Name exists. Try another display name!!!";
                    Res.IsSuccess = true;
                    return Res;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetFileType(FileSetupModel objModel, string WFS_String = "")
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


        public DataTable GetTranspose(int FileID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@FILEID", FileID),
                };
                dt = DataLib.ExecuteDataTable("GetTranspose", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public DataTable GetTransposeComponent(int EID, int FileID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID",EID),
                    new SqlParameter("@FILE_ID",FileID),

                };
                dt = DataLib.ExecuteDataTable("GET_FILE_SETUP_UPLOAD_Transpose", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //
        public DataTable GetTransposeByID(int TID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID", TID),
                };
                dt = DataLib.ExecuteDataTable("GetTransposeByID", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable DownLoadFileComponent(int FileID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@FILEID", FileID),
                };
                dt = DataLib.ExecuteDataTable("DOWNLOAD_FILE_COMPONENT", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable SearchFileComponent(int EID, string Search_Text, string Entity_Component_ID, int FileID)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlParameter[] parameters = new SqlParameter[]
                {
                    //new SqlParameter("@COMPONENT_FILE_TYPE",COMPONENT_FILE_TYPE),
                    new SqlParameter("@Search_Text",Search_Text),
                    new SqlParameter("@EID",EID),
                     new SqlParameter("@Entity_Component_ID",Entity_Component_ID),
                     new SqlParameter("@FileID",FileID),
                };
                return DataLib.ExecuteDataTable("GetEntityComponent", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
        public Response SetAddUpdateFileCompDetail(FileSetupModel model, SqlTransaction trans, SqlConnection con)
        {
            Response res = new Response();
            try
            {
                foreach (var list in model.ENTITY_Component_ID)
                {
                    SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",0),
                    new SqlParameter("@FILE_ID",model.FileID),
                    new SqlParameter("@COMPONENT_ID",list),
                    new SqlParameter("@UID",model.CreatedBy),
                };
                    int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateFileCompDetail_12_FEB", CommandType.StoredProcedure, parameters, con: con, trans: trans));
                    if (result > 0)
                    {
                        res.IsSuccess = true;
                        res.Message = "Component added sucessfully";
                    }
                    else if (result == 0)
                    {
                        res.IsSuccess = true;
                        res.Message = "Component updated sucessfully";
                    }
                    else if (result == -1)
                    {
                        res.IsSuccess = false;
                        res.Message = "Failed!!! component must be unique.";
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response AddUpdateFileCompDetail(FileSetupModel model)
        {
            SqlTransaction trans = null;
            SqlConnection con = null;
            Response Res = new Response();
            try
            {
                con = new SqlConnection(Ivap.Utils.DataLib.GetConnectionString());
                con.Open();
                trans = con.BeginTransaction();
                Res = SetAddUpdateFileCompDetail(model: model, trans: trans, con: con);
                if (Res.IsSuccess)
                {
                    trans.Commit();
                }
                else
                {
                    trans.Rollback();
                }
                return Res;
            }
            catch
            {
                trans.Rollback();
                con.Dispose();
                throw;
            }
            finally
            {
                con.Dispose();
                trans.Dispose();
            }

        }
        public Response DeleteFileType(FileSetupModel model)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@FileID",model.FileID),
                    new SqlParameter("@EID",model.EID),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("DeleteFileType", CommandType.StoredProcedure, parameters));
                if (model.FileID > 0)
                {
                    res.IsSuccess = true;
                    res.Message = "File type deleted successfully.";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Response ResetFileComponent(int TID)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{

                    new SqlParameter("@TID",TID),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("ResetFileComponent", CommandType.StoredProcedure, parameters));
                if (result > 0)
                {
                    res.IsSuccess = true;
                    res.Message = "Reset successfully.";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetFileCompDtl(FileSetupModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@FILE_ID", objModel.FileID),
                    new SqlParameter("@EID", objModel.EID),
                };
                dt = DataLib.ExecuteDataTable("GetFileCompDtl", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable GetCategory(FileSetupModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{

                    new SqlParameter("@TID", objModel.EID),
                };

                dt = DataLib.ExecuteDataTable("GET_FILE_CATEGORY", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable PayRollInputFile(int EID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{

                    new SqlParameter("@EID",EID),
                };

                dt = DataLib.ExecuteDataTable("GetPayRollInputFile", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public DataTable GetValidation(FileComponentModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{

                    new SqlParameter("@TID", objModel.EID),
                };

                dt = DataLib.ExecuteDataTable("GET_Validation", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable GetFileComponent(int TID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID", TID),

                };
                dt = DataLib.ExecuteDataTable("GetFileComponent", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public FileComponentModel GetFileComponentByTID(int TID)
        {
            DataTable dt = new DataTable();
            FileComponentModel RtModel = new FileComponentModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID", TID),

                };
                dt = DataLib.ExecuteDataTable("GetFileComponent", CommandType.StoredProcedure, parameters);

                RtModel.COMPONENTID = TID;
                RtModel.FILE_ID = Convert.ToInt32(dt.Rows[0]["FILE_ID"].ToString());
                RtModel.COMPONENT_DATATYPE = dt.Rows[0]["COMPONENT_DATATYPE"].ToString();
                RtModel.COMPONENT_DISPLAY_NAME = dt.Rows[0]["COMPONENT_DISPLAY_NAME"].ToString();
                RtModel.COMPONENT_NAME = dt.Rows[0]["COMPONENT_NAME"].ToString();
                RtModel.Component_TableName = dt.Rows[0]["COMPONENT_TABLE_NAME"].ToString();
                RtModel.Component_FieldNameShow = dt.Rows[0]["COMPONENT_COLUMN_NAME"].ToString();
                RtModel.Component_FieldName = dt.Rows[0]["COMPONENT_COLUMN_NAME"].ToString();
                RtModel.COMPONENT_FILE_TYPE = dt.Rows[0]["COMPONENT_FILE_TYPE"].ToString();
                RtModel.IsActive = Convert.ToBoolean(dt.Rows[0]["ISACTIVE"]);
                RtModel.MAX_LENGTH = Convert.ToInt32(dt.Rows[0]["MAX_LENGTH"].ToString());
                RtModel.MIN_LENGTH = Convert.ToInt32(dt.Rows[0]["MIN_LENGTH"].ToString());
                RtModel.MAX_VALUE = Convert.ToInt32(dt.Rows[0]["MAX_LENGTH"].ToString());
                RtModel.MIN_VALUE = Convert.ToInt32(dt.Rows[0]["MIN_LENGTH"].ToString());
                RtModel.MANDATORY = Convert.ToBoolean(dt.Rows[0]["MANDATORY"]);
                RtModel.GL_Code = dt.Rows[0]["GL_Code"].ToString();
                RtModel.PMS_CODE = dt.Rows[0]["PMS_CODE"].ToString();
                RtModel.Extra_Validation = dt.Rows[0]["EXTRA_INPUT_VALIDATION"].ToString();
                RtModel.Expression = dt.Rows[0]["EXTRA_RG_EXPRESSION"].ToString();
                return RtModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetFileHis(int TID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@FID", TID),

                };
                dt = DataLib.ExecuteDataTable("GET_FILETYPE_HISTORY", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable GetFileComponentHis(int TID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID", TID),

                };
                dt = DataLib.ExecuteDataTable("GetFileComponentHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response DeleteFileCompDtl(int EID, string FileCompDtlIDs)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@FileCompDtlIDs",FileCompDtlIDs),
                    new SqlParameter("@EID",EID),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("DeleteFileCompDtl", CommandType.StoredProcedure, parameters));
                if (result > 0)
                {
                    res.IsSuccess = true;
                    res.Message = "File component details deleted successfully.";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response SetDisplayOrderTranspose_UP(int TID, int FileID, int EID)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",TID),
                    new SqlParameter("@FILE_ID",FileID),
                    new SqlParameter("@EID",EID),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("SetOrderTranspose_UP", CommandType.StoredProcedure, parameters));
                if (result == 0)
                {
                    res.IsSuccess = true;
                    res.Message = "Display order change sucessfully";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response SetOrderTranspose_Down(int TID, int FileID, int EID)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",TID),
                    new SqlParameter("@FILE_ID",FileID),
                    new SqlParameter("@EID",EID),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("SetOrderTranspose_Down", CommandType.StoredProcedure, parameters));
                if (result == 0)
                {
                    res.IsSuccess = true;
                    res.Message = "Display order change sucessfully";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response SetOrderFileCompDtl_UP(int TID, int FileID, int EID)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",TID),
                    new SqlParameter("@FILE_ID",FileID),
                    new SqlParameter("@EID",EID),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("SetOrderFileCompDtl_UP", CommandType.StoredProcedure, parameters));
                if (result == 0)
                {
                    res.IsSuccess = true;
                    res.Message = "Display order change sucessfully";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Response SetOrderFileCompDtl_Down(int TID, int FileID, int EID)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",TID),
                    new SqlParameter("@FILE_ID",FileID),
                    new SqlParameter("@EID",EID),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("SetOrderFileCompDtl_Down", CommandType.StoredProcedure, parameters));
                if (result == 0)
                {
                    res.IsSuccess = true;
                    res.Message = "Display order change sucessfully";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public Response SetDisplayOrder_FileCompDtl(List<FileDtlModel> Model, int EID)
        {
            int CountDisplay = 0;
            int CountOrder = 0;
            int count = 0;
            Response res = new Response();
            try
            {
                CountOrder = Model.GroupBy(p => p.Display_Order).Where(x => x.Count() > 1).Count();
                if (CountOrder > 0)
                {
                    res.Message = "Creation failed. Display Order must be unique.";
                    res.IsSuccess = false;
                    return res;
                }
                foreach (var Items in Model)
                {
                    SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",Items.TID),
                    new SqlParameter("@DISPLAY_ORDER",Items.Display_Order),
                    new SqlParameter("@EID",EID),
                };
                    int result = Convert.ToInt32(DataLib.ExecuteScaler("SetDisplayOrder_FileCompDtl", CommandType.StoredProcedure, parameters));
                    Response.operation opration = Response.operation.Update;
                    res = res.GetResponse(opration, "Display Order", result, "");
                }

                return res;
            }
            catch (Exception ex) { throw; }
        }

        public DataTable FileSetupSample(int EID, int FILE_ID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FILE_ID",FILE_ID),

                };
                dt = DataLib.ExecuteDataTable("GET_FILE_SETUP_SAMPLE", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public KendoGridUtils GetCommandButtonForGrid_For_FileSetup(string RouteName)
        {
            try
            {
                KendoGridUtils objKGrid = new KendoGridUtils();
                int CommandCount = 0;
                List<CommandButton> LstCommand = new List<CommandButton>();
                if (AuthorizationRepo.IsValidAction(RouteName, "UpdateAction"))
                {
                    CommandButton ObjEditButton = new CommandButton();
                    ObjEditButton.name = "Edit";
                    ObjEditButton.text = "";
                    ObjEditButton.click = "EditHandler";
                    ObjEditButton.iconClass = "kIcon kIconEdit ";
                    ObjEditButton.title = "Edit";
                    LstCommand.Add(ObjEditButton);
                    CommandCount++;

                    ObjEditButton = new CommandButton();
                    ObjEditButton.name = "Delete";
                    ObjEditButton.text = "";
                    ObjEditButton.click = "DeleteHandler";
                    ObjEditButton.iconClass = "kIcon kdelete";
                    ObjEditButton.title = "Delete";
                    LstCommand.Add(ObjEditButton);
                    CommandCount++;
                }

                if (AuthorizationRepo.IsValidAction(RouteName, "ViewAction"))
                {
                    CommandButton ObjViewButton = new CommandButton();
                    ObjViewButton.name = "ViewFileCompDtl";
                    ObjViewButton.text = "";
                    ObjViewButton.click = "EditHandler";
                    ObjViewButton.iconClass = "kIcon kIconView";
                    ObjViewButton.title = "View";
                    LstCommand.Add(ObjViewButton);
                    CommandCount++;


                    ObjViewButton = new CommandButton();
                    ObjViewButton.name = "ViewFileSetup";
                    ObjViewButton.text = "";
                    ObjViewButton.click = "ViewFileSetup";
                    ObjViewButton.iconClass = "kIcon ActionIconReset";
                    ObjViewButton.title = "ViewFileSetup";
                    LstCommand.Add(ObjViewButton);
                    CommandCount++;


                    ObjViewButton = new CommandButton();
                    ObjViewButton.name = "DownLoadSample";
                    ObjViewButton.text = "";
                    ObjViewButton.click = "DownLoadSample";
                    ObjViewButton.iconClass = "kIcon k-icon k-i-excel";
                    ObjViewButton.title = "ViewFileCompDtl";
                    LstCommand.Add(ObjViewButton);
                    CommandCount++;


                    //ObjViewButton = new CommandButton();
                    //ObjViewButton.name = "DownLoadFileComponent";
                    //ObjViewButton.text = "";
                    //ObjViewButton.click = "DownLoadFileComponent";
                    //ObjViewButton.iconClass = "kIcon ActionIconDownload"; 
                    //ObjViewButton.title = "ViewFileCompDtl";
                    //LstCommand.Add(ObjViewButton);
                    //CommandCount++;


                }

                if (CommandCount > 0)
                {
                    Command ObjCommand = new Command();
                    ObjCommand.title = "Action";
                    ObjCommand.width = 40;
                    ObjCommand.command = LstCommand;
                    objKGrid.Command = ObjCommand;
                    if (CommandCount > 1)
                        ObjCommand.width = 100;
                }
                return objKGrid;
            }
            catch
            {
                throw;
            }

        }

        public DataTable GetFileComponent(int TID, int EntityID = 0, int EntityCompID = 0, string CheckDetl = "")
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@FileDetID", TID),
                    new SqlParameter("@ENTITY_ID", EntityID),
                    new SqlParameter("@EntityCompID", EntityCompID),
                    new SqlParameter("@CheckDetl", CheckDetl),
                };
                dt = DataLib.ExecuteDataTable("GetFileComponent0214", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable GetFileTypeForWFSetting(FileSetupModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ENTITY_ID", objModel.EID),
                    new SqlParameter("@FILE_TYPE", objModel.FILE_TYPE),
                    new SqlParameter("@UID", objModel.CreatedBy),
                };
                dt = DataLib.ExecuteDataTable("GetFileTypeForWfSetting13", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable FileSetupSampleDownload(int EID, int FILE_ID)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@FILE_ID",FILE_ID),

                };
                dt = DataLib.ExecuteDataTable("GET_FILE_SETUP_SAMPLE_23_FEB", CommandType.StoredProcedure, parameters);
                return dt;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string DataTableToExcel_FileSample(DataTable dt)
        {
            try
            {
                string FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Date.DayOfYear + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".xlsx";
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                using (XLWorkbook wb = new XLWorkbook())
                {

                    IXLWorksheet sheet = wb.Worksheets.Add("Sample");

                    var j = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        j = i + 1;
                        sheet.Cell(1, j).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                        sheet.Cell(1, j).Value = dt.Rows[i]["COMPONENT_DISPLAY_NAME"].ToString();
                    }
                    for (int i = 1; i < 7; i++)
                    {
                        sheet.Cell(5, i).Value = "TopBorder = Thick; TopBorderColor = Black";
                        sheet.Cell(5, i).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                        sheet.Cell(5, i).Style.Border.TopBorderColor = XLColor.Black;
                        sheet.Cell(5, i).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1, 0.5);
                    }
                    var NoOfRow = dt.Rows.Count + 5;
                    for (int i = 1; i < 7; i++)
                    {

                        sheet.Cell(NoOfRow, i).Value = "BottomBorder = Thick; BottomBorderColor = Black";
                        sheet.Cell(NoOfRow, i).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                        sheet.Cell(NoOfRow, i).Style.Border.BottomBorderColor = XLColor.Black;
                    }
                    var NoOfwidth = 5;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sheet.Cell(5, 6).Value = "RightBorder = Thick; RightBorderColor = Black";
                        sheet.Cell(5, 6).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                        sheet.Cell(5, 6).Style.Border.RightBorderColor = XLColor.Black;

                        sheet.Cell(5, 1).Value = "LeftBorder = Thick; LeftBorderColor = Black";
                        sheet.Cell(5, 1).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                        sheet.Cell(5, 1).Style.Border.LeftBorderColor = XLColor.Black;

                        NoOfwidth++;
                        sheet.Cell(NoOfwidth, 6).Value = "RightBorder = Thick; RightBorderColor = Black";
                        sheet.Cell(NoOfwidth, 6).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                        sheet.Cell(NoOfwidth, 6).Style.Border.RightBorderColor = XLColor.Black;

                        sheet.Cell(NoOfwidth, 1).Value = "LeftBorder = Thick; LeftBorderColor = Black";
                        sheet.Cell(NoOfwidth, 1).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                        sheet.Cell(NoOfwidth, 1).Style.Border.LeftBorderColor = XLColor.Black;


                    }
                    sheet.Cell(5, 1).Value = "Name";
                    sheet.Cell(5, 2).Value = "Mandatory";
                    sheet.Cell(5, 3).Value = "Data Type";
                    sheet.Cell(5, 4).Value = "Min-Length";
                    sheet.Cell(5, 5).Value = "Max-Length";
                    sheet.Cell(5, 6).Value = "Date Format";
                    int ExlRow = 5;
                    j = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ExlRow = ExlRow + 1;
                        j = i + 1;
                        if (dt.Rows[i]["COMPONENT_DATATYPE"].ToString() == "MASTER" || dt.Rows[i]["COMPONENT_DATATYPE"].ToString() == "DATETIME")
                        {
                            sheet.Cell(ExlRow, 1).Value = dt.Rows[i]["COMPONENT_DISPLAY_NAME"].ToString();
                            sheet.Cell(ExlRow, 2).Value = dt.Rows[i]["Mandatory"].ToString();
                            sheet.Cell(ExlRow, 3).Value = dt.Rows[i]["COMPONENT_DATATYPE"].ToString();
                            sheet.Cell(ExlRow, 4).Value = "";
                            sheet.Cell(ExlRow, 5).Value = "";
                            if (dt.Rows[i]["COMPONENT_DATATYPE"].ToString() == "DATETIME")
                            {
                                sheet.Cell(ExlRow, 6).Value = dt.Rows[i]["DATE_FORMAT"].ToString();
                            }
                            else
                            {
                                sheet.Cell(ExlRow, 6).Value = "";
                            }
                        }
                        else
                        {
                            sheet.Cell(ExlRow, 1).Value = dt.Rows[i]["COMPONENT_DISPLAY_NAME"].ToString();
                            sheet.Cell(ExlRow, 2).Value = dt.Rows[i]["Mandatory"].ToString();
                            sheet.Cell(ExlRow, 3).Value = dt.Rows[i]["COMPONENT_DATATYPE"].ToString();
                            sheet.Cell(ExlRow, 4).Value = dt.Rows[i]["MIN_LENGTH"].ToString();
                            sheet.Cell(ExlRow, 5).Value = dt.Rows[i]["MAX_LENGTH"].ToString();
                            sheet.Cell(ExlRow, 6).Value = "";
                        }
                    }

                    wb.SaveAs(FilePath);

                }
                return FileName;
            }
            catch (Exception ex) { throw new Exception("DataTableToExcel"); }
        }


        public DataTable GetSpecialComponentName(int EID, string searchStr)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                     new SqlParameter("@EID", EID),
                new SqlParameter("@SEARCHSTRING", searchStr),

                };
                dt = DataLib.ExecuteDataTable("GetSpecialComponentName", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Response AddUpdateSpecialComponent(SpecialComponentModel model)
        {
            if (model.Special_Field_Type == "DEFAULT VALUED")
            {

            }
            if (model.Special_Field_Type == "LOOKUP VALUED")
            {

            }
            Response Res = new Response();
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",model.TID),
                    new SqlParameter("@FileID",model.FileID),
                    new SqlParameter("@Special_Field_Type",model.Special_Field_Type),
                    new SqlParameter("@Display_Name",model.Display_Name),
                    new SqlParameter("@Default_Value",model.Default_Value),
                    new SqlParameter("@LookUp_Field_Value",model.LookUp_Field_Value),
                     new SqlParameter("@UID",model.CreatedBy),
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateSpecialComponent", CommandType.StoredProcedure, parameters));
                if (result > 0)
                {
                    Res.Message = "Special Component created successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                if (result == 0)
                {
                    Res.Message = "Special Component  updated successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                if (result == -1)
                {
                    Res.Message = "Display Name Exist Please Try another name.";
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
        public SpecialComponentModel GetSpecialFileComponentTid(int TID)
        {
            DataTable dt = new DataTable();
            SpecialComponentModel RtModel = new SpecialComponentModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID", TID),

                };
                dt = DataLib.ExecuteDataTable("GetSpecialFileComponentTid", CommandType.StoredProcedure, parameters);

                RtModel.TID = TID;
                RtModel.FileID = Convert.ToInt32(dt.Rows[0]["FILE_ID"].ToString());

                RtModel.Default_Value = dt.Rows[0]["Spl_Field_Value"].ToString();

                RtModel.LookUp_Field_Value = dt.Rows[0]["COMPONENT_ID"].ToString();
                RtModel.LookUp_Name = dt.Rows[0]["COMPONENT_NAME"].ToString();
                RtModel.Special_Field_Type = dt.Rows[0]["Spl_Field_Type"].ToString();
                RtModel.Display_Name = dt.Rows[0]["COMPONENT_DISPLAY_NAME"].ToString();


                return RtModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SetWorkFlowForFileSetup(string fileType, int FileId, int EID)
        {
            WorkFlowSettingRepo ObjWrkRepo = new WorkFlowSettingRepo();
            WFSettingModel WfModel = new WFSettingModel();
            PAYRollWFSettingModel PyModel = new PAYRollWFSettingModel();
            if (fileType == "Payroll Input File")
            {
                WfModel.FILE_ID = FileId;
                WfModel.EID = EID;

                ObjWrkRepo.GetWorkFlowSetting(WfModel);

                WfModel.WorkflowSettingClientAdmin.ISCheck = true;
                WfModel.WorkflowSettingMaker.ISCheck = true;
                WfModel.WorkflowSettingMyndChcker.ISCheck = true;
                WfModel.WorkflowSettingMyndMaker.ISCheck = true;
                WfModel.WorkflowSettingChecker.ISCheck = true;

                ObjWrkRepo.SetWorkFlowSetting(WfModel);

            }
            if (fileType == "PMS Output File")
            {
                PyModel.FILE_ID = FileId;
                PyModel.EID = EID;

                ObjWrkRepo.GetPayRollWorkFlowSetting(PyModel);

                PyModel.WorkflowSettingChecker.ISCheck = true;
                PyModel.WorkflowSettingClientAdmin.ISCheck = true;
                PyModel.WorkflowSettingMaker.ISCheck = true;
                PyModel.WorkflowSettingMyndChcker.ISCheck = true;
                PyModel.WorkflowSettingMyndMaker.ISCheck = true;

                ObjWrkRepo.SetPayRollWorkFlowSetting(PyModel);
            }
        }

    }
}