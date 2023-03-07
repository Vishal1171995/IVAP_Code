using Ivap.Areas.DashBoards.Models;
using Ivap.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ivap.Areas.DashBoards.Repository
{
    public class DashBoardRepo
    {

        public DataTable DownLoadDataPayrollTrend(int Entity_ID, string PAYDATE,string ROLENAME,int FILETYPEID,int UID)
        {
            ROLENAME = ROLENAME.Replace(" ", "_");
            DataSet ds = new DataSet();
            string COMPONENT_NAME = GetComponent(FILETYPEID, Entity_ID);
            COMPONENT_NAME = "select CONVERT(CHAR(4),PAYDATE, 100) + CONVERT(CHAR(4), PAYDATE, 120)[PAYDATE], " + COMPONENT_NAME + " from Ivap_TEMP_HIS_" + Entity_ID + " WHERE " + ROLENAME + "=" + UID;
            COMPONENT_NAME += " and PAYDATE in ('" + PAYDATE + "')";
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@COMPONENT_NAME",COMPONENT_NAME)
                };
                DataTable dt = DataLib.ExecuteDataTable("GetPayComponentTrendData", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataSet GetFileTypeDB(int Entity_ID, int UID, string ROLENAME, string PAYDATE)
        {
            ROLENAME = ROLENAME.Replace(" ", "_");
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ENTITY_ID",Entity_ID),
                    new SqlParameter("@UID",UID),
                    new SqlParameter("@ROLENAME",ROLENAME),
                    new SqlParameter("@PAYDATE",PAYDATE)

                };
                DataSet ds = DataLib.ExecuteDataSet("GetFileTypeDB_NEW", CommandType.StoredProcedure, parameters);
               
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
        }    

        public string GetComponent(int FILETYPEID, int EID)
        {
            string str = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@FILETYPEID", FILETYPEID),
                    new SqlParameter("@ENTITYID",EID),
                };
                dt = DataLib.ExecuteDataTable("GetPayComponentName", CommandType.StoredProcedure, parameters);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    str = str + "SUM(ISNULL(cast(" + dt.Rows[i]["COMPONENT_NAME"].ToString() + " as decimal),0))" + "[" + dt.Rows[i]["COMPONENT_NAME"].ToString() + "]";
                    str += (i < dt.Rows.Count) ? "," : string.Empty;
                }
                if (dt.Rows.Count > 0)
                {
                    str = str.Substring(0, str.Length - 1);
                }
                return str;
            }
            catch
            {
                throw;
            }
        }





        public DataTable GetCTCReconDataOutput(int Entity_ID, string PAYDATE, string PREV_PAYDATE)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID",Entity_ID),
                    new SqlParameter("@PayDate",PAYDATE),
                    new SqlParameter("@PreviousPayDate",PREV_PAYDATE)
                };




                string strQry = "DECLARE @LastMonthCTC DECIMAL(18,2); DECLARE @CurrentMonthCTC DECIMAL(18, 2); ";
                strQry = strQry + " SET @LastMonthCTC = (select isnull(Sum(CTC), 0)  from Ivap_TEMP_HIS_" + Entity_ID + " Temp inner " +
                "join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID  " +
                "where PayDate = '" + PREV_PAYDATE + "' and F.CATEGORY = 'CTC Master' " +
                "and F.file_type = 'PMS Output File') ";
                strQry = strQry + " SET @CurrentMonthCTC = (  " +
                "select isnull(Sum(CTC), 0)  from IVAP_TEMP_HIS_" + Entity_ID + " Temp " +
                "inner  " +
                "join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID  " +
                "where PayDate = '" + PAYDATE + "' and F.CATEGORY = 'CTC Master' " +
                "and F.file_type = 'PMS Output File' and Temp.EMP_CODE not in ( " +
                "select Distinct EMP_CODE from IVAP_TEMP_HIS_" + Entity_ID + " Temp " +
                "inner   join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID " +
                "where PayDate = '" + PREV_PAYDATE + "'  and F.CATEGORY = 'CTC Master' " +
                "and F.file_type = 'PMS Output File')) ";

                strQry = strQry + "select isnull(Sum(CTC),0) AS INPUTCTC, isnull(Sum(CTC), 0) AS OUTPUTCTC,'Opening Balance' As Type from IVAP_TEMP_HIS_" + Entity_ID + " Temp " +
                " inner join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID " +
                " where PayDate = '" + PREV_PAYDATE + "' and F.CATEGORY = 'CTC Master' " +
                " and F.file_type = 'PMS Output File' ";

                strQry = strQry + "union all " +
                " select isnull(Sum(CTC),0) As INPUTCTC,(select isnull(Sum(CTC),0) AS OUTPUTCTC from IVAP_TEMP_HIS_" + Entity_ID + " Temp  inner join IVAP_MST_FILETYPE F ON F.TID=Temp.File_ID"+
                " where PayDate =  '" + PREV_PAYDATE + "' and F.CATEGORY = 'CTC Master' and F.file_type = 'PMS Output File' AND EMP_CODE IN" +
                " (SELECT EMP_CODE FROM IVAP_TEMP_HIS_" + Entity_ID + " WHERE PayDate = '" + PAYDATE + "') and EMP_CODE not IN(SELECT EMP_CODE FROM IVAP_TEMP_HIS_" + Entity_ID + "" +
                " WHERE PayDate =  '" + PREV_PAYDATE + "')),'New Joiner' AS Type from IVAP_TEMP_HIS_" + Entity_ID + " Temp  " +
                " inner join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID " +
                " where PayDate = '" + PAYDATE + "' and F.CATEGORY = 'New Joiner' " +
                " and F.file_type = 'Payroll Input File' ";

                strQry = strQry + "union all " +
                " select isnull(Sum(CTC),0) As INPUTCTC,0 AS OUTPUTCTC,'Transfer In' AS Type from IVAP_TEMP_HIS_" + Entity_ID + " Temp " +
                " inner  join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID " +
                " where PayDate = '" + PAYDATE + "' and F.CATEGORY = 'Transfer InP' and F.file_type = 'Payroll Input File'";


                strQry = strQry + "union all " +

                "select isnull(Sum(CTC),0) As INPUTCTC,"+
                " (select isnull(Sum(CTC), 0) AS OUTPUTCTC from IVAP_TEMP_HIS_" + Entity_ID + " Temp" +
                 " inner"+
                 " join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID"+
                 " where PayDate = '" + PREV_PAYDATE + "' and F.CATEGORY = 'CTC Master'" +
                " and F.file_type = 'PMS Output File' AND EMP_CODE not IN"+
                " (SELECT EMP_CODE FROM IVAP_TEMP_HIS_" + Entity_ID + " WHERE PayDate = '" + PAYDATE + "')) OUTPUTCTC,'Transfer Out' AS Type from IVAP_TEMP_HIS_" + Entity_ID + " Temp " +
                " inner" + " join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID " +
                " where PayDate = '" + PAYDATE + "' and F.CATEGORY = 'Transfer Out' and F.file_type = 'Payroll Input File' " +
                " union all " +
                " select isnull(Sum(CTC),0) As INPUTCTC,0 AS OUTPUTCTC,'Separated Employee' AS Type from IVAP_TEMP_HIS_" + Entity_ID + " Temp " +
                " inner " +
                " join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID " +
                " where PayDate = '" + PAYDATE + "' and F.CATEGORY = 'Resignation' and F.file_type = 'Payroll Input File' " +
                " union all " +
                " select isnull(Sum(CTC),0) As INPUTCTC,(" +

                " select isnull(Sum(CTC),0) AS OUTPUTCTC from IVAP_TEMP_HIS_" + Entity_ID + " Temp " +
                " inner" +
                " join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID" +
                " where PayDate = '" + PREV_PAYDATE + "' and F.CATEGORY = 'CTC Master'" +
                " and F.file_type = 'PMS Output File' AND EMP_CODE IN" +
                " (SELECT EMP_CODE FROM IVAP_TEMP_HIS_" + Entity_ID + " WHERE PayDate = '" + PAYDATE + "')),'CTC Change' AS Type from IVAP_TEMP_HIS_" + Entity_ID + " Temp " +
                " inner join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID " +
                " where PayDate = '" + PAYDATE + "' and F.CATEGORY = 'Package Change' and F.file_type = 'Payroll Input File' " +
                " union all " +
                "select 0 As INPUTCTC,0 AS OUTPUTCTC,'Closing Balance'[Type] ";

                DataTable dt = DataLib.ExecuteDataTable(strQry, CommandType.Text, parameters);

                decimal Closingb_Input = 0;
                decimal Closingb_Output = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++)
                {
                    if (i < 3)
                    {
                        Closingb_Input += Convert.ToDecimal(dt.Rows[i][0].ToString());
                        Closingb_Output += Convert.ToDecimal(dt.Rows[i][1].ToString());
                    }
                    else
                    {
                        Closingb_Input -= Convert.ToDecimal(dt.Rows[i][0].ToString());
                        Closingb_Output -= Convert.ToDecimal(dt.Rows[i][1].ToString());
                    }

                }
                dt.Rows[6][0] = Closingb_Input;
                dt.Rows[6][1] = Closingb_Output;
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public DataTable GetHeadCountDataOutput(int Entity_ID, string PAYDATE, string PREV_PAYDATE)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID",Entity_ID),
                    new SqlParameter("@PayDate",PAYDATE),
                    new SqlParameter("@PreviousPayDate",PREV_PAYDATE)
                };




                string strQry = "DECLARE @LastMonthCTC DECIMAL(18,2); DECLARE @CurrentMonthCTC DECIMAL(18, 2); ";

                strQry = strQry + " SET @LastMonthCTC = (" +
                " select Count(*)  from Ivap_TEMP_HIS_" + Entity_ID + " Temp inner join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID" +
                " where PayDate = '" + PREV_PAYDATE + "' and F.CATEGORY = 'CTC Master'" +
                " and F.file_type = 'PMS Output File')" +
                " SET @CurrentMonthCTC = (" +
                " select Count(*)  from Ivap_TEMP_HIS_" + Entity_ID + " Temp " +
                " inner join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID" +
                " where PayDate = '" + PAYDATE + "' and F.CATEGORY = 'CTC Master'" +
                " and F.file_type = 'PMS Output File' and Temp.EMP_CODE not in (" +
                " select Distinct EMP_CODE from Ivap_TEMP_HIS_" + Entity_ID + " Temp" +
                " inner join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID" +
                " where PayDate = '" + PREV_PAYDATE + "' and F.CATEGORY = 'CTC Master'" +
                " and F.file_type = 'PMS Output File'))" +
                " select Count(*) AS INPUTCTC, Count(*) AS OUTPUTCTC,'Opening Balance' As Type from Ivap_TEMP_HIS_" + Entity_ID + " Temp" +
                " inner" +
                " join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID" +
                " where PayDate = '" + PREV_PAYDATE + "' and F.CATEGORY = 'CTC Master'" +
                " and F.file_type = 'PMS Output File'" +
                " union all" +
                " select Count(*) As INPUTCTC," + 
                " (SELECT count(distinct EMP_CODE) FROM IVAP_TEMP_HIS_" + Entity_ID + " WHERE PayDate = '" + PAYDATE + "' and EMP_CODE not IN (SELECT EMP_CODE " +
                " FROM IVAP_TEMP_HIS_" + Entity_ID + "" +
                " WHERE PayDate =  '" + PREV_PAYDATE + "'))" +
                " AS OUTPUTCTC, 'New Joiner' AS Type from Ivap_TEMP_HIS_" + Entity_ID + " Temp" +
                " inner join IVAP_MST_FILETYPE F ON F.TID=Temp.File_ID" +
                " where PayDate = '" + PAYDATE + "' and F.CATEGORY = 'New Joiner'" +
                " and F.file_type = 'Payroll Input File'" +
                " union all" +
                " select Count(*) As INPUTCTC,0 AS OUTPUTCTC,'Transfer In' AS Type from Ivap_TEMP_HIS_" + Entity_ID + " Temp" +
                " inner" +
                " join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID" +
                " where PayDate = '" + PAYDATE + "' and F.CATEGORY = 'Transfer InP' and F.file_type = 'Payroll Input File'" +
                " union all" +
                " select Count(*) As INPUTCTC,(SELECT count(distinct EMP_CODE) FROM IVAP_TEMP_HIS_" + Entity_ID + " WHERE PayDate = '" + PREV_PAYDATE+ "' and EMP_CODE not IN(SELECT EMP_CODE " +
                " FROM IVAP_TEMP_HIS_" + Entity_ID + "" +
                " WHERE PayDate =  '" + PAYDATE + "')) " +
                " AS OUTPUTCTC,'Transfer Out' AS Type from Ivap_TEMP_HIS_" + Entity_ID + " Temp" +
                " inner" +
                " join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID" +
                " where PayDate = '" + PAYDATE + "' and F.CATEGORY = 'Transfer Out' and F.file_type = 'Payroll Input File'" +
                " union all" +
                " select Count(*) As INPUTCTC,0 AS OUTPUTCTC,'Separated Employee' AS Type from Ivap_TEMP_HIS_" + Entity_ID + " Temp" +
                " inner" +
                " join IVAP_MST_FILETYPE F ON F.TID = Temp.File_ID" +
                " where PayDate = '" + PAYDATE + "' and F.CATEGORY = 'Resignation' and F.file_type = 'Payroll Input File'" +
                " union all" +
                " select 0 As INPUTCTC,0 AS OUTPUTCTC,'Closing Balance'[Type]";

                DataTable dt = DataLib.ExecuteDataTable(strQry, CommandType.Text, parameters);
                decimal Closingb_Input = 0;
                decimal Closingb_Output = 0;
                for (int i = 0; i < dt.Rows.Count - 1; i++)
                {
                    if (i < 3)
                    {
                        Closingb_Input += Convert.ToDecimal(dt.Rows[i][0].ToString());
                        Closingb_Output += Convert.ToDecimal(dt.Rows[i][1].ToString());
                    }
                    else
                    {
                        Closingb_Input -= Convert.ToDecimal(dt.Rows[i][0].ToString());
                        Closingb_Output -= Convert.ToDecimal(dt.Rows[i][1].ToString());
                    }

                }
                dt.Rows[5][0] = Closingb_Input;
                dt.Rows[5][1] = Closingb_Output;
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public DataTable GetExceptionDataOutput(int Entity_ID, string PAYDATE, string PREV_PAYDATE)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID",Entity_ID),
                    new SqlParameter("@PayDate",PAYDATE),
                    new SqlParameter("@PreviousPayDate",PREV_PAYDATE)
                };
                DataTable dt = DataLib.ExecuteDataTable("GetExceptionDashBoard", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataSet GetCTCReconcilationDB(int Entity_ID, string PAYDATE, string PREV_PAYDATE)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ENTITYID",Entity_ID),
                    new SqlParameter("@PAYDATE",PAYDATE),
                    new SqlParameter("@P_PAYDATE",PREV_PAYDATE)
                };
                DataSet ds = DataLib.ExecuteDataSet("GetCTCReconcilationDB_New1", CommandType.StoredProcedure, parameters);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public DataTable GetSDSOutput(int Entity_ID, string PAYDATE, string PREV_PAYDATE)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID",Entity_ID),
                    new SqlParameter("@PayDate",PAYDATE),
                    new SqlParameter("@PreviousPayDate",PREV_PAYDATE)
                };
                DataTable dt = DataLib.ExecuteDataTable("GetSDSOutputDashBoard", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable GetOneTimeEarningDeduction(int Entity_ID, string PAYDATE, string PREV_PAYDATE)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@EID",Entity_ID),
                    new SqlParameter("@PayDate",PAYDATE),
                    new SqlParameter("@PreviousPayDate",PREV_PAYDATE)
                };
                DataTable dt = DataLib.ExecuteDataTable("GetOneTimeEarningDeduction", CommandType.StoredProcedure, parameters);
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public HorizontalBarChartData GetVarianceDB_Output(int EID, string ROLENAME, int UID, string PAYDATE)
        {
            HorizontalBarChartData BarData = new HorizontalBarChartData();
            ROLENAME = ROLENAME.Replace(" ", "_");
            DataSet ds = new DataSet();
            string COMPONENT_NAME = GetComponent_Output(EID);
            if (string.IsNullOrEmpty(COMPONENT_NAME))
                return BarData;
            COMPONENT_NAME = "select CONVERT(CHAR(4),PAYDATE, 100) + CONVERT(CHAR(4), PAYDATE, 120)[PAYDATE], " + COMPONENT_NAME + " from Ivap_TEMP_HIS_" + EID + " WHERE " + ROLENAME + "=" + UID;
            COMPONENT_NAME += " and PAYDATE in ('" + PAYDATE + "',DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-1, -1))";
            COMPONENT_NAME += " group by PAYDATE order by convert(date,PAYDATE) desc";
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@COMPONENT_NAME", COMPONENT_NAME),
                    new SqlParameter("@ENTITYID", EID),
                    new SqlParameter("@DEFPAYDATE", PAYDATE)

                };

                ds = DataLib.ExecuteDataSet("GetDataFromQry_Output", CommandType.StoredProcedure, parameters);
                List<SeriesData> objList = new List<SeriesData>();
                SeriesData ObjData = null;

                List<string> lstCategories = new List<string>();
                for (int Comp = 0; Comp < ds.Tables[0].Rows.Count; Comp++)
                {
                    lstCategories.Add(ds.Tables[0].Rows[Comp]["PayDate"].ToString());
                }
                //Now Loop through Component
                for (int k = 1; k < ds.Tables[0].Columns.Count; k++)

                {

                    int addflag = 0;
                    var ObjData1 = new SeriesData();
                    List<decimal> LstCate = new List<decimal>();
                    for (int Comp = 0; Comp < ds.Tables[0].Rows.Count; Comp++)
                    {
                        LstCate.Add(Convert.ToInt32(ds.Tables[0].Rows[Comp][k]));
                        if (Convert.ToInt32(ds.Tables[0].Rows[Comp][k]) != 0)
                        {
                            addflag = 1;
                            ObjData1.name = ds.Tables[0].Columns[k].ColumnName;

                        }

                    }
                    if (addflag == 1)
                    {
                        ObjData1.data = LstCate;
                        objList.Add(ObjData1);
                    }

                }
                BarData.categorise = lstCategories;
                BarData.series = objList;
                return BarData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string GetComponent_Output(int EID)
        {
            string str = string.Empty;
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ENTITYID",EID)
                };
                dt = DataLib.ExecuteDataTable("GetPayComponentName_Output", CommandType.StoredProcedure, parameters);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    str = str + "SUM(ISNULL(cast(" + dt.Rows[i]["COMPONENT_NAME"].ToString() + " as decimal),0))" + "[" + dt.Rows[i]["COMPONENT_NAME"].ToString() + "]";
                    str += (i < dt.Rows.Count) ? "," : string.Empty;
                }
                if (dt.Rows.Count > 0)
                {
                    str = str.Substring(0, str.Length - 1);
                }
                return str;
            }
            catch
            {
                throw;
            }
        }
        public DataSet GetHeadCountDB(int Entity_ID,string PAYDATE, string PREV_PAYDATE)
        {
            Response Res = new Response();
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@ENTITYID",Entity_ID),
                    new SqlParameter("@PAYDATE",PAYDATE),
                    new SqlParameter("@PREV_PAYDATE",PREV_PAYDATE)
                };
                DataSet ds = DataLib.ExecuteDataSet("GetHeadCountDB_01", CommandType.StoredProcedure, parameters);
                return ds;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public HorizontalBarChartData GetPayComponentDB(int FILETYPEID, int EID, string ROLENAME, int UID, string PAYDATE, int DURATION)
        {
            HorizontalBarChartData BarData = new HorizontalBarChartData();
            ROLENAME = ROLENAME.Replace(" ", "_");
            DataSet ds = new DataSet();
            string COMPONENT_NAME = GetComponent(FILETYPEID, EID);
            if (string.IsNullOrEmpty(COMPONENT_NAME))
                return BarData;
            COMPONENT_NAME = "select CONVERT(CHAR(4),PAYDATE, 100) + CONVERT(CHAR(4), PAYDATE, 120)[PAYDATE], " + COMPONENT_NAME + " from Ivap_TEMP_HIS_" + EID + " WHERE " + ROLENAME + "=" + UID;
            if (DURATION == 3)
            {
                COMPONENT_NAME += " and PAYDATE in ('" + PAYDATE + "',DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-1, -1),DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-2, -1))";
            }
            else if (DURATION == 6)
            {
                COMPONENT_NAME += " and  PAYDATE in ('" + PAYDATE + "',DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-1, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-2, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-3, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-4, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-5, -1))";
            }
            else if(DURATION==9)
            {
                COMPONENT_NAME += " and  PAYDATE in ('" + PAYDATE + "',DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-1, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-2, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-3, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-4, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-5, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-6, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-7, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-8, -1))";
            }
            else
            {
                COMPONENT_NAME += " and PAYDATE in ('" + PAYDATE + "',DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-1, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-2, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-3, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-4, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-5, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-6, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-7, -1),DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-8, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-9, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-10, -1), DATEADD(MONTH, DATEDIFF(MONTH, -1, '" + PAYDATE + "')-11, -1))";
            }
            COMPONENT_NAME += " and FILE_ID=" + FILETYPEID + " group by PAYDATE order by convert(date,PAYDATE) desc";
            try
            {
                SqlParameter[] parameters = new SqlParameter[]{
                    new SqlParameter("@COMPONENT_NAME", COMPONENT_NAME),
                    new SqlParameter("@FILETYPEID", FILETYPEID),
                    new SqlParameter("@ENTITYID", EID),
                    new SqlParameter("@DEFPAYDATE", PAYDATE),
                    new SqlParameter("@DURATION", DURATION)

                };
                
                ds = DataLib.ExecuteDataSet("GetDataFromQry", CommandType.StoredProcedure, parameters);
                List<SeriesData> objList = new List<SeriesData>();
                SeriesData ObjData = null;
                
                List<string> lstCategories = new List<string>();
                for (int Comp = 0; Comp < ds.Tables[0].Rows.Count; Comp++)
                {
                    lstCategories.Add(ds.Tables[0].Rows[Comp]["PayDate"].ToString());
                }
                    //Now Loop through Component
                    for (int k = 1; k < ds.Tables[0].Columns.Count; k++)
                   
                     {
                    
                    int addflag = 0;
                    var ObjData1 = new SeriesData();
                    List<decimal> LstCate = new List<decimal>();
                    for (int Comp = 0; Comp < ds.Tables[0].Rows.Count; Comp++)
                    {
                        LstCate.Add(Convert.ToInt32(ds.Tables[0].Rows[Comp][k]));
                        if (Convert.ToInt32(ds.Tables[0].Rows[Comp][k]) != 0)
                        {
                            addflag = 1;
                            ObjData1.name = ds.Tables[0].Columns[k].ColumnName;
                            
                        }
                        
                    }
                    if (addflag == 1)
                    {
                        ObjData1.data = LstCate;
                        objList.Add(ObjData1);
                    }

                 }
                BarData.categorise = lstCategories;
                BarData.series = objList;
                return BarData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public List<SeriesData> GetChartComponentList(DataSet ds)
        {
            List<SeriesData> series = new List<SeriesData>();
            foreach (DataColumn col in ds.Tables[0].Columns)
            {
                List<Decimal> objString = new List<Decimal>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    objString.Add(Convert.ToDecimal(row[col.ColumnName]));

                }
                series.Add(new SeriesData(col.ColumnName, objString));
            }
            return series;
        }

        public class SeriesData
        {
            public SeriesData(string SeriesName, List<decimal> value)
            {
                this.name = SeriesName;
                this.data = value;
               

            }

            public SeriesData() { }
            public string name { get; set; }
            public List<decimal> data { get; set; }
         
        }
        
        public class HorizontalBarChartData
        {
            public List<string> categorise { get; set;}
            public List<SeriesData> series { get; set; }
        }

    }
}