using Ivap.Areas.Calendar.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.Calendar.Repository
{
    public class CalendarDetailsRepo
    {
        public List<CalendarDetailsModel> LoadData(int EID, int CalendarType, string PayDate, string DueDate, string Event,string FileType)
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
                /*Dt = DataLib.ExecuteDataTable("GetCalendarDtl_1", CommandType.StoredProcedure, parameters);*/ //GetCalendarDtl
                Dt = DataLib.ExecuteDataTable("GetCalendarDtl_1_New", CommandType.StoredProcedure, parameters);

                for (int i = 0; i < Dt.Rows.Count; i++)
                {
                    CalendarDetailsModel infoObj = new CalendarDetailsModel();
                    string Status = "";
                    if (Dt.Rows[i]["Last_Updated_Date"].ToString() == "" || Dt.Rows[i]["Last_Updated_Date"].ToString() == null)
                    {
                        Status = "Pending";
                        infoObj.BackColor = "#35B3CF";
                    }
                    else if (Convert.ToDateTime(Dt.Rows[i]["PAY_DATE"].ToString()) >= Convert.ToDateTime(Dt.Rows[i]["Last_Updated_Date"].ToString()))
                    {
                        Status = "Done";
                        infoObj.BackColor = "#4ACF35";
                    }
                    else if (Convert.ToDateTime(Dt.Rows[i]["PAY_DATE"].ToString()) < Convert.ToDateTime(Dt.Rows[i]["Last_Updated_Date"].ToString()))
                    {
                        Status = "Delay";
                        infoObj.BackColor = "#F73633";
                    }
                    string Last_Updated_Date = Dt.Rows[i]["Last_Updated_Date"].ToString() == "" ? "N/A" : Dt.Rows[i]["Last_Updated_Date"].ToString();
                    string Last_Action_By = Dt.Rows[i]["Last_Action_By"].ToString() == "" ? "N/A" : Dt.Rows[i]["Last_Action_By"].ToString();
                    infoObj.Sr = Convert.ToInt32(i + 1);
                    infoObj.Title = Dt.Rows[i]["DESCRIPTION"].ToString();
                    infoObj.Desc = "<b>Pay Date:</b> " + Dt.Rows[i]["PAY_DATE"].ToString() + "</br></br>" +
                                    "<b>Due Date:</b> " + Dt.Rows[i]["DUE_DATE_Show"].ToString() + "</br></br>" +
                                    "<b>Category:</b> " + Dt.Rows[i]["FILE_TYPE"].ToString() + "</br></br>" +
                                    "<b>Activity:</b> " + Dt.Rows[i]["Activity"].ToString() + "</br></br>" +
                                    "<b>Task Completion Date:</b> " + Last_Updated_Date + "</br></br>" +
                                    "<b>Last Action By:</b> " + Last_Action_By + "</br></br>" +
                                    "<b>Status:</b> " + Status;
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
        public DataTable GetUserByRole(int EID,string Role)
        {
            DataTable Dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@Role", Role),
                };
                Dt = DataLib.ExecuteDataTable("GetUserByRole", CommandType.StoredProcedure, parameters); //GetCalendarDtl
                return Dt;
            }
            catch(Exception ex)
            {
                throw;
                
            }
        }
    }
}