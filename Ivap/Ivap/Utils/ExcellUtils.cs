using ClosedXML.Excel;
using Ivap.Areas.Master.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;


namespace Ivap.Utils
{
    public class ExcellUtils
    {
        public static DataTable ExlToDataTableMTR(string Path, string extension)
        {
            try
            {
                DataTable dt = new DataTable();
                string conString = string.Empty;
                switch (extension.Trim().ToUpper())
                {
                    case "XLS": //Excel 97-03.
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                        break;
                    case "XLSX": //Excel 07 and above.
                        conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                        break;
                }

                conString = string.Format(conString, Path);
                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            string sheetName = "";
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            for (int i = 0; i < dtExcelSchema.Rows.Count;i++ )
                            {
                                if (!dtExcelSchema.Rows[i]["TABLE_NAME"].ToString().Contains("FilterDatabase"))
                                {
                                    sheetName = dtExcelSchema.Rows[i]["TABLE_NAME"].ToString();
                                    break;
                                }
                            }

                                
                            connExcel.Close();

                            //Read Data from First Sheet.
                            string StrQry = "select MTRID+'' As MTRID,MOVEMENT_TYPE,TRANSACTION_NUMBER,REQUEST_DATE +'' AS REQUEST_DATE,ITEM_CODE,ITEM_DESCRIPTION,UOM,QUANTITY,SOURCE_id+'' AS SOURCE_id ,DELIVERY_LOCATION+'' AS DELIVERY_LOCATION,DESTINATION_ID+'' As DESTINATION_ID ,DELIVERY_CHALLAN,INVOICE from  [" + sheetName + "]";
                            connExcel.Open();
                            cmdExcel.CommandText = StrQry;
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }
                return dt;
            }
            catch
            {
                throw;
            }
        }
        public static DataTable ExlToDataTable(string Path, string extension)
        {
            try
            {
                DataTable dt = new DataTable();
                string conString = string.Empty;
                switch (extension.Trim().ToUpper())
                {
                    case "XLS": //Excel 97-03.
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                    case "XLSX": //Excel 07 and above.
                        conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                        break;
                }

                conString = string.Format(conString, Path);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            for (int i = 0; i < dtExcelSchema.Rows.Count; i++)
                            {
                                if (!dtExcelSchema.Rows[i]["TABLE_NAME"].ToString().Contains("FilterDatabase"))
                                {
                                    sheetName = dtExcelSchema.Rows[i]["TABLE_NAME"].ToString();
                                    break;
                                }
                            }
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }
                return dt;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public static DataTable ExlToDataTableNew(string Path, string extension)
        {
            try
            {
                DataTable dt = new DataTable();
                string conString = string.Empty;
                switch (extension.Trim().ToUpper())
                {
                    case "XLS": //Excel 97-03.
                        conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=NO;IMEX=1'";
                        break;
                    case "XLSX": //Excel 07 and above.
                        conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=NO;IMEX=1'";
                        break;
                }

                conString = string.Format(conString, Path);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            for (int i = 0; i < dtExcelSchema.Rows.Count; i++)
                            {
                                if (!dtExcelSchema.Rows[i]["TABLE_NAME"].ToString().Contains("FilterDatabase"))
                                {
                                    sheetName = dtExcelSchema.Rows[i]["TABLE_NAME"].ToString();
                                    break;
                                }
                            }
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            DataRow rowDel = dt.Rows[0];
                            
                            //Now Change Column name
                            for(int i=0;i<dt.Columns.Count;i++)
                            {
                                string columnName = rowDel[i].ToString().Trim();
                                dt.Columns[i].ColumnName = rowDel[i].ToString().Trim();
                            }
                            dt.Rows.Remove(rowDel);
                            //string Qry = "";
                            //for (int i = 0; i < dt.Columns.Count; i++)
                            //{
                            //    Qry = Qry + ",[" + dt.Columns[i].ColumnName + "]+'' AS [" + dt.Columns[i].ColumnName + "]";
                            //}
                            //Qry = Qry.TrimEnd(',').TrimStart(',');
                            //Qry = "select " + Qry + " From[" + sheetName + "]";
                            //DataTable dtNew = new DataTable();
                            //cmdExcel.CommandText = Qry;
                            //odaExcel.SelectCommand = cmdExcel;
                            //odaExcel.Fill(dtNew);

                            connExcel.Close();
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string DataTableToExcel(DataTable dt)
        {
            try
            {
                string FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Date.DayOfYear + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".xlsx";
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                using (XLWorkbook wb = new XLWorkbook())
                {
                    IXLWorksheet sheet;
                    sheet = wb.Worksheets.Add(dt, "Result");
                    sheet.Tables.FirstOrDefault().ShowAutoFilter = false;
                    wb.SaveAs(FilePath);
                }
                return FileName;
            }
            catch (Exception ex) { throw new Exception("DataTableToExcel"); }
        }




        public static void DataTableToExcel(DataTable dt, string FileName)
        {

            string[] SheetName = FileName.Split('.');
            try
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt, SheetName[0].ToString());

                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.Buffer = true;
                    HttpContext.Current.Response.Charset = "";
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + FileName + "");
                    // HttpContext.Current.Response.ContentType = "application/ms-excel";
                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.End();
                    }
                }
            }
            catch
            {
                throw new Exception("DataTableToExcel");
            }
        }
        public static DataTable GetDisplayName(int EID,string TableName,string ActionName)
        {
            DataSet ds = new DataSet();
            DataTable Dt = new DataTable();
            try
            {
                MasterMetaRepo objRepo = new MasterMetaRepo(EID, TableName, ActionName);
                ds = objRepo.GetMasterMetaData();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Dt.Columns.Add("TID");
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Dt.Columns.Add(ds.Tables[0].Rows[i]["DISPLAY_NAME"].ToString());
                        Dt.AcceptChanges();
                    }
                }
                return Dt;
            }
            catch(Exception ex) { throw; }
        }

        public static bool CheckColumnFormat(string FilePath, int EID, string TableName, string ActionName)
        {
            bool ret = true;
            DataSet ds = new DataSet();
            try
            {
                DataTable DtSample = new DataTable();
                DtSample = GetDisplayName(EID, TableName, ActionName);
                DataTable dtFile = ExcellUtils.ExlToDataTable(FilePath, "XLSX");
                for (int i = 0; i < DtSample.Columns.Count; i++)
                {
                    if (!(dtFile.Columns.Contains(DtSample.Columns[i].ColumnName)))
                    {
                        ret = false;
                        return ret;
                    }
                }
            }
            catch(Exception ex){ throw; }
            return ret;
        }

        


    }
}