using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Ivap.Areas.Configuration.Models;
using Ivap.Utils;

namespace Ivap.Areas.Configuration.Repository
{
    public class WorkFlowSettingRepo
    {
        public void GetWorkFlowSetting(WFSettingModel Model,SqlConnection con=null,SqlTransaction tran=null)
        {
            PMSWorkFlowSettingItem WSItem;
            string FileID = "%" + Model.FILE_ID + ",%";
            DataSet dsAllSetting = new DataSet();
            DataTable dtRoleWiseSetting;
            DataTable dtRoleWiseCount;
            try
            {
                SqlParameter[] P = new[] {
                    new SqlParameter("@FILE_ID", Model.FILE_ID),
                    new SqlParameter("@ENTITY_ID",Model.EID),
                     new SqlParameter("@StringFile_ID",FileID),
                };
                if (tran == null)
                {
                    dsAllSetting = DataLib.ExecuteDataSet("GetWorkFlowSetting31", CommandType.StoredProcedure, P);
                }
                else {
                    dsAllSetting = DataLib.ExecuteDataSet("GetWorkFlowSetting31", CommandType.StoredProcedure, P,con:con,trans:tran);
                }
                DataView dv = new DataView(dsAllSetting.Tables[0]);
                DataView Countdv= new DataView(dsAllSetting.Tables[1]);
                //Populate Maker List
                dv.RowFilter = "ROLEID=9";
                dtRoleWiseSetting = new DataTable();
                dtRoleWiseSetting = dv.ToTable();
                //for count
                Countdv.RowFilter = "ROLEID=9";
                dtRoleWiseCount = new DataTable();
                dtRoleWiseCount = Countdv.ToTable();
                //
                WSItem = new PMSWorkFlowSettingItem();
                WSItem.USER_ROLE = 9;
                WSItem.UserCount= dtRoleWiseCount.Rows.Count>0? Convert.ToInt32(dtRoleWiseCount.Rows[0]["USERCOUNT"]):0;
                WSItem.ORDERING = WFORDERING.Maker;
                WSItem.ISCheck = false;
                WSItem.CompleteCssClass = "";
                WSItem.Icon_Name = "cancel.png";
                if (dtRoleWiseSetting.Rows.Count > 0)
                {
                    WSItem.ISCheck = true;
                    WSItem.Icon_Name = "checkgreen_icon.png";
                    WSItem.CompleteCssClass = "is-complete";
                }

                Model.WorkflowSettingMaker = WSItem;

                //checker

                dv.RowFilter = "ROLEID=10";
                dtRoleWiseSetting = new DataTable();
                dtRoleWiseSetting = dv.ToTable();
                //for count
                Countdv.RowFilter = "ROLEID=10";
                dtRoleWiseCount = new DataTable();
                dtRoleWiseCount = Countdv.ToTable();
                //
                WSItem = new PMSWorkFlowSettingItem();
                WSItem.USER_ROLE = 10;
                WSItem.UserCount = dtRoleWiseCount.Rows.Count > 0 ? Convert.ToInt32(dtRoleWiseCount.Rows[0]["USERCOUNT"]) : 0;
                WSItem.ORDERING = WFORDERING.Checker;
                WSItem.ISCheck = false;
                WSItem.CompleteCssClass = "";
                WSItem.Icon_Name = "cancel.png";
                if (dtRoleWiseSetting.Rows.Count > 0)
                {
                    WSItem.ISCheck = true;
                    WSItem.Icon_Name = "checkgreen_icon.png";
                    WSItem.CompleteCssClass = "is-complete";
                }
                
                Model.WorkflowSettingChecker = WSItem;
                //Mynd Maker
                dv.RowFilter = "ROLEID=2";
                dtRoleWiseSetting = new DataTable();
                dtRoleWiseSetting = dv.ToTable();
                //for count
                Countdv.RowFilter = "ROLEID=2";
                dtRoleWiseCount = new DataTable();
                dtRoleWiseCount = Countdv.ToTable();
                //
                WSItem = new PMSWorkFlowSettingItem();
                WSItem.USER_ROLE = 2;
                WSItem.UserCount = dtRoleWiseCount.Rows.Count > 0 ? Convert.ToInt32(dtRoleWiseCount.Rows[0]["USERCOUNT"]) : 0;
                WSItem.ORDERING = WFORDERING.MyndMaker;
                WSItem.ISCheck = false;
                WSItem.CompleteCssClass = "";
                WSItem.Icon_Name = "cancel.png";
                if (dtRoleWiseSetting.Rows.Count > 0)
                {
                    WSItem.ISCheck = true;
                    WSItem.Icon_Name = "checkgreen_icon.png";
                    WSItem.CompleteCssClass = "is-complete";
                }
                    
                Model.WorkflowSettingMyndMaker = WSItem;
                //Client Admin
                dv.RowFilter = "ROLEID=8";
                dtRoleWiseSetting = new DataTable();
                dtRoleWiseSetting = dv.ToTable();
                //for count
                Countdv.RowFilter = "ROLEID=8";
                dtRoleWiseCount = new DataTable();
                dtRoleWiseCount = Countdv.ToTable();
                //
                WSItem = new PMSWorkFlowSettingItem();

                WSItem.USER_ROLE = 8;
                WSItem.UserCount = dtRoleWiseCount.Rows.Count > 0 ? Convert.ToInt32(dtRoleWiseCount.Rows[0]["USERCOUNT"]) : 0;
                WSItem.ORDERING = WFORDERING.ClientAdmin;
                WSItem.ISCheck = false;
                WSItem.CompleteCssClass = "";
                WSItem.Icon_Name = "cancel.png";
                if (dtRoleWiseSetting.Rows.Count > 0)
                {
                    WSItem.ISCheck = true;
                    WSItem.Icon_Name = "checkgreen_icon.png";
                    WSItem.CompleteCssClass = "is-complete";
                }
                
                Model.WorkflowSettingClientAdmin = WSItem;
                //Mynd Checker
                dv.RowFilter = "ROLEID=3";
                dtRoleWiseSetting = new DataTable();
                dtRoleWiseSetting = dv.ToTable();
                //for count
                Countdv.RowFilter = "ROLEID=3";
                dtRoleWiseCount = new DataTable();
                dtRoleWiseCount = Countdv.ToTable();
                //
                WSItem = new PMSWorkFlowSettingItem();

                WSItem.USER_ROLE = 3;
                WSItem.UserCount = dtRoleWiseCount.Rows.Count > 0 ? Convert.ToInt32(dtRoleWiseCount.Rows[0]["USERCOUNT"]) : 0;
                WSItem.ORDERING = WFORDERING.MyndChcker;
                WSItem.ISCheck = false;
                WSItem.CompleteCssClass = "";
                WSItem.Icon_Name = "cancel.png";
                if (dtRoleWiseSetting.Rows.Count > 0)
                {
                    WSItem.ISCheck = true;
                    WSItem.Icon_Name = "checkgreen_icon.png";
                    WSItem.CompleteCssClass = "is-complete";
                }
                Model.WorkflowSettingMyndChcker = WSItem;

                
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GETUserRoleWise(int RoleID, WFSettingModel Model)
        {
            DataTable dt = new DataTable();
            string FileID = "%" + Model.FILE_ID + ",%";
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@RoleID", RoleID),
                    new SqlParameter("@EntityID", Model.EID),
                    new SqlParameter("@File_ID", Model.FILE_ID),
                      new SqlParameter("@StringFile_ID", FileID),

                };
                dt = DataLib.ExecuteDataTable("GetUserRoleWise", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Response SetWorkFlowSetting(WFSettingModel model)
        {
            SqlTransaction trans = null;
            SqlConnection con = null;
            Response Res = new Response();
            try
            {
                con = new SqlConnection(Ivap.Utils.DataLib.GetConnectionString());
                con.Open();
                trans = con.BeginTransaction();
                Res = AddUpdateWorkFlowSetting(model, con, trans);
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
        public Response AddUpdateWorkFlowSetting(WFSettingModel Model, SqlConnection Con, SqlTransaction Trans)
        {
            int result = 0;
            Response res = new Response();
           // res.IsSuccess = true;
            try
            {

                res = AddUpdate(Model.WorkflowSettingChecker, Model, Con, Trans);
                res = AddUpdate(Model.WorkflowSettingMaker, Model, Con, Trans);
                res = AddUpdate(Model.WorkflowSettingMyndChcker, Model, Con, Trans);
                res = AddUpdate(Model.WorkflowSettingMyndMaker, Model, Con, Trans);
                res = AddUpdate(Model.WorkflowSettingClientAdmin, Model, Con, Trans);

                if (res.IsSuccess == true)
                {
                    res.Message = "Work Flow Setting submitted successfully";
                    res.IsSuccess = true;
                }
                else
                {
                    res.Message = "some thing went wrong!";
                    res.IsSuccess = false;
                }
                return res;
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        public Response AddUpdate(PMSWorkFlowSettingItem Item, WFSettingModel Model, SqlConnection Con, SqlTransaction Trans)
        {
            int Result = 0;
            Response res = new Response();
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ENTITY_ID",Model.EID),
                    new SqlParameter("@FILE_ID",Model.FILE_ID),
                    new SqlParameter("@USER_ROLE",Item.USER_ROLE),
                    new SqlParameter("@ORDERING",(int)Item.ORDERING),
                       new SqlParameter("@CREATED_BY",Model.CreatedBy),
                    new SqlParameter("@ISChek",Item.ISCheck),

                    };
                    Result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateWorkFlowSetting", CommandType.StoredProcedure, parameters, Con, Trans));
                if (Result >= 0)
                {
                    res.IsSuccess = true;
                }
                else
                {
                    res.IsSuccess = false;
                }
            }
            catch
            {
                res.IsSuccess = false;
                throw;
            }
            return res;
        }
        #region PAYROLL WORKFLOW
        public void GetPayRollWorkFlowSetting(PAYRollWFSettingModel Model,SqlConnection con= null,SqlTransaction trans= null)
        {
            PAYRollWorkFlowSettingItem WSItem;
            string FileID = "%" + Model.FILE_ID + ",%";
            DataSet dsAllSetting = new DataSet();
            DataTable dtRoleWiseSetting;
            DataTable dtRoleWiseCount;
            try
            {
                SqlParameter[] P = new[] {
                    new SqlParameter("@FILE_ID", Model.FILE_ID),
                    new SqlParameter("@ENTITY_ID",Model.EID),
                     new SqlParameter("@StringFile_ID",FileID),
                };
                if (trans == null)
                {
                    dsAllSetting = DataLib.ExecuteDataSet("GetWorkFlowSetting31", CommandType.StoredProcedure, P);
                }
                else {
                    dsAllSetting = DataLib.ExecuteDataSet("GetWorkFlowSetting31", CommandType.StoredProcedure, P,con:con,trans:trans);
                }
                DataView dv = new DataView(dsAllSetting.Tables[0]);
                DataView Countdv = new DataView(dsAllSetting.Tables[1]);
                //Populate Maker List
                dv.RowFilter = "ROLEID=9";
                dtRoleWiseSetting = new DataTable();
                dtRoleWiseSetting = dv.ToTable();
                //for count
                Countdv.RowFilter = "ROLEID=9";
                dtRoleWiseCount = new DataTable();
                dtRoleWiseCount = Countdv.ToTable();
                //
                WSItem = new PAYRollWorkFlowSettingItem();
                WSItem.USER_ROLE = 9;
                WSItem.UserCount = dtRoleWiseCount.Rows.Count > 0 ? Convert.ToInt32(dtRoleWiseCount.Rows[0]["USERCOUNT"]) : 0;
                WSItem.ORDERING = PAYRoll_WFORDERING.Maker;
                WSItem.ISCheck = false;
                WSItem.CompleteCssClass = "";
                WSItem.Icon_Name = "cancel.png";
                if (dtRoleWiseSetting.Rows.Count > 0)
                {
                    WSItem.ISCheck = true;
                    WSItem.Icon_Name = "checkgreen_icon.png";
                    WSItem.CompleteCssClass = "is-complete";
                }

                Model.WorkflowSettingMaker = WSItem;

                //checker
                //        is-complete
                //checkgreen_icon.png

                dv.RowFilter = "ROLEID=10";
                dtRoleWiseSetting = new DataTable();
                dtRoleWiseSetting = dv.ToTable();
                //for count
                Countdv.RowFilter = "ROLEID=10";
                dtRoleWiseCount = new DataTable();
                dtRoleWiseCount = Countdv.ToTable();
                //
                WSItem = new PAYRollWorkFlowSettingItem();
                WSItem.USER_ROLE = 10;
                WSItem.UserCount = dtRoleWiseCount.Rows.Count > 0 ? Convert.ToInt32(dtRoleWiseCount.Rows[0]["USERCOUNT"]) : 0;
                WSItem.ORDERING = PAYRoll_WFORDERING.Checker;
                WSItem.ISCheck = false;
                WSItem.CompleteCssClass = "";
                WSItem.Icon_Name = "cancel.png";
                if (dtRoleWiseSetting.Rows.Count > 0)
                {
                    WSItem.ISCheck = true;
                    WSItem.Icon_Name = "checkgreen_icon.png";
                    WSItem.CompleteCssClass = "is-complete";
                }

                Model.WorkflowSettingChecker = WSItem;
                //Mynd Maker
                dv.RowFilter = "ROLEID=2";
                dtRoleWiseSetting = new DataTable();
                dtRoleWiseSetting = dv.ToTable();
                //for count
                Countdv.RowFilter = "ROLEID=2";
                dtRoleWiseCount = new DataTable();
                dtRoleWiseCount = Countdv.ToTable();
                //
                WSItem = new PAYRollWorkFlowSettingItem();
                WSItem.USER_ROLE = 2;
                WSItem.UserCount = dtRoleWiseCount.Rows.Count > 0 ? Convert.ToInt32(dtRoleWiseCount.Rows[0]["USERCOUNT"]) : 0;
                WSItem.ORDERING = PAYRoll_WFORDERING.MyndMaker;
                WSItem.ISCheck = false;
                WSItem.CompleteCssClass = "";
                WSItem.Icon_Name = "cancel.png";
                if (dtRoleWiseSetting.Rows.Count > 0)
                {
                    WSItem.ISCheck = true;
                    WSItem.Icon_Name = "checkgreen_icon.png";
                    WSItem.CompleteCssClass = "is-complete";
                }

                Model.WorkflowSettingMyndMaker = WSItem;
                //Client Admin
                dv.RowFilter = "ROLEID=8";
                dtRoleWiseSetting = new DataTable();
                dtRoleWiseSetting = dv.ToTable();
                //for count
                Countdv.RowFilter = "ROLEID=8";
                dtRoleWiseCount = new DataTable();
                dtRoleWiseCount = Countdv.ToTable();
                //
                WSItem = new PAYRollWorkFlowSettingItem();

                WSItem.USER_ROLE = 8;
                WSItem.UserCount = dtRoleWiseCount.Rows.Count > 0 ? Convert.ToInt32(dtRoleWiseCount.Rows[0]["USERCOUNT"]) : 0;
                WSItem.ORDERING = PAYRoll_WFORDERING.ClientAdmin;
                WSItem.ISCheck = false;
                WSItem.CompleteCssClass = "";
                WSItem.Icon_Name = "cancel.png";
                if (dtRoleWiseSetting.Rows.Count > 0)
                {
                    WSItem.ISCheck = true;
                    WSItem.Icon_Name = "checkgreen_icon.png";
                    WSItem.CompleteCssClass = "is-complete";
                }

                Model.WorkflowSettingClientAdmin = WSItem;
                //Mynd Checker
                dv.RowFilter = "ROLEID=3";
                dtRoleWiseSetting = new DataTable();
                dtRoleWiseSetting = dv.ToTable();
                //for count
                Countdv.RowFilter = "ROLEID=3";
                dtRoleWiseCount = new DataTable();
                dtRoleWiseCount = Countdv.ToTable();
                //
                WSItem = new PAYRollWorkFlowSettingItem();

                WSItem.USER_ROLE = 3;
                WSItem.UserCount = dtRoleWiseCount.Rows.Count > 0 ? Convert.ToInt32(dtRoleWiseCount.Rows[0]["USERCOUNT"]) : 0;
                WSItem.ORDERING = PAYRoll_WFORDERING.MyndChcker;
                WSItem.ISCheck = false;
                WSItem.CompleteCssClass = "";
                WSItem.Icon_Name = "cancel.png";
                if (dtRoleWiseSetting.Rows.Count > 0)
                {
                    WSItem.ISCheck = true;
                    WSItem.Icon_Name = "checkgreen_icon.png";
                    WSItem.CompleteCssClass = "is-complete";
                }
                Model.WorkflowSettingMyndChcker = WSItem;


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Response SetPayRollWorkFlowSetting(PAYRollWFSettingModel model)
        {
            SqlTransaction trans = null;
            SqlConnection con = null;
            Response Res = new Response();
            try
            {
                con = new SqlConnection(Ivap.Utils.DataLib.GetConnectionString());
                con.Open();
                trans = con.BeginTransaction();
                Res = PayRollAddUpdateWorkFlowSetting(model, con, trans);
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
        public Response PayRollAddUpdateWorkFlowSetting(PAYRollWFSettingModel Model, SqlConnection Con, SqlTransaction Trans)
        {
            bool IsChecked = false;
            int UserCount = 0;
            
            Response res = new Response();
            try
            {
                IsChecked = Model.WorkflowSettingChecker.ISCheck;
                UserCount = Model.WorkflowSettingChecker.UserCount;
                //
                IsChecked = Model.WorkflowSettingChecker.ISCheck;
                UserCount = Model.WorkflowSettingChecker.UserCount;

                res = PayRollAddUpdate(Model.WorkflowSettingChecker, Model, Con, Trans);
                res = PayRollAddUpdate(Model.WorkflowSettingMaker, Model, Con, Trans);
                res = PayRollAddUpdate(Model.WorkflowSettingMyndChcker, Model, Con, Trans);
                res = PayRollAddUpdate(Model.WorkflowSettingMyndMaker, Model, Con, Trans);
                res = PayRollAddUpdate(Model.WorkflowSettingClientAdmin, Model, Con, Trans);
                if (res.IsSuccess == true)
                {
                    res.Message = "Work Flow Setting submitted successfully";
                    res.IsSuccess = true;
                }
                else
                {
                    res.Message = "some thing went wrong!";
                    res.IsSuccess = false;
                }
                return res;
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        public Response PayRollAddUpdate(PAYRollWorkFlowSettingItem Item, PAYRollWFSettingModel Model, SqlConnection Con, SqlTransaction Trans)
        {
            int Result = 0;
            Response res = new Response();
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ENTITY_ID",Model.EID),
                    new SqlParameter("@FILE_ID",Model.FILE_ID),
                    new SqlParameter("@USER_ROLE",Item.USER_ROLE),
                    new SqlParameter("@ORDERING",(int)Item.ORDERING),
                       new SqlParameter("@CREATED_BY",Model.CreatedBy),
                    new SqlParameter("@ISChek",Item.ISCheck),

                    };
                Result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateWorkFlowSetting", CommandType.StoredProcedure, parameters, Con, Trans));
                if (Result >= 0)
                {
                    res.IsSuccess = true;
                }
                else
                {
                    res.IsSuccess = false;
                }
            }
            catch
            {
                res.IsSuccess = false;
                throw;
            }
            return res;
        }
        #endregion PAYROLL
    }
}