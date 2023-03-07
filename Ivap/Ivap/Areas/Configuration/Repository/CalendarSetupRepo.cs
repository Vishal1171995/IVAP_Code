using Ivap.Areas.Calendar.Models;
using Ivap.Areas.Configuration.Models;
using Ivap.Repository;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Configuration.Repository
{
    public class CalendarSetupRepo
    {
        public Response AddUpdateCalendarSetup(CalendarSetupModel model)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",model.TID),
                    new SqlParameter("@ENTITY_ID",model.EID),
                     new SqlParameter("@PAY_DATE",model.PAY_DATE),
                    new SqlParameter("@CALENDAR_TYPE",model.CALENDAR_TYPE),
                    new SqlParameter("@DESCRIPTION",model.DESCRIPTION),
                    new SqlParameter("@DUE_DATE",model.DUE_DATE),
                    new SqlParameter("@EVENT",model.EVENT),
                    new SqlParameter("@File_Type",model.FILE_TYPE),
                    new SqlParameter("@ISACTIVE",model.IsActive),
                    new SqlParameter("@UID",model.CreatedBy),
                    new SqlParameter("@ActivityCategory",model.ActivityCategory),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("AddUpdateCalendar_Setup_New_1", CommandType.StoredProcedure, parameters)); //AddUpdateCalendar_Setup_New
                if (result > 0)
                {
                    Res.Message = "Calendar setup created successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                if (result == 0)
                {
                    Res.Message = "Calendar setup updated successfully.";
                    Res.IsSuccess = true;
                    return Res;
                }
                else if (result == -1)
                {
                    Res.Message = "Failed!!! Calendar name must be unique.";
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

        public DataTable GetCalendarType()
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = null;
                dt = DataLib.ExecuteDataTable("GetCalendarType", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch
            {
                throw;
            }
        }
        public DataTable GetCalendarSetup(CalendarSetupModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID", objModel.TID),
                    new SqlParameter("@ENTITY_ID", objModel.EID),
                    new SqlParameter("@CALENDAR_NAME", objModel.CALENDAR_NAME),
                    new SqlParameter("@IsAct", objModel.IsActive)
                };
                dt = DataLib.ExecuteDataTable("GetCalendar_Setup", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataTable GetCalendarSetupHistory(CalendarSetupModel objModel)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID", objModel.TID),
                };
                dt = DataLib.ExecuteDataTable("GetCalendar_Setup_His", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public KendoGridUtils GetCommandButtonForGrid(string RouteName)
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

                    CommandButton ObjDeleteButton = new CommandButton();
                    ObjDeleteButton.name = "Delete";
                    ObjDeleteButton.text = "";
                    ObjDeleteButton.click = "DeleteHandler";
                    ObjDeleteButton.iconClass = "kIcon k-delete ";
                    ObjDeleteButton.title = "Delete";
                    LstCommand.Add(ObjDeleteButton);
                    CommandCount++;
                }
                if (AuthorizationRepo.IsValidAction(RouteName, "ViewAction"))
                {
                    CommandButton ObjViewButton = new CommandButton();
                    ObjViewButton.name = "View";
                    ObjViewButton.text = "";
                    ObjViewButton.click = "ViewHandler";
                    ObjViewButton.iconClass = "kIcon kIconView";
                    ObjViewButton.title = "View";
                    LstCommand.Add(ObjViewButton);
                    CommandCount++;
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
        public Response DeleteCalendarSetup(CalendarSetupModel model)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@TID",model.TID),
                    new SqlParameter("@ENTITY_ID",model.EID),
                    new SqlParameter("@UID",model.CreatedBy),

                };
                int result = Convert.ToInt32(DataLib.ExecuteScaler("DeleteCalendarSetup", CommandType.StoredProcedure, parameters));
                if (result == 0)
                {
                    Res.Message = "Calendar setup deleted successfully.";
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

        public List<CalendarDetailsModel> GetCalendarSetupForCalendarView(int EID, int CalendarType, string PayDate, string DueDate, string Event, string FileType)
        {
            // Initialization.
            List<CalendarDetailsModel> lst = new List<CalendarDetailsModel>();
            DataTable Dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@CaledarType", CalendarType),
                    new SqlParameter("@Event", Event),
                    new SqlParameter("@FileType", FileType),
                    new SqlParameter("@DueDate", DueDate),
                    new SqlParameter("@PayDate", PayDate),
                };
                Dt = DataLib.ExecuteDataTable("GetCalendarSetupForCalendarView", CommandType.StoredProcedure, parameters); //GetCalendarDtl

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    CalendarDetailsModel infoObj = new CalendarDetailsModel();
                    infoObj.Sr = Convert.ToInt32(i + 1);
                    infoObj.Title = Dt.Rows[i]["DESCRIPTION"].ToString();
                    infoObj.Desc = "<b>Pay Date:</b> " + Dt.Rows[i]["PAY_DATE"].ToString() + "</br></br>" +
                                    "<b>Caledar Type:</b> " + Dt.Rows[i]["CaledarType"].ToString() + "</br></br>" +
                                    "<b>FileType:</b> " + Dt.Rows[i]["FileType"].ToString() + "</br></br>" +
                                    "<b>Event:</b> " + Dt.Rows[i]["Event"].ToString();
                    infoObj.Start_Date = Dt.Rows[i]["DUE_DATE"].ToString();
                    infoObj.End_Date = Dt.Rows[i]["DUE_DATE"].ToString();
                    lst.Add(infoObj);
                }
            }
            catch (Exception ex)
            {
                // info.
                Console.Write(ex);
            }

            // info.
            return lst;
        }
    }
}