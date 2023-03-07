using Ivap.Areas.CAPA.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.CAPA.Repository
{
    public class EditCapaRepo
    {
        public DataSet GetCapa(int CapaID,int EID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@CapaID", CapaID),
                    
                };
                ds = DataLib.ExecuteDataSet("GetCapa_New", CommandType.StoredProcedure, parameters);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public CapaModel GetCapaEdit(int CapaID, int EID)
        {
            DataSet ds = new DataSet();
            CapaModel objCapaModel = new CapaModel();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@CapaID", CapaID),

                };
                ds = DataLib.ExecuteDataSet("GetCapa_New", CommandType.StoredProcedure, parameters);
                objCapaModel.TID=Convert.ToInt32( ds.Tables[0].Rows[0]["TID"]);
                objCapaModel.Issue = ds.Tables[0].Rows[0]["ISSUE"].ToString();
                objCapaModel.Issue_Description = ds.Tables[0].Rows[0]["ISSUE_DESCRIPTION"].ToString();
                objCapaModel.Customer_Impact = ds.Tables[0].Rows[0]["CUSTOMER_IMPACT"].ToString();
                objCapaModel.Sequence_of_events = ds.Tables[0].Rows[0]["SEQUENCE_OF_EVENT"].ToString();
                objCapaModel.Communication_process = ds.Tables[0].Rows[0]["COMMUNICATION_PROCESS"].ToString();
                objCapaModel.Root_Cause = ds.Tables[0].Rows[0]["ROOT_CAUSE"].ToString();

                objCapaModel.Category = ds.Tables[0].Rows[0]["CATEGORY"].ToString();
                objCapaModel.Finance_Type = ds.Tables[0].Rows[0]["FINANCE_TYPE"].ToString();
                objCapaModel.Finance_Amount = ds.Tables[0].Rows[0]["FINANCE_AMOUNT"].ToString();
                objCapaModel.Stage = ds.Tables[0].Rows[0]["STAGE"].ToString();
                objCapaModel.Impact_Value = ds.Tables[0].Rows[0]["IMPACT_VALUE"].ToString();
                objCapaModel.Incident_date = ds.Tables[0].Rows[0]["INCIDENT_DATE"].ToString();
                objCapaModel.CorrectiveAction_Detail = JsonSerializer.SerializeTable(ds.Tables[1]);
                objCapaModel.PreventiveAction_Detail = JsonSerializer.SerializeTable(ds.Tables[2]);

                objCapaModel.CapaConversationCorrective = JsonSerializer.SerializeTable(ds.Tables[3]);
                objCapaModel.CapaConversationPreventive = JsonSerializer.SerializeTable(ds.Tables[4]);

                return objCapaModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public DataTable GetCapaHis(int CapaID)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@CapaID", CapaID),

                };
            
                dt = DataLib.ExecuteDataTable("GetCapaHis", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        ////New Code start
        ///
        public DataSet GetCapaCalendar(string CapaDate, int EID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", EID),
                    new SqlParameter("@CapaDate", CapaDate),

                };
                ds = DataLib.ExecuteDataSet("GetCapaCalendar", CommandType.StoredProcedure, parameters);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<CapaCalendarModel> GetCapaCalendarData(int EID)
        {
            DataSet ds = new DataSet();
            List<CapaCalendarModel> lst = new List<CapaCalendarModel>();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID", EID),


                };
                ds = DataLib.ExecuteDataSet("GetCapaCalendarData", CommandType.StoredProcedure, parameters);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    CapaCalendarModel infoObj = new CapaCalendarModel();

                    infoObj.Sr = Convert.ToInt32(ds.Tables[0].Rows[i]["TID"]);
                    infoObj.Title = ds.Tables[0].Rows[i]["ISSUE"].ToString();
                    infoObj.Desc = ds.Tables[0].Rows[i]["ISSUE"].ToString();
                    //+ "</br></br>" +
                    //                "<b>ISSUE DESCRIPTION:</b> " + ds.Tables[0].Rows[i]["ISSUE_DESCRIPTION"].ToString();
                    infoObj.Start_Date = ds.Tables[0].Rows[i]["CREATED_ON"].ToString();
                    infoObj.End_Date = ds.Tables[0].Rows[i]["CREATED_ON"].ToString();
                    lst.Add(infoObj);
                }

                return lst;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}