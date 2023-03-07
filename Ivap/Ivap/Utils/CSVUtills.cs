using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Web.Hosting;
using Excel = Microsoft.Office.Interop.Excel;
using System.Data;
using ClosedXML.Excel;
namespace Ivap.Utils
{
    public static  class CSVUtills
    {

        

        public static string DataTableToCSV(DataTable dt, string  seperator)
        { 
            try
            {
                StringBuilder sb = new StringBuilder();
                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                  Select(column => column.ColumnName);
                sb.AppendLine(string.Join(seperator, columnNames));
                foreach (DataRow row in dt.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field =>
                     string.Concat("\"", field.ToString().Replace("\"", "\"\"").Replace("=", "'=").Replace("+", "'+"), "\"").Replace("-", "'-"));
                    sb.AppendLine(string.Join(seperator, fields));
                }
                string FileName = DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Date.DayOfYear + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".csv";
                string FilePath = HostingEnvironment.MapPath("~/Docs/Temp/") + FileName;
                File.WriteAllText(FilePath, sb.ToString());
                return FileName;
            }
            catch
            {
                throw new Exception("DataTableToCSV");
            }
        }

        public static DataTable CSVToDataTable(string strDataFilePath, char  Seperator)
        {
            // GetDataFromExcel123(strDataFilePath)
            try
            {
                
                StreamReader sr = new StreamReader(strDataFilePath);
                string fullFileStr = sr.ReadToEnd();
                fullFileStr = fullFileStr.Replace("'=", "=").Replace("'+", "+").Replace( "'-","-");
                fullFileStr = fullFileStr.Replace("'", "''");
                //fullFileStr=fullFileStr.Replace(System.Environment.NewLine, " ");
                sr.Dispose();
                string[] lines = fullFileStr.Split(ControlChars.Lf);
                DataTable recs = new DataTable();
                string[] sArr = lines[0].Split(Seperator);
                foreach (string s in sArr)
                {
                    recs.Columns.Add(new DataColumn(s.Trim()));
                }
                DataRow row = default(DataRow);
                string finalLine = "";
                int i = 0;
                foreach (string line in lines)
                {
                    if (i > 0 & !string.IsNullOrEmpty(line.Trim()))
                    {
                        row = recs.NewRow();
                        finalLine = line.Replace(Convert.ToString(ControlChars.Cr), "");
                        row.ItemArray = finalLine.Split(Seperator);
                        recs.Rows.Add(row);
                    }
                    i = i + 1;
                }
                return recs;
            }
            catch (Exception ex)
            {
                throw new Exception("Please remove all the comma from your file.");
            }
        }

        public static DataTable CSVHeaderToDataTable(string strDataFilePath, char Seperator)
        {
            // GetDataFromExcel123(strDataFilePath)
            try
            {

                StreamReader sr = new StreamReader(strDataFilePath);
                string fullFileStr = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                string[] lines = fullFileStr.Split(ControlChars.Lf);
                DataTable recs = new DataTable();
                string[] sArr = lines[0].Split(Seperator);
                foreach (string s in sArr)
                {
                    recs.Columns.Add(new DataColumn(s.Trim()));
                }

                return recs;
            }
            catch (Exception ex)
            {
                throw new Exception("CSVToDataTable");
            }
        }

    }

    public sealed class ControlChars
    {
        public const char Back = '\b';
        public const char Cr = '\r';
        public const string CrLf = "\r\n";
        public const char FormFeed = '\f';
        public const char Lf = '\n';
        public const string NewLine = "\r\n";
        public const char NullChar = '\0';
        public const char Quote = '"';
        public const char Tab = '\t';
        public const char VerticalTab = '\v';
    } 
}