using Ivap.Areas.MOM.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.MOM.Repository
{
    public class MomRepo
    {
        Response res;

        public Response GetViewMOM(int MOMID, int ENTITY_ID)
        {
            try
            {
                Response res = new Response();
                DataTable dt = new DataTable();
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MOMID",MOMID),
                    new SqlParameter("@Entity_ID",ENTITY_ID)
                };
                dt = DataLib.ExecuteDataTable("GetViewMOM", CommandType.StoredProcedure, parameters);
                res.Data = JsonSerializer.SerializeTable(dt);
                res.IsSuccess = true;
                return res;
            }
            catch
            {
                throw;
            }
        }
        public UpdateMoM GetUpdateRequest(int MOMID, int Entity_ID)
        {

            try
            {
                UpdateMoM objMOM = new UpdateMoM();
                objMOM.MOMID = MOMID;
                objMOM.CreatedBy = Entity_ID;
                DataTable dt = new DataTable();
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MOMID",objMOM.MOMID),
                    new SqlParameter("@Entity_ID",objMOM.CreatedBy)
                };
                dt = DataLib.ExecuteDataTable("GetUpdateMOM", CommandType.StoredProcedure, parameters);
                if (dt.Rows.Count > 0)
                {
                    objMOM.IsActive = Convert.ToInt32(dt.Rows[0]["ISACTIVE"]) == 1 ? true : false;
                    objMOM.MeetingHeld = Convert.ToDateTime(dt.Rows[0]["MEETING_HELD"]);
                    objMOM.Meeting_Attendees = Convert.ToString(dt.Rows[0]["MEETING_ATTENDEES"]);
                    objMOM.Address = Convert.ToString(dt.Rows[0]["ADDRESS"]);
                    objMOM.Agenda = Convert.ToString(dt.Rows[0]["AGENDA"]);

                }
                return objMOM;
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetAttendeesData(int MOMID, int EntityID)
        {
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@MOMID", MOMID),
                    new SqlParameter("@ENTITYID", EntityID),
                };
                return DataLib.ExecuteDataTable("GetMomAttendeesDeatails", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetTotalMoMData(int MOMID, int EntityID, string Status)
        {
            try
            {

                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@MOMID", MOMID),
                    new SqlParameter("@Entity_ID", EntityID),
                    new SqlParameter("@Status", Status),
                };
                return DataLib.ExecuteDataTable("GetTotalMOM", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }
        public List<MOMGrid> GetMOMCalendarData(int MOMID, int @ENTITYID, int @UID, string Status)
        {
            DataSet ds = new DataSet();
            List<MOMGrid> lst = new List<MOMGrid>();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@MOMID", MOMID),
                    new SqlParameter("@ENTITYID", ENTITYID),
                    new SqlParameter("@UID", UID),
                    new SqlParameter("@STATUS", Status),

                };
                ds = DataLib.ExecuteDataSet("GetMomGridDeatails", CommandType.StoredProcedure, parameters);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    MOMGrid infoObj = new MOMGrid();
                    infoObj.UniqueID = Convert.ToInt32(ds.Tables[0].Rows[i]["TID"]);
                    infoObj.Sr = Convert.ToInt32(i + 1);
                    infoObj.Title = ds.Tables[0].Rows[i]["AGENDA"].ToString().Length > 30 ? ds.Tables[0].Rows[i]["AGENDA"].ToString().Substring(0, 30) + "..." + "(" + ds.Tables[0].Rows[i]["TotalPending"] + ")" : ds.Tables[0].Rows[i]["AGENDA"].ToString() + " (" + ds.Tables[0].Rows[i]["TotalPending"] + ")";
                    infoObj.Desc = "<b>ISSUE:</b> " + ds.Tables[0].Rows[i]["AGENDA"].ToString() + "</br></br>";


                    infoObj.Start_Date = ds.Tables[0].Rows[i]["MEETING_HELD"].ToString();
                    infoObj.End_Date = ds.Tables[0].Rows[i]["MEETING_HELD"].ToString();
                    infoObj.BackColor = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalPending"]) == 0 ? "#5cb85c" : Convert.ToString(ds.Tables[0].Rows[i]["PASSED_DATE"]) == "" ? "grey" : "#d9534f";
                    lst.Add(infoObj);
                }

                return lst;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
        }
        private bool moveFile(int MOMID, string fileName, int Entity_ID)
        {
            bool result = false;
            try
            {
                if (fileName != "" && fileName != null)
                {
                    string sourceFile = HttpContext.Current.Server.MapPath("~/Docs/TEMP/" + fileName);
                    string targetFile = HttpContext.Current.Server.MapPath("~/Docs/Entity_" + Entity_ID + "/MOM/" + MOMID + "/" + fileName);
                    bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/Docs/Entity_" + Entity_ID + "/MOM/ " + MOMID + "/"));
                    if (!exists)
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Docs/Entity_" + Entity_ID + "/MOM/" + MOMID + "/"));
                    System.IO.File.Move(sourceFile, targetFile);
                    result = true;
                }
                return result;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public Response AddMOM_ITEM(CreateMOM Model)
        {
            try
            {
                var collection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BaseMinutesDetails>>(Model.MinutesDetail);
                if (collection.Count == 0)
                {
                    res = new Response();
                    res.IsSuccess = false;
                    res.Message = "Kindly add minutes into grid";
                }
                if (collection.Count > 0)
                {
                    SqlTransaction trans = null;
                    SqlConnection conn = null;
                    try
                    {
                        conn = new SqlConnection(DataLib.GetConnectionString());
                        conn.Open();
                        trans = conn.BeginTransaction();
                        res = AddMom(Model, con: conn, trans: trans);
                        if (res.IsSuccess)
                        {
                            int MOMID = Convert.ToInt32(res.Data);
                            foreach (BaseMinutesDetails objCls in collection)
                            {
                                res = AddMoMItem(MOMID: MOMID, ITEM_ID: objCls.ITEM_ID.Value, MOM_Minutes: objCls.MINUTES, OwnerShip: objCls.RESPONSIBILITY, Expected_Closure_Date: objCls.E_C_D,
                                    Actual_Date: objCls.A_C_D, Curr_Status: objCls.MINUTES_STATUS, closedRemarks: objCls.closedRemarks, System_File_Name: objCls.systemFileName,
                                    Origianl_File_Name: objCls.originalFileName, conn: conn, trans: trans);

                                moveFile(MOMID: MOMID, fileName: objCls.systemFileName, Entity_ID: Model.EID);
                                if (res.IsSuccess == false)
                                {
                                    break;
                                }
                            }
                            res.Message = "MoM has been sucessfully saved";
                            trans.Commit();
                        }
                        else
                        {
                            trans.Rollback();
                            return res;
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        conn.Dispose();
                    }
                }
                return res;
            }
            catch
            {
                throw;
            }
        }

        public Response UpdateMOM_ITEM(UpdateMoM Model)
        {
            try
            {
                var collection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BaseMinutesDetails>>(Model.MinutesDetail);
                if (collection.Count == 0)
                {
                    res = new Response();
                    res.IsSuccess = false;
                    res.Message = "Kindly add minutes into grid";
                }
                if (collection.Count > 0)
                {
                    SqlTransaction trans = null;
                    SqlConnection conn = null;
                    try
                    {
                        conn = new SqlConnection(DataLib.GetConnectionString());
                        conn.Open();
                        trans = conn.BeginTransaction();
                        res = UpdateMom(Model, con: conn, trans: trans);
                        if (res.IsSuccess)
                        {
                            //int MOMID = Convert.ToInt32(res.Data);
                            //foreach (BaseMinutesDetails objCls in collection)
                            //{
                            //    res = AddMoMItem(MOMID: MOMID, ITEM_ID: objCls.ITEM_ID.Value, MOM_Minutes: objCls.MINUTES, OwnerShip: objCls.RESPONSIBILITY, Expected_Closure_Date: objCls.E_C_D, Actual_Date: objCls.A_C_D, Curr_Status: objCls.MINUTES_STATUS, closedRemarks: objCls.closedRemarks, conn: conn, trans: trans);
                            //    if (res.IsSuccess == false)
                            //    {
                            //        break;
                            //    }
                            //}
                            res.Message = "MoM has been sucessfully saved";
                            trans.Commit();
                        }
                        else
                        {
                            trans.Rollback();
                            return res;
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        conn.Dispose();
                    }
                }
                return res;
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetMomCountDetails(int EntityID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ENTITYID",EntityID),
                };

                return DataLib.ExecuteDataTable("GetMomCountDeatails", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }



        public DataTable GetMomDetails(MomBase model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MOMID",model.MID),
                    new SqlParameter("@ENTITYID",model.EID),
                    new SqlParameter("@ISACT",model.IsActive),
                    new SqlParameter("@UID",model.CreatedBy),
                    new SqlParameter("@filter",model.filter),
                };

                return DataLib.ExecuteDataTable("GetMomDeatails", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }

        public DataTable GetMomItemDetails(MomBase model)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MOM_ID",model.MID)
                };

                return DataLib.ExecuteDataTable("GetMinutesItem", CommandType.StoredProcedure, parameters);
            }
            catch
            {
                throw;
            }
        }

        public Response DeleteMomItemDetails(int Item_ID)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@ITEM_ID",Item_ID)
                };

                DataLib.ExecuteNonQuery("DeleteMomItem", CommandType.StoredProcedure, parameters);
                res.IsSuccess = true;
                res.Message = "Item sucessfully deleted";
                return res;
            }
            catch
            {
                throw;
            }
        }


        public Response AddMom(CreateMOM model, SqlConnection con, SqlTransaction trans)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ENTITY_ID",model.EID),
                    new SqlParameter("@MEETING_HELD",model.MeetingHeld),
                    new SqlParameter("@MEETING_ATTENDEES",model.Meeting_Attendees),
                    new SqlParameter("@ADDRESS",model.Address),
                    new SqlParameter("@AGENDA",model.Agenda),
                    new SqlParameter("@CREATED_BY",model.CreatedBy),
                    new SqlParameter("@ISACTIVE",model.IsActive)
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddMOM", CommandType.StoredProcedure, parameters, con: con, trans: trans));
                if (result > 0)
                {

                }
                Response.operation opration = Response.operation.ADD;
                if (model.MID > 0)
                    opration = Response.operation.Update;
                res = res.GetResponse(opration, "Mom", result, "Agenda");
                if (result == 0) res.Data = model.MID.ToString();
                return res;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
        }

        public Response UpdateMom(UpdateMoM model, SqlConnection con, SqlTransaction trans)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@MID",model.MOMID),
                    new SqlParameter("@ENTITY_ID",model.EID),
                    new SqlParameter("@MEETING_HELD",model.MeetingHeld),
                    new SqlParameter("@MEETING_ATTENDEES",model.Meeting_Attendees),
                    new SqlParameter("@ADDRESS",model.Address),
                    new SqlParameter("@AGENDA",model.Agenda),
                    new SqlParameter("@CREATED_BY",model.CreatedBy),
                    new SqlParameter("@ISACTIVE",model.IsActive)
                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("UpdateMOM", CommandType.StoredProcedure, parameters, con: con, trans: trans));
                if (result > 0)
                {

                }
                Response.operation opration = Response.operation.ADD;
                if (model.MOMID > 0)
                    opration = Response.operation.Update;
                res = res.GetResponse(opration, "Mom", result, "Agenda");
                if (result == 0) res.Data = model.MOMID.ToString();
                return res;
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                throw;
            }
        }

        public Response AddMoMItem(int MOMID, int? ITEM_ID, string MOM_Minutes, string OwnerShip,
            string Expected_Closure_Date, string Actual_Date, string Curr_Status, string closedRemarks,
            string System_File_Name, string Origianl_File_Name,
            SqlConnection conn, SqlTransaction trans)
        {
            Response res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MOM_ID",MOMID),
                    new SqlParameter("@ITEM_ID",ITEM_ID),
                    new SqlParameter("@MOM_Minutes",MOM_Minutes),
                    new SqlParameter("@OwnerShip",OwnerShip),
                    new SqlParameter("@Expected_Closure_Date",Expected_Closure_Date),
                    new SqlParameter("@Actual_Date",Actual_Date),
                    new SqlParameter("@closed_Remarks",closedRemarks),
                    new SqlParameter("@Curr_Status",Curr_Status),
                    new SqlParameter("@System_File_Name",System_File_Name),
                    new SqlParameter("@Original_File_Name",Origianl_File_Name),
                };

                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddMomItem1", CommandType.StoredProcedure, parameters, con: conn, trans: trans));
                res.IsSuccess = true;
                return res;
            }
            catch
            {
                throw;
            }
        }

        public Response AddMinutes(BaseMinutesDetails model)
        {
            Response res;
            SqlTransaction trans = null;
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(DataLib.GetConnectionString());
                conn.Open();
                trans = conn.BeginTransaction();
                res = new Response();
                res = AddMoMItem(MOMID: model.MID, ITEM_ID: model.ITEM_ID, MOM_Minutes: model.MINUTES, OwnerShip: model.RESPONSIBILITY,
                    Expected_Closure_Date: model.E_C_D, Actual_Date: model.A_C_D, Curr_Status: model.MINUTES_STATUS,
                    closedRemarks: model.closedRemarks, System_File_Name: model.systemFileName, Origianl_File_Name: model.originalFileName, conn: conn, trans: trans);
                moveFile(MOMID: model.MID, fileName: model.systemFileName, Entity_ID: model.EID);
                trans.Commit();
                conn.Dispose();
                return res;
            }
            catch
            {
                trans.Rollback();
                conn.Dispose();
                throw;
            }
        }
    }
}