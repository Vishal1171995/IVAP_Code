using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ivap.Utils;
using Ivap.Areas.Master.ViewModel;
using System.Data;
using System.Data.SqlClient;
using IVap.Areas.Master.Repository;
using System.Configuration;
using Ivap.Areas.Master.Models;
using System.Text;
using Ivap.Factory;

namespace Ivap.Areas.Master.Repository
{
    public class EntityRepo
    {

        public DataTable GetEntity(EntityModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EntityID", objModel.TID),
                    new SqlParameter("@IsAct", "1"),
                };
                dt = DataLib.ExecuteDataTable("GetEntity_001", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch
            {
                throw;
            }
        }

        public Response SetUpEntity(EntityOnBoardVM Model) {
            Response res = new Response();
            SqlTransaction trans = null;
            SqlConnection con = null;
            try
            {
                DefaultAbstractClass objAccess = new EntityClass();
                res= objAccess.Run(Model);
                if (res.IsSuccess) {
                    StringBuilder mailbody = new StringBuilder("");
                    mailbody.Append(" <body><header style=\"width:996px; margin:auto;\"><div style=\"font-family:Arial, Helvetica, sans-serif; color:#444; font-size:18px;font-weight:bold;\"> ");
                    mailbody.Append(" Dear " + Model.EntityUser.FirstName + ",</div></header><section style=\"width:996px; margin:auto;\"><div style=\"color:#333;font-size:14px;font-family:Arial, Helvetica, sans-serif;\"><p>Entity has been created on IVAP portal.<br/><b>Entity Name:</b>" + Model.EntityModel.ENTITY_NAME + "<br/><b>User Name:</b>" + Model.EntityUser.UserID + " < br /><b>Password:</b>" + Model.EntityUser.UserID + "<br/>");
                    mailbody.Append(" </section> <footer style=\"width:996px; margin:auto;\"> <div style=\"color:#333;font-size:14px;font-family:Arial, Helvetica, sans-serif;padding:5px 0px;\">Thanks!<br> Team IVAP</div> ");
                    mailbody.Append(" </footer></body>");
                    MailUtils.SendMail(Model.EntityUser.Email, "Entity creation alert!!", mailbody.ToString(), "", "", "");
                }
                //res = CreateEntity(Model: Model,trans:trans,conn:con);
                //if (res.IsSuccess)
                //{
                //    trans.Commit();
                //    StringBuilder mailbody = new StringBuilder("");
                //    mailbody.Append(" <body><header style=\"width:996px; margin:auto;\"><div style=\"font-family:Arial, Helvetica, sans-serif; color:#444; font-size:18px;font-weight:bold;\"> ");
                //    mailbody.Append(" Dear " + Model.EntityUser.FirstName + ",</div></header><section style=\"width:996px; margin:auto;\"><div style=\"color:#333;font-size:14px;font-family:Arial, Helvetica, sans-serif;\"><p>Entity has been created on IVAP portal.<br/><b>Entity Name:</b>" + Model.EntityModel.ENTITY_NAME + "<br/><b>User Name:</b>" + Model.EntityUser.UserID + " < br /><b>Password:</b>" + Model.EntityUser.UserID + "<br/>");
                //    mailbody.Append(" </section> <footer style=\"width:996px; margin:auto;\"> <div style=\"color:#333;font-size:14px;font-family:Arial, Helvetica, sans-serif;padding:5px 0px;\">Thanks!<br> Team IVAP</div> ");
                //    mailbody.Append(" </footer></body>");
                //    MailUtils.SendMail(Model.EntityUser.Email, "Entity creation alert!!", mailbody.ToString(), "", "", "");
                //}
                //else {
                //    trans.Rollback();
                //}
                return res;
            }
            catch(Exception ex) {
                trans.Rollback();
                con.Dispose();
                throw;
            }
        }
        public Response CreateEntity(EntityOnBoardVM Model,SqlConnection conn,SqlTransaction trans)
        {
            Response Res = new Response();
            try
            {
                //Insert Data into Entity Table
                SqlParameter[] PEntity = new SqlParameter[]
                {
                    new SqlParameter("@ENTITY_NAME",Model.EntityModel.ENTITY_NAME),
                    new SqlParameter("@ENTITY_ADDR1",Model.EntityModel.ENTITY_ADDR1),
                    new SqlParameter("@ENTITY_ADDR2",Model.EntityModel.ENTITY_ADDR2),
                    new SqlParameter("@ENTITY_CITY",Model.EntityModel.ENTITY_CITY),
                    new SqlParameter("@ENTITY_STATE",Model.EntityModel.ENTITY_STATE),
                    new SqlParameter("@ENTITY_PIN",Model.EntityModel.ENTITY_PIN),
                    new SqlParameter("@CREATED_BY",Model.EntityModel.CreatedBy),
                    new SqlParameter("@DOMAIN_NAME",Model.EntityModel.DOMAIN_NAME),
                    new SqlParameter("@CURRENCY",Model.EntityModel.ENTITY_Currency),
                    new SqlParameter("@DATE_FORMAT",Model.EntityModel.DATE_FORMAT),
                    new SqlParameter("@COUNTRY",Model.EntityModel.ENTITY_Currency),
                    new SqlParameter("@PAY_PERIOD",Model.EntityModel.ENTITY_Payperiod),
                    new SqlParameter("@Payperiod_Weekly_Fr",Model.ENTITY_Payperiod_Weekly_Fr),
                    new SqlParameter("@Payperiod_Weekly_To",Model.ENTITY_Payperiod_Weekly_To),
                    new SqlParameter("@Payperiod_Monthly_Fr",Model.ENTITY_Payperiod_Monthly_Fr),
                    new SqlParameter("@Payperiod_Monthly_To",Model.ENTITY_Payperiod_Monthly_To),
                    new SqlParameter("@Payperiod_Fortnightly_Fr1",Model.ENTITY_Payperiod_Fortnightly_Fr1),
                    new SqlParameter("@Payperiod_Fortnightly_To1",Model.ENTITY_Payperiod_Fortnightly_To1),
                    new SqlParameter("@Payperiod_Fortnightly_Fr2",Model.ENTITY_Payperiod_Fortnightly_Fr2),
                    new SqlParameter("@Payperiod_Fortnightly_To2",Model.ENTITY_Payperiod_Fortnightly_To2),

                    //new SqlParameter("@Payperiod_Weekly_Fr",Model.EntityModel.ENTITY_Payperiod_Weekly_Fr),
                    //new SqlParameter("@Payperiod_Weekly_To",Model.EntityModel.ENTITY_Payperiod_Weekly_To),
                    //new SqlParameter("@Payperiod_Monthly_Fr",Model.EntityModel.ENTITY_Payperiod_Monthly_Fr),
                    //new SqlParameter("@Payperiod_Monthly_To",Model.EntityModel.ENTITY_Payperiod_Monthly_To),
                    //new SqlParameter("@Payperiod_Fortnightly_Fr1",Model.EntityModel.ENTITY_Payperiod_Fortnightly_Fr1),
                    //new SqlParameter("@Payperiod_Fortnightly_To1",Model.EntityModel.ENTITY_Payperiod_Fortnightly_To1),
                    //new SqlParameter("@Payperiod_Fortnightly_Fr2",Model.EntityModel.ENTITY_Payperiod_Fortnightly_Fr2),
                    //new SqlParameter("@Payperiod_Fortnightly_To2",Model.EntityModel.ENTITY_Payperiod_Fortnightly_To2),

                    new SqlParameter("@Services_Availed",Model.EntityModel.ENTITY_Services_Availed)
                };
                 int EntityRes = Convert.ToInt32(DataLib.ExecuteScaler("CreateEntity_1",CommandType.StoredProcedure,PEntity,conn,trans));
                if(EntityRes==0)
                {
                    Res.IsSuccess = false;
                    Res.Message = "Entity creation failed.Entity name must be unique.";
                    return Res;
                }
                if (EntityRes == -1)
                {
                    Res.IsSuccess = false;
                    Res.Message = "Entity creation failed.Domain name must be unique.";
                    return Res;
                }
                //Now Create User Of Concern Entity
                UserRepo ObjURepo = new UserRepo();
                Model.EntityUser.EID = EntityRes;
                Model.EntityUser.Role = 8;
                Model.EntityUser.Password= HashingLib.GenerateSHA512String(Model.EntityUser.Password);
                var UserRes = ObjURepo.UserSignUp(Model.EntityUser,conn,trans);
                //Now Get Default Menu from Database

                DataTable dtDefaultMenu = DataLib.ExecuteDataTable("GetDefaultMenu", CommandType.StoredProcedure, null);

                //Now Insert Default Menu into DataBase
                //Get All DefaultMenu 
                DataView dvPMenu = new DataView(dtDefaultMenu);
                dvPMenu.RowFilter = "PMENU=0";
                DataTable dtParentMenu = dvPMenu.ToTable();
                for(int i=0;i<dtParentMenu.Rows.Count;i++)
                {
                    SqlParameter[] P1 = new SqlParameter[]
                    {
                        new SqlParameter("@Entity_ID",EntityRes),
                        new SqlParameter("@NAME",dtParentMenu.Rows[i]["NAME"].ToString()),
                        new SqlParameter("@PMENU","0"),
                        new SqlParameter("@ROLES",dtParentMenu.Rows[i]["ROLES"].ToString()),
                        new SqlParameter("@DISPLAY_ORDER",dtParentMenu.Rows[i]["DISPLAY_ORDER"].ToString()),
                        new SqlParameter("@ACTIONS_NAME",dtParentMenu.Rows[i]["ACTIONS_NAME"].ToString()),
                        new SqlParameter("@IMAGE",dtParentMenu.Rows[i]["IMAGE"].ToString()),
                        new SqlParameter("@CONTROLLER",dtParentMenu.Rows[i]["CONTROLLER"].ToString()),
                        new SqlParameter("@MENU_TYPE",dtParentMenu.Rows[i]["MENU_TYPE"].ToString()),
                        new SqlParameter("@Route",dtParentMenu.Rows[i]["Route"].ToString())
                    };

                    int Pmenu2 = Convert.ToInt32(DataLib.ExecuteScaler("InsertDefaultMenu", CommandType.StoredProcedure, P1,con:conn,trans:trans));
                    DataView dvPMenu2 = new DataView(dtDefaultMenu);
                    dvPMenu2.RowFilter = "PMENU="+ dtParentMenu.Rows[i]["TID"];
                    DataTable dtParentMenu2 = dvPMenu2.ToTable();
                    for (int j = 0; j < dtParentMenu2.Rows.Count; j++)
                    {
                        SqlParameter[] P2 = new SqlParameter[]
                        {
                        new SqlParameter("@Entity_ID",EntityRes),
                        new SqlParameter("@NAME",dtParentMenu2.Rows[j]["NAME"].ToString()),
                        new SqlParameter("@PMENU",Pmenu2),
                        new SqlParameter("@ROLES",dtParentMenu2.Rows[j]["ROLES"].ToString()),
                        new SqlParameter("@DISPLAY_ORDER",dtParentMenu2.Rows[j]["DISPLAY_ORDER"].ToString()),
                        new SqlParameter("@ACTIONS_NAME",dtParentMenu2.Rows[j]["ACTIONS_NAME"].ToString()),
                        new SqlParameter("@IMAGE",dtParentMenu2.Rows[j]["IMAGE"].ToString()),
                        new SqlParameter("@CONTROLLER",dtParentMenu2.Rows[j]["CONTROLLER"].ToString()),
                        new SqlParameter("@MENU_TYPE",dtParentMenu2.Rows[j]["MENU_TYPE"].ToString()),
                        new SqlParameter("@Route",dtParentMenu2.Rows[j]["Route"].ToString())
                        };

                        int Pmenu3 = Convert.ToInt32(DataLib.ExecuteScaler("InsertDefaultMenu", CommandType.StoredProcedure, P2,conn,trans));

                        DataView dvPMenu3 = new DataView(dtDefaultMenu);
                        dvPMenu3.RowFilter = "PMENU=" + dtParentMenu2.Rows[j]["TID"];
                        DataTable dtParentMenu3 = dvPMenu3.ToTable();
                        for (int k = 0; k < dtParentMenu3.Rows.Count; k++)
                        {
                            SqlParameter[] P3 = new SqlParameter[]
                            {
                                new SqlParameter("@Entity_ID",EntityRes),
                                new SqlParameter("@NAME",dtParentMenu3.Rows[k]["NAME"].ToString()),
                                new SqlParameter("@PMENU",Pmenu3),
                                new SqlParameter("@ROLES",dtParentMenu3.Rows[k]["ROLES"].ToString()),
                                new SqlParameter("@DISPLAY_ORDER",dtParentMenu3.Rows[k]["DISPLAY_ORDER"].ToString()),
                                new SqlParameter("@ACTIONS_NAME",dtParentMenu3.Rows[k]["ACTIONS_NAME"].ToString()),
                                new SqlParameter("@IMAGE",dtParentMenu3.Rows[k]["IMAGE"].ToString()),
                                new SqlParameter("@CONTROLLER",dtParentMenu3.Rows[k]["CONTROLLER"].ToString()),
                                new SqlParameter("@MENU_TYPE",dtParentMenu3.Rows[k]["MENU_TYPE"].ToString()),
                                new SqlParameter("@Route",dtParentMenu3.Rows[k]["Route"].ToString())
                            };

                            int Pmenu4 = Convert.ToInt32(DataLib.ExecuteScaler("InsertDefaultMenu", CommandType.StoredProcedure, P3,conn,trans));
                        }
                    }
                }
                SqlParameter[] PMeta = new SqlParameter[]
                            {
                                new SqlParameter("@Entity_ID",EntityRes),
                            };
                int MetaRes = Convert.ToInt32(DataLib.ExecuteScaler("InsertDefaultMasterMeta", CommandType.StoredProcedure, PMeta, conn, trans));
                Res.Data = EntityRes.ToString();
                Res.IsSuccess = true;
                Res.Message = "New Entity Created Successfully.";
                return Res;
            }
            catch
            {  
                throw;
            }
        }

        public Response UpdateEntity(EntityVM model)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("@EntityID", model.EntityModel.TID),
                new SqlParameter("@EntityName", model.EntityModel.ENTITY_NAME),
                new SqlParameter("@EntityState", model.EntityModel.ENTITY_STATE),
                new SqlParameter("@EntityCity", model.EntityModel.ENTITY_CITY),
                new SqlParameter("@EntityPIN", model.EntityModel.ENTITY_PIN),
                new SqlParameter("@EntityAdd1", model.EntityModel.ENTITY_ADDR1),
                new SqlParameter("@EntityAdd2", model.EntityModel.ENTITY_ADDR2),
                new SqlParameter("@DomainName", model.EntityModel.DOMAIN_NAME),
                //new SqlParameter("@Payperiod", model.EntityModel.ENTITY_Payperiod),
                new SqlParameter("@DateFormat", model.EntityModel.DATE_FORMAT),
                new SqlParameter("@EntityCountry", model.EntityModel.ENTITY_Country),
                new SqlParameter("@EntityCurrency", model.EntityModel.ENTITY_Currency),
                  new SqlParameter("@Services_Availed", model.EntityModel.ENTITY_Services_Availed),
                new SqlParameter("@IsAct", model.EntityModel.IsActive),
                new SqlParameter("@UID", model.EntityModel.CreatedBy),
                };
                int InsRes = Convert.ToInt32(DataLib.ExecuteScaler("UpdateEntity_001", CommandType.StoredProcedure, parameters));
                if (InsRes == 0)
                {
                    Res.Message = "Entity updated successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                if (InsRes == -1)
                {
                    Res.Message = "Entity_Name must be unique.";
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

        public DataTable GetEntityCityGlobal(int State_Id)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@StateID", State_Id),
                };
                dt = DataLib.ExecuteDataTable("GetLocation_Entity", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetCurrencyEntity(CurrencyModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                   
                };
                dt = DataLib.ExecuteDataTable("GetCurrencyEntity", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable GetComponentForFile(int TemplateID, int Entity_ID)
        {
            SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@Template_ID", TemplateID),
                    new SqlParameter("@Entity_ID",Entity_ID)
                };
           return DataLib.ExecuteDataTable("GetComponentForFile", CommandType.StoredProcedure, parameters);
        }

        public DataTable GetDefaultFileDetail()
        {
            SqlParameter[] parameters = new SqlParameter[]{
                };
            return DataLib.ExecuteDataTable("GetDefaultFileDetail", CommandType.StoredProcedure, parameters);
        }

    }
}